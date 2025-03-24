using UnityEditor;
using UnityEngine;

public class AssetBundleBuilder
{
    [MenuItem("Assets/Build Assets Bundles")]
    static void BuildAllAssetBundles()
    {
        string bundleDirectory = Application.dataPath + "/../AssetsBundles";
        if (!System.IO.Directory.Exists(bundleDirectory))
        {
            System.IO.Directory.CreateDirectory(bundleDirectory);
        }

        try
        {
            BuildPipeline.BuildAssetBundles(bundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
        }

        Debug.Log("Asset Bundles creados en: " + bundleDirectory);
    }
}