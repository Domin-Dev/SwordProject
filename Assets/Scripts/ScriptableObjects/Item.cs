using UnityEngine;
[CreateAssetMenu(fileName = "Item", menuName = "GameAsset/Items/Item")]
public class Item : ScriptableObject
{
    [Header("Item Stats")]
    public string name;
    [Multiline()]
    public string description;
    public int ID;
    public int stackMax = 50;

    [Header("Item graphic")]
    public Sprite icon;

    [Header("Craft recipe")]
    public CrafingIngredient[] crafingIngredients;
    public virtual ItemStats GetItemStats()
    {
        return new ItemStats(ID);
    }
}

[System.Serializable]
public class CrafingIngredient
{
    public int itemID;
    public int number;

    public CrafingIngredient(int itemID, int number)
    {
        this.itemID = itemID;
        this.number = number;
    }
}


[CreateAssetMenu(fileName = "DestroyableItem", menuName = "GameAsset/Items/DestroyableItem")]
public class Destroyable: Item
{
    [Header("Destroyable")]
    public int durability;
    public override ItemStats GetItemStats()
    {
        return new DestroyableItem(ID,durability);
    }
}

