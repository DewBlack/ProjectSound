#!/usr/bin/env python3
"""Adaptadores de producto para Unity, Android nativo y proyectos npm/estáticos.

Comparación SemVer y contador se reutilizan del toolset fijado de la plantilla.
No publica, no crea tags y no asigna códigos al inspeccionar o sincronizar.
"""
from __future__ import annotations

import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import subprocess
import sys
import zipfile
import xml.etree.ElementTree as ET

HERE = Path(__file__).resolve().parent
sys.path.insert(0, str(HERE / "vendor"))
from semver import Version

MAX_CODE = 2100000000


def run(args, root=None):
    p = subprocess.run(args, cwd=root, capture_output=True, text=True)
    if p.returncode:
        raise ValueError(f"{args[0]}: {p.stderr.strip()}")
    return p.stdout.strip()


def text(path):
    return path.read_text(encoding="utf-8-sig")


def one(content, pattern, label):
    values = re.findall(pattern, content, re.M)
    if len(values) != 1:
        raise ValueError(f"{label}: se esperaba una fuente, se encontraron {len(values)}")
    return values[0]


def source(root):
    unity, gradle, npm, plain = (root / p for p in (
        "ProjectSettings/ProjectSettings.asset", "app/build.gradle", "package.json", "VERSION"))
    if unity.exists():
        content = text(unity)
        version = one(content, r"^  bundleVersion:\s*(\S+)\s*$", "Unity bundleVersion").strip('"\'')
        code = int(one(content, r"^  AndroidBundleVersionCode:\s*(\d+)\s*$", "Unity Android code"))
        product = one(content, r"^  productName:\s*(.+)$", "Unity productName").strip()
        identifier_map = re.search(r"(?m)^  applicationIdentifier:[^\n]*\n((?:^    [^\n]*\n)*)", content)
        android = re.findall(r"^    Android:[ \t]*([^\s{}]+)[ \t]*$", identifier_map[1], re.M) if identifier_map else []
        application_id = android[0] if len(android) == 1 else None
        return dict(adapter="unity", version=version, version_code=code, product=product,
                    application_id=application_id, source="ProjectSettings/ProjectSettings.asset")
    if gradle.exists():
        content = text(gradle)
        application_id = one(content, r'^\s*applicationId\s+["\']([^"\']+)["\']\s*$', "Gradle applicationId")
        resources = root / "app/src/main/res/values/strings.xml"
        name = ET.fromstring(text(resources)).find("string[@name='app_name']") if resources.exists() else None
        return dict(adapter="gradle", version=one(content, r'^\s*versionName\s+["\']([^"\']+)["\']\s*$', "Gradle versionName"),
                    version_code=int(one(content, r"^\s*versionCode\s+(\d+)\s*$", "Gradle versionCode")),
                    application_id=application_id,
                    product=name.text if name is not None else application_id, source="app/build.gradle")
    if npm.exists():
        data = json.loads(text(npm))
        if "version" not in data:
            raise ValueError("package.json no tiene versión de producto; falta decidir su valor inicial")
        return dict(adapter="npm", version=data["version"], product=data["name"], source="package.json")
    if plain.exists():
        return dict(adapter="file", version=text(plain).strip(), product=root.name, source="VERSION")
    raise ValueError("No hay fuente soportada: Unity ProjectSettings, app/build.gradle, package.json o VERSION")


def check(root, release=False, platform=None):
    info = source(root)
    Version.parse(info["version"])
    if "version_code" in info and not 1 <= info["version_code"] <= MAX_CODE:
        raise ValueError("versionCode debe estar entre 1 y 2100000000")
    if info["adapter"] == "gradle":
        manifest = root / "app/src/main/AndroidManifest.xml"
        if manifest.exists() and re.search(r'android:version(?:Name|Code)\s*=', text(manifest)):
            raise ValueError("El manifiesto duplica una versión: conservar solo la fuente Gradle")
    if info["adapter"] == "npm" and (root / "package-lock.json").exists():
        lock = json.loads(text(root / "package-lock.json"))
        for container in (lock, lock.get("packages", {}).get("", {})):
            if "version" in container and container["version"] != info["version"]:
                raise ValueError("package-lock.json no coincide con package.json; ejecutar sync")
    if release and info["adapter"] == "unity" and platform is None:
        raise ValueError("Unity: indicar --platform android|windows|linux|macos|web para validar la release")
    if release and "version_code" in info and (info["adapter"] == "gradle" or platform == "android"):
        if not info.get("application_id"):
            raise ValueError("Falta applicationId Android real; no se inventa uno para publicar")
        request = os.environ.get("PLG_ANDROID_RESERVATION_ID")
        if not request:
            raise ValueError("Falta PLG_ANDROID_RESERVATION_ID reservado en android_counter.py")
        from android_counter import Ledger
        Ledger().validate_reservation(info["application_id"], request, info["version_code"])
    return info


