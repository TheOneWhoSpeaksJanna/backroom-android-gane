#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public sealed class DesolationAndroidBrandingPrebuild : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        PlayerSettings.companyName = "Desolation";
        PlayerSettings.productName = "Desolation: The Backrooms";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.desolation.thebackrooms");
        AssetDatabase.SaveAssets();
    }
}
#endif
