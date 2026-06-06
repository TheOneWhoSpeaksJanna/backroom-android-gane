using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

public sealed class TextureBridgeBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => -1000;

    [InitializeOnLoadMethod]
    static void Load()
    {
        Sync();
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        Sync();
    }

    static void Sync()
    {
        // Godot project has been removed — textures are already in Assets/Resources/Textures/
        // and Assets/Resources/UI/ from the initial migration.
        // This processor now only ensures the directories exist.
        string target = Path.Combine(Application.dataPath, "Resources/Textures");
        Directory.CreateDirectory(target);
        string uiTarget = Path.Combine(Application.dataPath, "Resources/UI");
        Directory.CreateDirectory(uiTarget);
        AssetDatabase.Refresh();
    }
}
