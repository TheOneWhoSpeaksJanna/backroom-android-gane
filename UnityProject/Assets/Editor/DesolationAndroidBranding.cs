#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

[InitializeOnLoad]
public static class DesolationAndroidBranding
{
    const string IconPath = "Assets/AppIcon.png";

    static DesolationAndroidBranding()
    {
        EditorApplication.delayCall += Apply;
    }

    [MenuItem("Desolation/Apply Android Branding")]
    public static void Apply()
    {
        TextureImporter importer = AssetImporter.GetAtPath(IconPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Default;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.maxTextureSize = 1024;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.SaveAndReimport();
        }

        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath);
        if (icon != null)
        {
            Texture2D[] icons = new Texture2D[] { icon, icon, icon, icon, icon, icon, icon, icon };
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, icons);
        }

        PlayerSettings.companyName = "Desolation";
        PlayerSettings.productName = "Desolation: The Backrooms";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.desolation.thebackrooms");
        AssetDatabase.SaveAssets();
    }
}

public sealed class DesolationAndroidBrandingPrebuild : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        DesolationAndroidBranding.Apply();
    }
}
#endif
