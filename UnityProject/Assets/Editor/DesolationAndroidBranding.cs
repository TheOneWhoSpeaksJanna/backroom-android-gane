#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public sealed class DesolationAndroidBrandingPrebuild : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return -1000; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        ApplyBranding();
    }

    [InitializeOnLoadMethod]
    private static void ApplyBrandingWhenEditorLoads()
    {
        ApplyBranding();
    }

    private static void ApplyBranding()
    {
        PlayerSettings.companyName = "Desolation";
        PlayerSettings.productName = "Desolation: The Backrooms";
        PlayerSettings.bundleVersion = "0.1.0";
        PlayerSettings.defaultScreenOrientation = UIOrientation.AutoRotation;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.desolation.thebackrooms");
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;

        // Prefer the UI Kit app icon if available, otherwise generate one
        const string uiKitIconPath = "Assets/UI/06_app_icon_512.png";
        const string iconPath = "Assets/AppIcon.png";
        Texture2D uiKitIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(uiKitIconPath);
        if (uiKitIcon != null)
        {
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, new[] { uiKitIcon });
        }
        else
        {
            EnsureIcon(iconPath);
            AssetDatabase.ImportAsset(iconPath, ImportAssetOptions.ForceUpdate);
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (icon != null)
            {
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, new[] { icon });
            }
        }

        AssetDatabase.SaveAssets();
    }

    private static void EnsureIcon(string assetPath)
    {
        Texture2D texture = new Texture2D(512, 512, TextureFormat.RGBA32, false);
        Color clear = new Color(0, 0, 0, 0);
        Color black = new Color(0.012f, 0.010f, 0.006f, 1f);
        Color rim = new Color(1f, 0.72f, 0.14f, 1f);
        Color top = new Color(0.96f, 0.96f, 0.92f, 1f);
        Color left = new Color(0.72f, 0.72f, 0.70f, 1f);
        Color right = new Color(0.86f, 0.86f, 0.82f, 1f);

        for (int y = 0; y < 512; y++)
        {
            for (int x = 0; x < 512; x++)
            {
                texture.SetPixel(x, y, InsideRoundedRect(x, y, 50, 50, 412, 412, 72) ? black : clear);
            }
        }

        DrawGlow(texture, 256, 260, 152, new Color(1f, .72f, .12f, .08f));
        FillPoly(texture, new Vector2[] { new Vector2(256, 126), new Vector2(386, 202), new Vector2(256, 278), new Vector2(126, 202) }, top);
        FillPoly(texture, new Vector2[] { new Vector2(126, 202), new Vector2(256, 278), new Vector2(256, 408), new Vector2(126, 332) }, left);
        FillPoly(texture, new Vector2[] { new Vector2(386, 202), new Vector2(256, 278), new Vector2(256, 408), new Vector2(386, 332) }, right);
        FillPoly(texture, new Vector2[] { new Vector2(206, 210), new Vector2(256, 180), new Vector2(306, 210), new Vector2(256, 240) }, black);
        FillPoly(texture, new Vector2[] { new Vector2(176, 252), new Vector2(235, 286), new Vector2(235, 350), new Vector2(176, 316) }, black);
        FillPoly(texture, new Vector2[] { new Vector2(336, 252), new Vector2(277, 286), new Vector2(277, 350), new Vector2(336, 316) }, black);
        DrawFrame(texture, rim);

        texture.Apply();
        File.WriteAllBytes(assetPath, texture.EncodeToPNG());
    }

    private static bool InsideRoundedRect(int x, int y, int rx, int ry, int w, int h, int r)
    {
        int px = Mathf.Clamp(x, rx + r, rx + w - r);
        int py = Mathf.Clamp(y, ry + r, ry + h - r);
        int dx = x - px;
        int dy = y - py;
        return x >= rx && x <= rx + w && y >= ry && y <= ry + h && dx * dx + dy * dy <= r * r;
    }

    private static void DrawGlow(Texture2D t, int cx, int cy, int radius, Color color)
    {
        for (int y = cy - radius; y <= cy + radius; y++)
        {
            for (int x = cx - radius; x <= cx + radius; x++)
            {
                if (x < 0 || y < 0 || x >= t.width || y >= t.height) continue;
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy)) / radius;
                if (d <= 1f)
                {
                    Color old = t.GetPixel(x, y);
                    t.SetPixel(x, y, Color.Lerp(old, color, (1f - d) * color.a));
                }
            }
        }
    }

    private static void DrawFrame(Texture2D t, Color color)
    {
        for (int i = 0; i < 3; i++)
        {
            FillPoly(t, new Vector2[] { new Vector2(118 - i, 202), new Vector2(256, 120 - i), new Vector2(394 + i, 202), new Vector2(256, 284 + i) }, new Color(color.r, color.g, color.b, .18f));
        }
    }

    private static void FillPoly(Texture2D t, Vector2[] p, Color color)
    {
        int minX = 511, minY = 511, maxX = 0, maxY = 0;
        foreach (Vector2 v in p)
        {
            minX = Mathf.Min(minX, Mathf.RoundToInt(v.x));
            minY = Mathf.Min(minY, Mathf.RoundToInt(v.y));
            maxX = Mathf.Max(maxX, Mathf.RoundToInt(v.x));
            maxY = Mathf.Max(maxY, Mathf.RoundToInt(v.y));
        }

        for (int y = Mathf.Clamp(minY, 0, 511); y <= Mathf.Clamp(maxY, 0, 511); y++)
        {
            for (int x = Mathf.Clamp(minX, 0, 511); x <= Mathf.Clamp(maxX, 0, 511); x++)
            {
                if (PointInPolygon(new Vector2(x, y), p))
                    t.SetPixel(x, y, color);
            }
        }
    }

    private static bool PointInPolygon(Vector2 point, Vector2[] polygon)
    {
        bool inside = false;
        for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
        {
            if (((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
            {
                inside = !inside;
            }
        }
        return inside;
    }
}
#endif
