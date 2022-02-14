using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public static class CaptureScreenshotTool
{
    public const string k_editorPref = "CapturesScreenshotPath";
    public const string k_menuPath = "Tools/Capture Screenshot";
    
    [MenuItem(k_menuPath + " _F11")]
    public static void CaptureScreenToolMenuItem()
    {
        string path = EditorPrefs.GetString(k_editorPref);
        if (string.IsNullOrWhiteSpace(path)) path = GetDefaultPath();
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        string filepath = Path.Combine(path, string.Format("{0}_{1}.png", Application.productName, DateTime.Now
            .ToString("yyyymmddhhmmss")));

        ScreenCapture.CaptureScreenshot(filepath, 1);
    }

    public static string GetDefaultPath()
    {
        string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Screenshots");
        return defaultPath;
    }
}

public class CaptureScreenshotSettingsProvider : SettingsProvider
{
    public CaptureScreenshotSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope) {}

    public override void OnGUI(string searchContext)
    {
        base.OnGUI(searchContext);
        
        GUILayout.Space(20f);

        string path = EditorPrefs.GetString(CaptureScreenshotTool.k_editorPref);

        if (string.IsNullOrWhiteSpace(path)) path = CaptureScreenshotTool.GetDefaultPath();

        string changedPath = EditorGUILayout.TextField(path);

        if (string.Compare(path, changedPath) != 0) EditorPrefs.SetString(CaptureScreenshotTool.k_editorPref, changedPath);
        
        GUILayout.Space(10f);

        if (GUILayout.Button("Reset to Default", GUILayout.Width(150f)))
        {
            EditorPrefs.DeleteKey(CaptureScreenshotTool.k_editorPref);
            Repaint();
        }
    }

    [SettingsProvider]
    public static SettingsProvider CreateCaptureScreenshotSettingsProvider()
    {
        CaptureScreenshotSettingsProvider captureScreenshotSettingsProvider =
            new CaptureScreenshotSettingsProvider(CaptureScreenshotTool.k_menuPath);
        return captureScreenshotSettingsProvider;
    }
}