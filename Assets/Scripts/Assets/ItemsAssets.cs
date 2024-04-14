
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

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
    public int x;
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

    public Sprite[] GetGarmentSprites(int itemID)
    {
        return (items[itemID] as Garment).GetArray();
    }
    
    public Item GetItem(int itemID)
    {
        return items[itemID];
    }
}
