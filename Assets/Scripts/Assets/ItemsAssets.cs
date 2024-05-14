using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
    private Dictionary<int, BuildingObject> buildingObjects = new Dictionary<int, BuildingObject>();

    public void Awake()
    {
        LoadItems();
        LoadBuildingObjects();
    }

   
    private void LoadBuildingObjects()
    {
        BuildingObject[] loadedObjects = Resources.LoadAll<BuildingObject>("BuildingObjects");
        for (int i = 0; i < loadedObjects.Length; i++)
        {
            BuildingObject buildingObject = loadedObjects[i];
            buildingObjects.Add(buildingObject.ID,buildingObject);
        }
    }    

    public BuildingObject GetBuildingObject(int ID)
    {
        return buildingObjects[ID];
    }

    public Sprite[] GetBuildingObjectSprites(int ID)
    {
        if (buildingObjects[ID] as Wall != null)
            return (buildingObjects[ID] as Wall).sprites;
        else 
            return new Sprite[0];
    }
    public Sprite GetBuildingObjectSprite(int ID,int spriteIndex)
    {
        Sprite[] sprites = GetBuildingObjectSprites(ID);
        if(sprites.Length > spriteIndex) return sprites[spriteIndex];
        else return null;
    }
    private void LoadItems()
    {
        Item[] loadedItems = Resources.LoadAll<Item>("Items");
        ammoList = new List<AmmoInfo>();
        for (int i = 0; i < loadedItems.Length; i++)
        {
            Item item = loadedItems[i];
            items.Add(item.ID, item);
            if (item as Ammo != null)
            {
                ammoList.Add(new AmmoInfo(item.ID, (item as Ammo).type));
            }
        }
    }
    public BuildingObject[] GetBuildingObjects()
    {
        return buildingObjects.Values.ToArray();
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

    public ToolType GetToolType(int ID)
    {
        Tool item = GetItem(ID) as Tool;
        if (item != null) return item.toolType;
        else return ToolType.None;
    }

    public int GetAmmoID(int weaponID)
    {
        AmmoType type = (GetItem(weaponID) as RangedWeapon).ammoType;
        for (int i = 0; i < ammoList.Count; i++)
        {
            if (ammoList[i].type == type)
            {
                return ammoList[i].id;
            }
        }
        return -1;
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
