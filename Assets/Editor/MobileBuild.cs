#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class MobileBuild
{
    private const string AppIdentifier = "com.gbhide.balloonblast";
    private const string AndroidBuildPath = "Builds/Android/BalloonBlast.apk";
    private const string IosBuildPath = "Builds/iOS";

    [MenuItem("Balloon Blast/Mobile/Configure Player Settings")]
    public static void ConfigurePlayerSettings()
    {
        ApplyMobilePlayerSettings();
        AssetDatabase.SaveAssets();
        Debug.Log("Balloon Blast mobile player settings configured for portrait iPhone and Android builds.");
    }

    [MenuItem("Balloon Blast/Mobile/Build Android APK")]
    public static void BuildAndroidApk()
    {
        ApplyMobilePlayerSettings();
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        EditorUserBuildSettings.buildAppBundle = false;

        Directory.CreateDirectory(Path.GetDirectoryName(AndroidBuildPath));

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = AndroidBuildPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        HandleBuildReport(report, AndroidBuildPath);
    }

    [MenuItem("Balloon Blast/Mobile/Build iOS Xcode Project")]
    public static void BuildIosXcodeProject()
    {
        ApplyMobilePlayerSettings();
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);

        Directory.CreateDirectory(IosBuildPath);

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = IosBuildPath,
            target = BuildTarget.iOS,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        HandleBuildReport(report, IosBuildPath);
    }

    public static void BuildAndroidApkCommandLine()
    {
        BuildAndroidApk();
    }

    public static void BuildIosXcodeProjectCommandLine()
    {
        BuildIosXcodeProject();
    }

    private static void ApplyMobilePlayerSettings()
    {
        PlayerSettings.companyName = "Gbhide";
        PlayerSettings.productName = "Balloon Blast";
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, AppIdentifier);
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, AppIdentifier);

        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;

        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        PlayerSettings.iOS.targetOSVersionString = "12.0";
    }

    private static string[] GetEnabledScenes()
    {
        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            throw new BuildFailedException("No enabled scenes found in Build Settings.");
        }

        return scenes;
    }

    private static void HandleBuildReport(BuildReport report, string buildPath)
    {
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new BuildFailedException("Build failed: " + report.summary.result);
        }

        Debug.Log("Build completed: " + buildPath);
        EditorUtility.RevealInFinder(buildPath);
    }
}
#endif
