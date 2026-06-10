using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public sealed class TextureBridgeBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => -1000;

    public void OnPreprocessBuild(BuildReport report)
    {
        // Textures are committed in Assets/Resources/Textures/.
        // No pre-build sync needed anymore.
    }
}
