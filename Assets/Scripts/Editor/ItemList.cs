using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;

[InitializeOnLoad]

public class ItemList : MonoBehaviour
{
    public static Dictionary<int,Item> items { private set; get; }

    public static string GetItemName(int ID)
    {
        if(items.ContainsKey(ID))
        {
            return items[ID].name;
        }
        else
        {
            return null;
        }
    }
    static ItemList()
    {
        Item[] loadedItems = Resources.LoadAll<Item>("Items");
        items = new Dictionary<int,Item>();
        for (int i = 0; i < loadedItems.Length; i++)
        {
            Item item = loadedItems[i];
            items.Add(item.ID, item);
        }
        Debug.Log(items.Count);
    }
}
