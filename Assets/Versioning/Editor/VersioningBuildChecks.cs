using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>Read-only validation and an explicitly requested local smoke build.</summary>
public static class VersioningBuildChecks
{
    private static void Tool(string command)
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        var info = new ProcessStartInfo(Environment.GetEnvironmentVariable("PLG_PYTHON") ?? "python",
            "\"" + Path.Combine(root, "scripts/versioning/stack_version.py") + "\" --root \"" + root + "\" " + command);
        info.WorkingDirectory = root;
        info.UseShellExecute = false; info.CreateNoWindow = true;
        info.RedirectStandardOutput = true; info.RedirectStandardError = true;
        using (var process = Process.Start(info))
        {
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd(); process.WaitForExit();
            if (process.ExitCode != 0) throw new BuildFailedException("Version validation failed: " + output + error);
            UnityEngine.Debug.Log(output);
        }
    }

    public static void Validate() { Tool("check"); }

    public static void BuildSmoke()
    {
        Validate();
        string output = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "builds/versioning-smoke/player.exe");
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray(),
            locationPathName = output, target = BuildTarget.StandaloneWindows64, options = BuildOptions.Development
        });
        if (report.summary.result != BuildResult.Succeeded) throw new BuildFailedException("Smoke build failed");
        Tool("build-info --platform windows --output builds/versioning-smoke/build-version.json");
    }

    public static void ValidateBuild(BuildTarget target, BuildOptions options)
    {
        bool publishedAndroid = target == BuildTarget.Android && (options & BuildOptions.Development) == 0;
        Tool(publishedAndroid ? "check --release --platform android" : "check");
    }
}

public sealed class VersioningBuildGuard : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return -1000; } }
    public void OnPreprocessBuild(BuildReport report) { VersioningBuildChecks.ValidateBuild(report.summary.platform, report.summary.options); }
}
