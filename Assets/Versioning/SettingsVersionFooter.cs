using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>A non-interactive footer, owned by the main Settings panel.</summary>
public sealed class SettingsVersionFooter : MonoBehaviour
{
    public static void Ensure(Transform settings)
    {
        if (settings.Find("InstalledVersion") != null) return;
        var footer = new GameObject("InstalledVersion", typeof(RectTransform));
        footer.transform.SetParent(settings, false);
        var rect = (RectTransform)footer.transform;
        // This existing Settings object is a 100x100 positioning container,
        // not a full-screen panel. Put the footer below its Back control.
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 1);
        rect.sizeDelta = new Vector2(580, 40);
        rect.anchoredPosition = new Vector2(40, -110);
        var sample = settings.GetComponentInChildren<TextMeshProUGUI>(true);
        var label = footer.AddComponent<TextMeshProUGUI>();
        if (sample != null) { label.font = sample.font; label.fontSharedMaterial = sample.fontSharedMaterial; }
        label.color = Color.white; label.fontSize = 24; label.enableAutoSizing = true;
        label.fontSizeMin = 16; label.fontSizeMax = 24;
        label.alignment = TextAlignmentOptions.Center;
        label.overflowMode = TextOverflowModes.Overflow;
        label.raycastTarget = false;
        footer.AddComponent<SettingsVersionFooter>();
    }

    private void OnEnable()
    {
        FitCanvasWidth();
        GetComponent<TextMeshProUGUI>().text = ProductBuildInfo.VersionLabel;
        Debug.Log("Installed product: " + Application.productName + " " + ProductBuildInfo.Version);
    }

    private void OnRectTransformDimensionsChange() { FitCanvasWidth(); }

    private void FitCanvasWidth()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;
        float scale = Mathf.Abs(transform.lossyScale.x / canvas.transform.lossyScale.x);
        if (scale <= 0) return;
        var rect = (RectTransform)transform;
        var size = rect.sizeDelta;
        size.x = Mathf.Max(100, Mathf.Min(580, ((RectTransform)canvas.transform).rect.width / scale - 64));
        if (Mathf.Abs(rect.sizeDelta.x - size.x) > 0.1f) rect.sizeDelta = size;
    }
}
