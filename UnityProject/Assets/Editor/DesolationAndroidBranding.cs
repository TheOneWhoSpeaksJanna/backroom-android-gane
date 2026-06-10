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
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.desolation.thebackrooms");
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;

        // Use the project's AppIcon.png directly — no procedural generation
        const string iconPath = "Assets/AppIcon.png";
        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        if (icon != null)
        {
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, new[] { icon });
        }

        AssetDatabase.SaveAssets();
    }
}
#endif
