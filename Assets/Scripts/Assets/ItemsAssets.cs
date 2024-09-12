using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

public class ItemsAsset : MonoBehaviour
{
    private struct AmmoInfo
    {
        public AmmoType type;
        public int id;

        public AmmoInfo(int id, AmmoType ammoType)
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
                i = new GameObject("ItemsAsset", typeof(ItemsAsset)).GetComponent<ItemsAsset>();
            }
            return i;
        }
    }

    private Dictionary<int, Item> items = new Dictionary<int, Item>();
    private List<AmmoInfo> ammoList;

    private Dictionary<int, Item[]> itemRecipes;
    public void Awake()
    {
        LoadItems();
    }
    public T[] GetItemsByType<T>() where T : Item
    {
        List<T> itemList = new List<T>();
        foreach (var item in items)
        {
            if (item.Value is T)
            {
                itemList.Add(item.Value as T);
            }
        }
        return itemList.ToArray();
    }
    public Sprite GetBuildingObjectSprite(int id, int index)
    {
        if (items.ContainsKey(id))
        {
            VariantItem item = items[id] as VariantItem;
            if (item != null && item.objectVariants.Length > index)
            {
                return item.objectVariants[index].variants[0].sprite;
            }
        }
        return null;
    }
    public ObjectVariant GetObjectVariant(int id, int index)
    {
        if (items.ContainsKey(id))
        {
            VariantItem item = items[id] as VariantItem;
            if (item != null && item.objectVariants.Length > index)
            {
                return item.objectVariants[index].Clone();
            }
        }
        return null;
    }
    public Vector2[] GetBuildingObjectHitbox(int id, int index)
    {
        if (items.ContainsKey(id))
        {
            VariantItem item = items[id] as VariantItem;
            if (item != null && item.objectVariants.Length > index)
            {
                // return item.objectVariants[index].hitbox;
            }
        }
        return null;
    }
    private void LoadItems()
    {
        Item[] loadedItems = Resources.LoadAll<Item>("Items");
        Dictionary<int, List<Item>> recipes = new Dictionary<int, List<Item>>();

        ammoList = new List<AmmoInfo>();
        for (int i = 0; i < loadedItems.Length; i++)
        {
            Item item = loadedItems[i];
            items.Add(item.ID, item);
            if (item as Ammo != null) ammoList.Add(new AmmoInfo(item.ID, (item as Ammo).type));
            if (item.crafingIngredients.Length > 0)
            {
                foreach (int j in item.craftTables)
                {
                    if (!recipes.ContainsKey(j)) recipes.Add(j, new List<Item>());
                    recipes[j].Add(item);
                }
            }
        }

        itemRecipes = new Dictionary<int, Item[]>();
        foreach (var item in recipes)
        {
            itemRecipes.Add(item.Key, item.Value.ToArray());
        }
    }
    public Sprite GetIcon(int itemID)
    {
        return items[itemID].icon;
    }
    public int GetStackMax(int itemID)
    {
        if (items.ContainsKey(itemID)) return items[itemID].stackMax;
        else return 0;
    }
    public bool IsItem(int itemId)
    {
        return items.ContainsKey(itemId);
    }
    public Item GetItem(int itemID)
    {
        return items[itemID];
    }
    public TooltipInfo GetTooltipInfo(int itemID)
    {
        return GetTooltipInfo(GetItem(itemID));
    }
    public TooltipInfo GetTooltipInfo(Item item)
    {
        return new TooltipInfo(item.description, item.name);
    }
    public ItemStats GetItemStats(int itemID, int itemCount = 1)
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
        for (int i = 0; i < ammoList.Count; i++)
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
    public ReadOnlyCollection<Item> GetItems()
    {
        Item[] itemArray = new Item[items.Count];
        int i = 0;
        foreach (var item in items)
        {
            itemArray[i] = item.Value;
            i++;
        }
        return new ReadOnlyCollection<Item>(itemArray);
    }
    public ReadOnlyCollection<Item> GetRecipesCrafTable(int tableID)
    {
        if (itemRecipes.ContainsKey(tableID))
        {
            return new ReadOnlyCollection<Item>(itemRecipes[tableID]);
        } 
        else
        {
            return null;
        }
    }
}
