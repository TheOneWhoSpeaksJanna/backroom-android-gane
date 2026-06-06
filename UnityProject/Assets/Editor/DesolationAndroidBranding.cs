#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

[InitializeOnLoad]
public static class DesolationAndroidBranding
{
    const string IconPath = "Assets/AppIcon.jpg";

    static DesolationAndroidBranding()
    {
        EditorApplication.delayCall += Apply;
    }

    [MenuItem("Desolation/Apply Android Branding And Import Settings")]
    public static void Apply()
    {
        AssetDatabase.ImportAsset(IconPath, ImportAssetOptions.ForceUpdate);
        TextureImporter ti = AssetImporter.GetAtPath(IconPath) as TextureImporter;
        if (ti != null)
        {
            ti.textureType = TextureImporterType.Default;
            ti.mipmapEnabled = false;
            ti.alphaIsTransparency = false;
            ti.maxTextureSize = 1024;
            ti.textureCompression = TextureImporterCompression.CompressedHQ;
            ti.SaveAndReimport();
        }

        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath);
        if (icon != null)
        {
            Texture2D[] groupIcons = new Texture2D[8];
            for (int i = 0; i < groupIcons.Length; i++) groupIcons[i] = icon;
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, groupIcons);
            try
            {
                PlatformIconKind[] kinds = PlayerSettings.GetPlatformIconKindsForTargetGroup(BuildTargetGroup.Android);
                foreach (PlatformIconKind kind in kinds)
                {
                    PlatformIcon[] platformIcons = PlayerSettings.GetPlatformIcons(BuildTargetGroup.Android, kind);
                    for (int i = 0; i < platformIcons.Length; i++) platformIcons[i].SetTexture(icon);
                    PlayerSettings.SetPlatformIcons(BuildTargetGroup.Android, kind, platformIcons);
                }
            }
            catch (Exception)
            {
            }
        }

        PlayerSettings.companyName = "Desolation";
        PlayerSettings.productName = "Desolation: The Backrooms";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.desolation.thebackrooms");

        foreach (string guid in AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
            if (importer == null) continue;
            AudioImporterSampleSettings settings = importer.defaultSampleSettings;
            settings.loadType = AudioClipLoadType.CompressedInMemory;
            settings.compressionFormat = AudioCompressionFormat.Vorbis;
            settings.quality = .58f;
            importer.defaultSampleSettings = settings;
            importer.forceToMono = true;
            importer.SaveAndReimport();
        }

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
