import hashlib
import importlib.util
import json
from pathlib import Path
import shutil
import sys
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[3]
TOOLS = ROOT / "scripts/versioning"
sys.path.insert(0, str(TOOLS))
import stack_version as stack
import update_stack_tools

class UnityIntegrationTests(unittest.TestCase):
    def test_real_source_semver_and_idempotent_sync(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / "ProjectSettings").mkdir()
            asset = root / "ProjectSettings/ProjectSettings.asset"
            original = (ROOT / "ProjectSettings/ProjectSettings.asset").read_bytes()
            asset.write_bytes(original)
            baseline = stack.check(root)
            first = stack.sync(root)
            after = asset.read_bytes()
            self.assertEqual([], stack.sync(root)["changed"])
            self.assertEqual(after, asset.read_bytes())
            self.assertEqual(baseline["version_code"], stack.check(root)["version_code"])
            self.assertEqual(baseline["application_id"], stack.check(root)["application_id"])
            target = stack.Version.parse(baseline["version"]).bump_patch().replace(prerelease="beta.10")
            stack.sync(root, str(target), dry_run=True)
            self.assertEqual(after, asset.read_bytes())
            stack.sync(root, str(target))
            self.assertEqual(str(target), stack.check(root)["version"])
            self.assertEqual(baseline["application_id"], stack.check(root)["application_id"])
            self.assertEqual(baseline["version_code"], stack.check(root)["version_code"])

    def test_both_immutable_toolsets_match_reviewed_hashes(self):
        for name, lockname in (("TOOLSET.json", "versioning.lock.json"), ("STACK_TOOLSET.json", "stack-versioning.lock.json")):
            lock = json.loads((ROOT / "scripts" / lockname).read_text(encoding="utf-8"))
            raw = (TOOLS / name).read_bytes()
            self.assertEqual(lock["manifest_sha256"], hashlib.sha256(raw).hexdigest())
            for file, sha in json.loads(raw)["files"].items():
                self.assertEqual(sha, hashlib.sha256((TOOLS / file).read_bytes()).hexdigest(), file)

    def test_stack_update_verifies_all_bytes_and_dry_run(self):
        raw = (TOOLS / "STACK_TOOLSET.json").read_bytes()
        fetch = lambda name: (TOOLS / name).read_bytes()
        with tempfile.TemporaryDirectory() as directory:
            root=Path(directory); (root / "scripts").mkdir()
            lock = json.loads((ROOT / "scripts/stack-versioning.lock.json").read_text(encoding="utf-8"))
            result=update_stack_tools.update(root, lock["revision"], lock["manifest_sha256"], True, fetch)
            self.assertGreater(result["verified_files"], 10)
            self.assertFalse((root / "scripts/versioning").exists())
            with self.assertRaises(ValueError):
                update_stack_tools.update(root, lock["revision"], "0"*64, False, fetch)
            self.assertFalse((root / "scripts/versioning").exists())

    def test_settings_consume_installed_metadata_without_literal_version(self):
        provider=(ROOT / "Assets/Versioning/ProductBuildInfo.cs").read_text(encoding="utf-8")
        footer=(ROOT / "Assets/Versioning/SettingsVersionFooter.cs").read_text(encoding="utf-8")
        self.assertIn("Application.version", provider)
        self.assertIn("ProductBuildInfo.VersionLabel", footer)
        self.assertIn("label.raycastTarget = false", footer)
        self.assertNotIn(stack.source(ROOT)["version"], provider + footer)
        self.assertNotIn("UnityEditor", provider + footer)
        probe=(ROOT / "Assets/Versioning/InstalledVersionProbe.cs").read_text(encoding="utf-8")
        self.assertIn('"--expected-version"', probe)
        self.assertIn("label == ProductBuildInfo.VersionLabel", probe)

    def test_settings_entry_is_integrated_with_real_menu(self):
        files={p.relative_to(ROOT).as_posix():p.read_text(encoding="utf-8-sig") for p in (ROOT/"Assets/Versioning").glob("*.cs")}
        if "Assets/Versioning/VersioningMainMenu.cs" in files:
            build=(ROOT/"ProjectSettings/EditorBuildSettings.asset").read_text(encoding="utf-8")
            self.assertLess(build.index("VersioningMainMenu.unity"), build.index("DemoScene1.unity"))
            self.assertIn('SceneManager.LoadScene("DemoScene1")', files["Assets/Versioning/VersioningMainMenu.cs"])
        elif "Assets/Versioning/CrimsonSettingsEntry.cs" in files:
            self.assertIn('Assets/Menu/Scenes/Menu.unity', files["Assets/Versioning/CrimsonSettingsEntry.cs"])
            self.assertIn("CrimsonSettingsEntry.Install()", (ROOT/"Assets/Menu/Scripts/PlayButtonScript.cs").read_text(encoding="utf-8"))
            self.assertIn("raycastTarget = false", files["Assets/Versioning/MinimalSettingsMenu.cs"])
        elif (ROOT/"Assets/Scripts/Hud/ViewBase.cs").exists():
            self.assertIn('if (_viewID == "Settings") SettingsVersionFooter.Ensure(transform)', (ROOT/"Assets/Scripts/Hud/ViewBase.cs").read_text(encoding="utf-8"))
            self.assertIn('_viewID: Settings', (ROOT/"Assets/Scenes/Main.unity").read_text(encoding="utf-8"))
        else:
            self.assertIn('child.name == "Settings"', (ROOT/"Assets/Scripts/buttonsMenu.cs").read_text(encoding="utf-8-sig"))
            self.assertIn('m_Name: Settings', (ROOT/"Assets/Prefabs/MenuPrincipal.prefab").read_text(encoding="utf-8"))

if __name__ == "__main__": unittest.main()
