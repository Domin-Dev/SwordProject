using UnityEngine;
[CreateAssetMenu(fileName = "Item", menuName = "GameAsset/Items/Item")]
public class Item : ScriptableObject
{
    [Header("Item Stats")]
    public string name;
    [Multiline()]
    public string description;
    public int ID = -1;
    public int stackMax = 50;

    [Header("Item graphic")]
    public Sprite icon;

    [Header("Craft recipe")]
    public Ingredient[] crafingIngredients;
    public int[] craftTables;
    public int numberItem = 1;
    public virtual ItemStats GetItemStats()
    {
        return new ItemStats(ID);
    }

    private void OnValidate()
    {
       if(ID == -1) ID = Resources.Load<IDManager>("IDManager").GetNextID();
    }
}

[System.Serializable]
public class Ingredient
{
    public int itemID;
    public int number;

    public Ingredient(int itemID, int number)
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