def sync(root, version=None, code=None, dry_run=False, allow_major=False):
    info = source(root)
    old = Version.parse(info["version"])
    new = Version.parse(version) if version is not None else old
    if new < old:
        raise ValueError("La versión no puede retroceder")
    if new.major > old.major and not allow_major:
        raise ValueError("MAJOR requiere decisión explícita justificada y --allow-major")
    if new.prerelease and not re.fullmatch(r"(?:alpha|beta|rc)\.[0-9]+", new.prerelease):
        raise ValueError("Una candidata nueva usa alpha.N, beta.N o rc.N")
    if code is not None and ("version_code" not in info or not 1 <= code <= MAX_CODE):
        raise ValueError("Código no aplicable o fuera de rango")
    pending = {}
    p = root / info["source"]
    content = text(p)
    if info["adapter"] == "unity":
        content = re.sub(r"(?m)^(  bundleVersion:)[ \t]*\S+[ \t]*$", lambda m: m[1] + " " + str(new), content)
        if code is not None:
            content = re.sub(r"(?m)^(  AndroidBundleVersionCode:)[ \t]*\d+[ \t]*$", lambda m: m[1] + " " + str(code), content)
    elif info["adapter"] == "gradle":
        content = re.sub(r'(?m)^([ \t]*versionName[ \t]+)["\'][^"\']+["\'][ \t]*$', lambda m: m[1] + json.dumps(str(new)), content)
        if code is not None:
            content = re.sub(r"(?m)^([ \t]*versionCode[ \t]+)\d+[ \t]*$", lambda m: m[1] + str(code), content)
    elif info["adapter"] == "npm":
        data = json.loads(content); data["version"] = str(new)
        content = json.dumps(data, ensure_ascii=False, indent=2) + "\n"
        lockpath = root / "package-lock.json"
        if lockpath.exists():
            lock = json.loads(text(lockpath)); lock["version"] = str(new)
            if "" in lock.get("packages", {}): lock["packages"][""]["version"] = str(new)
            pending[lockpath] = json.dumps(lock, ensure_ascii=False, indent=2) + "\n"
    else:
        content = str(new) + "\n"
    pending[p] = content
    changed = [str(path.relative_to(root)) for path, value in pending.items() if text(path) != value]
    if not dry_run:
        for path, value in pending.items():
            if text(path) != value: path.write_text(value, encoding="utf-8", newline="\n")
    return {**info, "version": str(new), "changed": changed, "dry_run": dry_run}


