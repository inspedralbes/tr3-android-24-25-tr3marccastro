using UnityEngine;
using System.Collections.Generic;

public static class PlayerSkin
{
    private const string OwnedItemsKey = "OwnedItems";

    public static void AddItem(int itemId)
    {
        List<int> ownedItems = GetOwnedItems();
        if (!ownedItems.Contains(itemId))
        {
            ownedItems.Add(itemId);
            SaveOwnedItems(ownedItems);
        }
    }

    public static bool HasItem(int itemId)
    {
        return GetOwnedItems().Contains(itemId);
    }

    public static List<int> GetOwnedItems()
    {
        string json = PlayerPrefs.GetString(OwnedItemsKey, "");
        if (string.IsNullOrEmpty(json))
        {
            return new List<int>();
        }
        IntList intList = JsonUtility.FromJson<IntList>(json);
        return intList != null ? intList.items : new List<int>();
    }

    private static void SaveOwnedItems(List<int> items)
    {
        IntList intList = new() { items = items };
        string json = JsonUtility.ToJson(intList);
        PlayerPrefs.SetString(OwnedItemsKey, json);
        PlayerPrefs.Save();
    }

    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    [System.Serializable]
    private class IntList
    {
        public List<int> items;
    }
}
