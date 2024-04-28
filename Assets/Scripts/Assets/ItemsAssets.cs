
using UnityEngine;
using System.Collections.Generic;



public class ItemsAsset : MonoBehaviour
{
    private struct AmmoInfo
    {
       public AmmoType type;
       public int id;

        public AmmoInfo(int id,AmmoType ammoType)
        {
            this.id = id;
            this.type = ammoType;
        }
    }
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
    private List<AmmoInfo> ammoList;

    public void Awake()
    {
        Item[] loadedItems = Resources.LoadAll<Item>("Items");
        ammoList = new List<AmmoInfo>();
        for (int i = 0; i < loadedItems.Length; i++)
        {
            Item item = loadedItems[i];
            items.Add(item.ID, item);
            if(item as Ammo != null) 
            { 
                ammoList.Add(new AmmoInfo(item.ID,(item as Ammo).type));
            }
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

    public Sprite GetAmmoSpriteUI(AmmoType type)
    {
        for(int i = 0;i<ammoList.Count;i++)
        {
            if (ammoList[i].type == type)
            {
                return (GetItem(ammoList[i].id) as Ammo).UIBulletIcon;
            }
        }
        return null;
    }
    public Sprite GetAmmoHandSprite(AmmoType type)
    {
        for (int i = 0; i < ammoList.Count; i++)
        {
            if (ammoList[i].type == type)
            {
                return (GetItem(ammoList[i].id) as Ammo).inHandSprite;
            }
        }
        return null;
    }

}
