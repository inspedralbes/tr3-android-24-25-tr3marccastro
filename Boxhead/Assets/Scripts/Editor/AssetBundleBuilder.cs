using UnityEditor;
using UnityEngine;

public class AssetBundleBuilder
{
    // Crear un ítem en el menú de Unity Editor para ejecutar la construcción de los AssetBundles
    [MenuItem("Assets/Build Asset Bundles")]
    static void BuildAllAssetBundles()
    {
        // Directorio donde se guardarán los AssetBundles generados
        string bundleDirectory = Application.dataPath + "/../AssetBundles"; // Ruta fuera de la carpeta Assets

        // Verificar si la carpeta de AssetBundles existe. Si no, crearla.
        if (!System.IO.Directory.Exists(bundleDirectory))
        {
            System.IO.Directory.CreateDirectory(bundleDirectory);
        }

        try
        {
            // Construir los AssetBundles para la plataforma activa
            BuildPipeline.BuildAssetBundles(bundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            Debug.Log("Asset Bundles creados en: " + bundleDirectory);  // Confirmación en la consola
        }
        catch (System.Exception e)
        {
            // Si ocurre un error durante la creación de los AssetBundles, lo manejamos
            Debug.LogWarning("Error al crear los Asset Bundles: " + e.Message);
        }
    }
}