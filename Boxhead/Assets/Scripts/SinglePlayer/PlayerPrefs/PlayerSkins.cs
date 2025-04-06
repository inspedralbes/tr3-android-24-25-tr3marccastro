using UnityEngine;
using System.Collections.Generic;

public static class PlayerSkins
{
    // Clave utilizada para almacenar las pieles (skins) del jugador en PlayerPrefs
    private const string OwnedSkinKey = "OwnedSkins";

    // Método para agregar una nueva piel (skin) al jugador
    public static void AddSkin(int skinId, string nameImage, string assetBundlePath)
    {
        // Obtiene la lista de pieles que el jugador ya posee
        List<SkinsData> ownedSkins = GetOwnedSkins();

        // Verifica si la piel ya existe en la lista, si no existe, la agrega
        if (!ownedSkins.Exists(skin => skin.id == skinId))
        {
            ownedSkins.Add(new SkinsData { id = skinId, name = nameImage, assetBundlePath = assetBundlePath });
            // Guarda la lista de pieles actualizada
            SaveOwnedSkins(ownedSkins);
        }
    }

    // Método para verificar si el jugador tiene una piel específica
    public static bool HasSkin(int skinId)
    {
        // Obtiene la lista de pieles que el jugador posee
        List<SkinsData> ownedSkins = GetOwnedSkins();
        // Verifica si la piel con el ID especificado existe en la lista
        return ownedSkins.Exists(skin => skin.id == skinId);
    }

    // Método para obtener todas las pieles que el jugador posee
    public static List<SkinsData> GetOwnedSkins()
    {
        // Obtiene la cadena JSON que representa las pieles del jugador
        string json = PlayerPrefs.GetString(OwnedSkinKey, "");
        // Si no hay datos de pieles guardados, devuelve una lista vacía
        if (string.IsNullOrEmpty(json))
        {
            return new List<SkinsData>();
        }
        // Convierte la cadena JSON en un objeto SkinList que contiene una lista de SkinsData
        SkinList skinsList = JsonUtility.FromJson<SkinList>(json);
        // Si la conversión fue exitosa, devuelve la lista de pieles, de lo contrario, una lista vacía
        return skinsList != null ? skinsList.skins : new List<SkinsData>();
    }

    // Método privado para guardar la lista de pieles del jugador
    private static void SaveOwnedSkins(List<SkinsData> skins)
    {
        // Crea un objeto SkinList que contiene la lista de pieles
        SkinList skinsList = new SkinList() { skins = skins };
        // Convierte el objeto SkinList a una cadena JSON
        string json = JsonUtility.ToJson(skinsList);
        // Guarda la cadena JSON en PlayerPrefs bajo la clave "OwnedSkins"
        PlayerPrefs.SetString(OwnedSkinKey, json);
        PlayerPrefs.Save(); // Guarda los cambios en PlayerPrefs
    }

    // Método para eliminar todas las pieles guardadas
    public static void DeleteAll()
    {
        // Elimina la clave "OwnedSkins" de PlayerPrefs, borrando todas las pieles
        PlayerPrefs.DeleteKey(OwnedSkinKey);
    }

    // Clase que contiene los datos de una piel (skin)
    [System.Serializable]
    public class SkinsData
    {
        public int id; // ID de la piel
        public string name; // Nombre de la imagen de la piel
        public string assetBundlePath; // Ruta del AssetBundle de la piel
    }

    // Clase interna utilizada para convertir la lista de pieles en un objeto serializable
    [System.Serializable]
    private class SkinList
    {
        public List<SkinsData> skins; // Lista de pieles
    }
}