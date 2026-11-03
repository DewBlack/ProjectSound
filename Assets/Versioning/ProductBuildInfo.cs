using UnityEngine;

/// <summary>Installed player metadata; never fetches the latest server release.</summary>
public static class ProductBuildInfo
{
    public static string Version { get { return Application.version; } }
    public static bool Spanish { get { return Application.systemLanguage == SystemLanguage.Spanish; } }
    public static string VersionLabel { get { return (Spanish ? "Versión " : "Version ") + Version; } }
    public static string SettingsTitle { get { return Spanish ? "Ajustes" : "Settings"; } }
}
