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
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string repoRoot = Directory.GetParent(projectRoot).FullName;
        string target = Path.Combine(Application.dataPath, "Resources/Textures");
        Directory.CreateDirectory(target);
        string[] sources =
        {
            Path.Combine(repoRoot, "godot/assets/textures/uploaded"),
            Path.Combine(repoRoot, "godot/assets/textures")
        };
        foreach (string source in sources)
        {
            if (!Directory.Exists(source)) continue;
            foreach (string file in Directory.GetFiles(source))
            {
                string ext = Path.GetExtension(file).ToLowerInvariant();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png") continue;
                string destination = Path.Combine(target, Path.GetFileName(file));
                File.Copy(file, destination, true);
            }
        }
        AssetDatabase.Refresh();
    }
}
