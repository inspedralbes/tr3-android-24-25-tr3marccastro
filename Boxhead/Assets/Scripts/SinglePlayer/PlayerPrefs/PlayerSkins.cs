using UnityEngine;
using System.Collections.Generic;

public static class PlayerSkins
{
    private const string OwnedSkinKey = "OwnedSkins";

    public static void AddSkin(int skinId, string nameImage, string assetBundlePath)
    {
        List<SkinsData> ownedSkins = GetOwnedSkins();
        
        if (!ownedSkins.Exists(skin => skin.id == skinId))
        {
            ownedSkins.Add(new SkinsData { id = skinId, name = nameImage, assetBundlePath = assetBundlePath });
            SaveOwnedSkins(ownedSkins);
        }
    }

    public static bool HasSkin(int skinId)
    {
        List<SkinsData> ownedSkins = GetOwnedSkins();
        return ownedSkins.Exists(skin => skin.id == skinId);
    }

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

    private static void SaveOwnedSkins(List<SkinsData> skins)
    {
        SkinList skinsList = new SkinList() { skins = skins };
        string json = JsonUtility.ToJson(skinsList);
        PlayerPrefs.SetString(OwnedSkinKey, json);
        PlayerPrefs.Save();
    }

    public static void DeleteAll()
    {
        PlayerPrefs.DeleteKey(OwnedSkinKey);
    }

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