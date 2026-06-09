using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

public sealed class TextureBridgeBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => -1000;

    [InitializeOnLoadMethod]
    static void Load()
    {
        EnsureDirs();
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        EnsureDirs();
    }

    static void EnsureDirs()
    {
        string target = Path.Combine(Application.dataPath, "Resources/Textures");
        Directory.CreateDirectory(target);
        AssetDatabase.Refresh();
    }
}
