#!/usr/bin/env python3
"""Update the reviewed immutable studio stack toolset; verify before writing."""
import argparse
import hashlib
import json
from pathlib import Path, PurePosixPath
import re
import subprocess
import urllib.request

def update(root, revision=None, manifest_sha256=None, dry_run=False, fetch=None):
    lock = root / "scripts/stack-versioning.lock.json"
    previous = json.loads(lock.read_text(encoding="utf-8")) if lock.exists() else {}
    if revision and not manifest_sha256: raise ValueError("An explicit revision requires --manifest-sha256")
    revision = revision or previous.get("revision", "")
    manifest_sha256 = manifest_sha256 or previous.get("manifest_sha256", "")
    if not re.fullmatch(r"[0-9a-f]{40}", revision) or not re.fullmatch(r"[0-9a-f]{64}", manifest_sha256):
        raise ValueError("Pin a reviewed immutable studio revision and manifest SHA-256")
    if fetch is None:
        credential = subprocess.run(["gh", "auth", "token"], capture_output=True, text=True)
        token = credential.stdout.strip() if credential.returncode == 0 else ""
        def fetch(path):
            headers = {"Accept": "application/vnd.github.raw+json", "User-Agent": "personal-unity-versioning"}
            if token: headers["Authorization"] = "Bearer " + token
            request = urllib.request.Request("https://api.github.com/repos/pallobbygames/studio/contents/tools/versioning/" + path + "?ref=" + revision, headers=headers)
            with urllib.request.urlopen(request, timeout=30) as response: return response.read()
    raw = fetch("STACK_TOOLSET.json")
    if hashlib.sha256(raw).hexdigest() != manifest_sha256: raise ValueError("Reviewed manifest checksum mismatch")
    manifest = json.loads(raw); pending = {"STACK_TOOLSET.json": raw}
    for name, expected in manifest["files"].items():
        path = PurePosixPath(name)
        if path.is_absolute() or ".." in path.parts or "\\" in name or ":" in name or name == "STACK_TOOLSET.json":
            raise ValueError("Unsafe upstream path")
        data = fetch(name)
        if hashlib.sha256(data).hexdigest() != expected: raise ValueError("Upstream checksum mismatch: " + name)
        pending[name] = data
    if not dry_run:
        for name, data in pending.items():
            destination = root / "scripts/versioning" / name
            destination.parent.mkdir(parents=True, exist_ok=True); destination.write_bytes(data)
        lock.write_text(json.dumps({"upstream": "https://github.com/pallobbygames/studio", "path": "tools/versioning", "revision": revision, "manifest_sha256": manifest_sha256}, indent=2) + "\n")
    return {"revision": revision, "verified_files": len(pending), "dry_run": dry_run}

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[2])
    parser.add_argument("--revision"); parser.add_argument("--manifest-sha256"); parser.add_argument("--dry-run", action="store_true")
    args = parser.parse_args()
    try: print(json.dumps(update(args.root, args.revision, args.manifest_sha256, args.dry_run), indent=2))
    except (ValueError, OSError) as error: parser.exit(1, "STACK_UPDATE_ERROR: " + str(error) + "\n")
