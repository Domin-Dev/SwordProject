
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using static UnityEditor.Progress;

public class ItemsAsset : MonoBehaviour
{
    private static ItemsAsset i;
    public static ItemsAsset instance 
    {
        get
        {
            if (i == null)
            {
                i = new GameObject("ItemsAsset",typeof(ItemsAsset)).GetComponent<ItemsAsset>();
            }
            return i;
        }
    }

    private Dictionary<int, Item> items = new Dictionary<int, Item>();
    public void Awake()
    {
        Item[] loadedItems = Resources.LoadAll<Item>("Items");
        for (int i = 0; i < loadedItems.Length; i++)
        {
            Item item = loadedItems[i];
            items.Add(item.ID, item);
        }
    }
    public Sprite GetIcon(int itemID)
    {
        return items[itemID].icon;
    }
    public int GetStackMax(int itemID)
    {
         if(items.ContainsKey(itemID)) return items[itemID].stackMax;
         else return 0;
    }
    public bool IsItem(int itemId)
    {
       return items.ContainsKey(itemId);
    }
    public Sprite[] GetGarmentSprites(int itemID)
    {
        return (items[itemID] as Garment).GetArray();
    }
    public Item GetItem(int itemID)
    {
        return items[itemID];
    }
    public TooltipInfo GetTooltipInfo(int itemID)
    {
        Item item = GetItem(itemID);
        return new TooltipInfo(item.description, item.name);
    }
    public ItemStats GetItemStats(int itemID,int itemCount = 1)
    {
        ItemStats item = GetItem(itemID).GetItemStats();
        item.itemCount = itemCount;
        return item;
    }

}
