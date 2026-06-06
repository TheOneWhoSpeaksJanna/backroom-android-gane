#if UNITY_EDITOR
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
        PlayerSettings.defaultScreenOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.desolation.thebackrooms");
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;

        Texture2D icon = LoadTexture("Assets/AppIcon.png");
        if (icon == null)
        {
            icon = LoadTexture("Assets/AppIcon.jpg");
        }

        if (icon != null)
        {
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, new[] { icon });
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new[] { icon });
        }

        AssetDatabase.SaveAssets();
    }

    private static Texture2D LoadTexture(string assetPath)
    {
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        if (texture != null)
        {
            return texture;
        }

        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
    }
}
#endif
