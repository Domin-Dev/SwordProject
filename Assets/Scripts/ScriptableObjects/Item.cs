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
    public virtual ItemStats GetItemStats()
    {
        return new ItemStats(ID);
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

