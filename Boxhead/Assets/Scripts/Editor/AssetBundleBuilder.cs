using UnityEditor;
using UnityEngine;

public class AssetBundleBuilder
{
    [MenuItem("Assets/Build Asset Bundles")]
    static void BuildAllAssetBundles()
    {
        string bundleDirectory = Application.dataPath + "/../AssetBundles";

        if (!System.IO.Directory.Exists(bundleDirectory))
        {
            System.IO.Directory.CreateDirectory(bundleDirectory);
        }

        try
        {
            BuildPipeline.BuildAssetBundles(bundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            Debug.Log("Asset Bundles creados en: " + bundleDirectory);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Error al crear los Asset Bundles: " + e.Message);
        }
    }
}