def inspect_artifact(path, aapt2=None):
    path = path.resolve(strict=True)
    result = {"file": path.name, "bytes": path.stat().st_size,
              "sha256": hashlib.sha256(path.read_bytes()).hexdigest()}
    if path.suffix.lower() == ".apk":
        output = run([aapt2 or "aapt2", "dump", "badging", str(path)])
        package, code, version = one(output, r"package: name='([^']+)' versionCode='(\d+)' versionName='([^']+)'", "APK metadata")
        result.update(application_id=package, version=version, version_code=int(code), platform="android")
    elif path.suffix.lower() == ".zip":
        with zipfile.ZipFile(path) as archive:
            if any(name in archive.namelist() for name in ("AndroidManifest.xml", "base/manifest/AndroidManifest.xml")):
                raise ValueError("Un paquete Android no se etiqueta como ZIP web; inspeccionarlo como APK")
            names = [name for name in archive.namelist() if Path(name).name == "build-version.json"]
            if len(names) != 1: raise ValueError("ZIP requiere un único build-version.json generado dentro del artefacto")
            data = json.loads(archive.read(names[0]))
            result.update(version=data["version"], product=data["product"], platform=data["platform"], embedded_commit=data.get("commit"))
    else:
        raise ValueError("Inspección soportada: APK real o ZIP con build-version.json incrustado; no se presenta la fuente como artefacto")
    Version.parse(result["version"])
    return result


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=HERE.parents[1])
    commands = parser.add_subparsers(dest="command", required=True)
    p = commands.add_parser("show"); p.add_argument("--json", action="store_true")
    p = commands.add_parser("check"); p.add_argument("--release", action="store_true"); p.add_argument("--platform", choices=["android", "windows", "linux", "macos", "web"])
    for command in ("sync", "prepare"):
        p = commands.add_parser(command)
        if command == "prepare": p.add_argument("version")
        p.add_argument("--code", type=int); p.add_argument("--dry-run", action="store_true"); p.add_argument("--allow-major", action="store_true")
    p = commands.add_parser("propose"); p.add_argument("bump", choices=["patch", "minor", "major"]); p.add_argument("--phase", choices=["alpha", "beta", "rc", "stable"], default="stable")
    p = commands.add_parser("changelog"); p.add_argument("--since"); p.add_argument("--output", type=Path); p.add_argument("--dry-run", action="store_true")
    p = commands.add_parser("build-info"); p.add_argument("--platform", required=True); p.add_argument("--output", type=Path, required=True); p.add_argument("--dry-run", action="store_true")
    for command in ("artifact-inspect", "manifest"):
        p = commands.add_parser(command); p.add_argument("artifact", type=Path); p.add_argument("--aapt2")
        if command == "manifest": p.add_argument("--output", type=Path); p.add_argument("--bind-reservation", action="store_true"); p.add_argument("--dry-run", action="store_true")
    p = commands.add_parser("promotion-verify"); p.add_argument("artifact", type=Path); p.add_argument("manifest", type=Path)
    args = parser.parse_args(argv); root = args.root.resolve()
    try:
        if args.command == "show": result = source(root)
        elif args.command == "check": result = check(root, args.release, args.platform)
        elif args.command in ("sync", "prepare"):
            result = sync(root, getattr(args, "version", None), args.code, args.dry_run, args.allow_major)
        elif args.command == "propose":
            version = getattr(Version.parse(source(root)["version"]), "bump_" + args.bump)()
            result = {"proposed": str(version if args.phase == "stable" else version.replace(prerelease=args.phase + ".1")), "applied": False}
        elif args.command == "changelog":
            result = "## Unreleased\n\n" + run(["git", "log", "--format=- %s", f"{args.since}..HEAD" if args.since else "HEAD"], root) + "\n"
            if args.output and not args.dry_run: args.output.write_text(result, encoding="utf-8")
        elif args.command == "build-info":
            result = {**check(root), "platform": args.platform, "commit": run(["git", "rev-parse", "HEAD"], root)}
            if not args.dry_run:
                args.output.parent.mkdir(parents=True, exist_ok=True)
                args.output.write_text(json.dumps(result, indent=2) + "\n", encoding="utf-8")
        elif args.command in ("artifact-inspect", "manifest"):
            result = inspect_artifact(args.artifact, args.aapt2)
            if args.command == "manifest":
                info = check(root)
                keys = ("version", "version_code", "application_id") if result["platform"] == "android" else ("version",)
                for key in keys:
                    if key in info and info[key] != result.get(key): raise ValueError(f"El artefacto no coincide con la fuente: {key}")
                if result["platform"] == "android" and not info.get("application_id"):
                    raise ValueError("El producto no declara un applicationId Android publicable")
                if result.get("embedded_commit") and result["embedded_commit"] != run(["git", "rev-parse", "HEAD"], root):
                    raise ValueError("El commit incrustado no corresponde al checkout de la build")
                result.update(product=info["product"], commit=run(["git", "rev-parse", "HEAD"], root), publishable=False)
                if args.bind_reservation:
                    if result["platform"] != "android": raise ValueError("La reserva Android no es aplicable a este artefacto")
                    from android_counter import Ledger
                    request = os.environ.get("PLG_ANDROID_RESERVATION_ID")
                    if not request: raise ValueError("Falta PLG_ANDROID_RESERVATION_ID")
                    result["artifact_binding"] = Ledger().bind_artifact(result["application_id"], request, result["version_code"], sha256=result["sha256"], version=result["version"], commit=result["commit"], dry_run=args.dry_run)
                    result.update(reservation_id=request, publishable=not args.dry_run)
                if args.output and not args.dry_run: args.output.write_text(json.dumps(result, indent=2) + "\n", encoding="utf-8")
        else:
            result = json.loads(text(args.manifest))
            if hashlib.sha256(args.artifact.read_bytes()).hexdigest() != result["sha256"] or args.artifact.stat().st_size != result["bytes"]: raise ValueError("Cambian los bytes: no es una promoción intacta")
            if args.artifact.suffix.lower() == ".apk" and result.get("platform") != "android":
                raise ValueError("Un APK no puede eludir la reserva declarándose web/escritorio")
            if result.get("platform") == "android":
                if not result.get("publishable"): raise ValueError("El artefacto local no tiene reserva publicable")
                from android_counter import Ledger
                Ledger().validate_reservation(result["application_id"], result["reservation_id"], result["version_code"], sha256=result["sha256"], version=result["version"], commit=result["commit"])
            result["verified"] = True
        print(result if isinstance(result, str) else json.dumps(result, indent=2, ensure_ascii=False))
        return 0
    except Exception as error:
        print(f"VERSION_ERROR: {error}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
