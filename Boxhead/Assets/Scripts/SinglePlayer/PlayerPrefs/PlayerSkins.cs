using UnityEngine;
using System.Collections.Generic;

public static class PlayerSkins
{
    private const string OwnedItemsKey = "OwnedItems";

    // Añadir un ítem con su ID y nombre de la imagen
    public static void AddItem(int itemId, string nameImage, string assetBundlePath)
    {
        List<ItemData> ownedItems = GetOwnedItems();
        
        // Verificar si el ítem ya está en la lista
        if (!ownedItems.Exists(item => item.id == itemId))
        {
            ownedItems.Add(new ItemData { id = itemId, name = nameImage, assetBundlePath = assetBundlePath });
            SaveOwnedItems(ownedItems);
        }
    }

    // Verificar si el jugador tiene un ítem con el ID
    public static bool HasItem(int itemId)
    {
        List<ItemData> ownedItems = GetOwnedItems();
        return ownedItems.Exists(item => item.id == itemId);
    }

    // Obtener los ítems poseídos (con ID y nombre de imagen)
    public static List<ItemData> GetOwnedItems()
    {
        string json = PlayerPrefs.GetString(OwnedItemsKey, "");
        if (string.IsNullOrEmpty(json))
        {
            return new List<ItemData>();
        }
        ItemList itemList = JsonUtility.FromJson<ItemList>(json);
        return itemList != null ? itemList.items : new List<ItemData>();
    }

    // Guardar los ítems poseídos
    private static void SaveOwnedItems(List<ItemData> items)
    {
        ItemList itemList = new ItemList() { items = items };
        string json = JsonUtility.ToJson(itemList);
        PlayerPrefs.SetString(OwnedItemsKey, json);
        PlayerPrefs.Save();
    }

    // Eliminar todos los ítems
    public static void DeleteAll()
    {
        PlayerPrefs.DeleteKey(OwnedItemsKey); // Eliminar solo los ítems, no todos los PlayerPrefs
    }

    // Clase para representar el ítem (ID y nombre de la imagen) - Ahora es pública
    [System.Serializable]
    public class ItemData
    {
        public int id;
        public string name;
        public string assetBundlePath;
    }

    // Clase auxiliar para deserializar la lista de ítems
    [System.Serializable]
    private class ItemList
    {
        public List<ItemData> items;
    }
}