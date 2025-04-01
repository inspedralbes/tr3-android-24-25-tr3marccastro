using UnityEngine;
using System.Collections.Generic;

public static class PlayerSkins
{
    private const string OwnedSkinKey = "OwnedSkins";

    // Añadir un ítem con su ID y nombre de la imagen
    public static void AddSkin(int skinId, string nameImage, string assetBundlePath)
    {
        List<SkinsData> ownedSkins = GetOwnedSkins();
        
        // Verificar si el ítem ya está en la lista
        if (!ownedSkins.Exists(skin => skin.id == skinId))
        {
            ownedSkins.Add(new SkinsData { id = skinId, name = nameImage, assetBundlePath = assetBundlePath });
            SaveOwnedSkins(ownedSkins);
        }
    }

    // Verificar si el jugador tiene un ítem con el ID
    public static bool HasSkin(int skinId)
    {
        List<SkinsData> ownedSkins = GetOwnedSkins();
        return ownedSkins.Exists(skin => skin.id == skinId);
    }

    // Obtener los ítems poseídos (con ID y nombre de imagen)
    public static List<SkinsData> GetOwnedSkins()
    {
        string json = PlayerPrefs.GetString(OwnedSkinKey, "");
        if (string.IsNullOrEmpty(json))
        {
            return new List<SkinsData>();
        }
        SkinList skinsList = JsonUtility.FromJson<SkinList>(json);
        return skinsList != null ? skinsList.skins : new List<SkinsData>();
    }

    // Guardar los ítems poseídos
    private static void SaveOwnedSkins(List<SkinsData> skins)
    {
        SkinList skinsList = new SkinList() { skins = skins };
        string json = JsonUtility.ToJson(skinsList);
        PlayerPrefs.SetString(OwnedSkinKey, json);
        PlayerPrefs.Save();
    }

    // Eliminar todos los ítems
    public static void DeleteAll()
    {
        PlayerPrefs.DeleteKey(OwnedSkinKey); // Eliminar solo los ítems, no todos los PlayerPrefs
    }

    // Clase para representar el ítem (ID y nombre de la imagen) - Ahora es pública
    [System.Serializable]
    public class SkinsData
    {
        public int id;
        public string name;
        public string assetBundlePath;
    }

    // Clase auxiliar para deserializar la lista de ítems
    [System.Serializable]
    private class SkinList
    {
        public List<SkinsData> skins;
    }
}