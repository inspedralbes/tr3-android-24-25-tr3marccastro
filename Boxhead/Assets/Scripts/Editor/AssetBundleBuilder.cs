using UnityEditor;
using UnityEngine;

// Clase que contiene la funcionalidad para construir Asset Bundles
public class AssetBundleBuilder
{
    // Esta l�nea agrega una opci�n al men� "Assets" en el Editor de Unity llamada "Build Asset Bundles"
    [MenuItem("Assets/Build Asset Bundles")]
    static void BuildAllAssetBundles()
    {
        // Define el directorio donde se guardar�n los Asset Bundles generados
        string bundleDirectory = Application.dataPath + "/../AssetBundles";

        // Si el directorio no existe, se crea
        if (!System.IO.Directory.Exists(bundleDirectory))
        {
            System.IO.Directory.CreateDirectory(bundleDirectory);
        }

        try
        {
            // Construye todos los Asset Bundles del proyecto
            // - bundleDirectory: ruta donde se guardar�n los archivos
            // - BuildAssetBundleOptions.None: sin opciones adicionales
            // - EditorUserBuildSettings.activeBuildTarget: usa la plataforma seleccionada en el editor (por ejemplo, Windows, Android, etc.)
            BuildPipeline.BuildAssetBundles(bundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

            // Muestra un mensaje de �xito en la consola
            Debug.Log("Asset Bundles creados en: " + bundleDirectory);
        }
        catch (System.Exception e)
        {
            // Si ocurre un error, se muestra una advertencia con el mensaje del error
            Debug.LogWarning("Error al crear los Asset Bundles: " + e.Message);
        }
    }
}