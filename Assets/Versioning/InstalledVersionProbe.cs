using System;
using System.Collections;
using System.IO;
using UnityEngine;
using TMPro;

/// <summary>Opt-in installed-player check; dormant during normal play.</summary>
public sealed class InstalledVersionProbe : MonoBehaviour
{
    [Serializable] private sealed class Result
    {
        public string product, version, label, platform;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Launch()
    {
        var args = Environment.GetCommandLineArgs();
        int index = Array.IndexOf(args, "--version-smoke-output");
        if (index < 0 || index + 1 >= args.Length) return;
        var go = new GameObject("InstalledVersionProbe");
        var probe = go.AddComponent<InstalledVersionProbe>();
        probe.StartCoroutine(probe.Run(args, args[index + 1]));
    }

    private IEnumerator Run(string[] args, string output)
    {
        yield return null; // Menu Start methods have created their Settings panel.

        SettingsVersionFooter footer = null;
        foreach (var candidate in Resources.FindObjectsOfTypeAll<SettingsVersionFooter>())
            if (candidate.gameObject.scene.IsValid()) { footer = candidate; break; }
        if (footer == null) { Debug.LogError("No Settings version footer in entry scene"); Application.Quit(2); yield break; }
        footer.transform.parent.gameObject.SetActive(true);
        footer.transform.parent.SetAsLastSibling();
        yield return null;
        var label = footer.GetComponent<TextMeshProUGUI>().text;
        int expectedIndex = Array.IndexOf(args, "--expected-version");
        bool matches = label == ProductBuildInfo.VersionLabel && !string.IsNullOrEmpty(ProductBuildInfo.Version)
            && (expectedIndex < 0 || (expectedIndex + 1 < args.Length && ProductBuildInfo.Version == args[expectedIndex + 1]));
        if (!matches) { Debug.LogError("Installed version/UI mismatch: " + label); Application.Quit(3); yield break; }
        File.WriteAllText(output, JsonUtility.ToJson(new Result {
            product = Application.productName, version = ProductBuildInfo.Version,
            label = label, platform = Application.platform.ToString()
        }, true));
        int screenshotIndex = Array.IndexOf(args, "--version-smoke-screenshot");
        if (screenshotIndex >= 0 && screenshotIndex + 1 < args.Length)
        {
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(args[screenshotIndex + 1]);
            yield return new WaitForSecondsRealtime(1);
        }
        Application.Quit(0);
    }
}
