using UnityEngine;
public abstract class BuildingObject : Item
{
    public int durability;
    public virtual ItemStats GetItemStats()
    {
        return new ItemStats(ID);
    }
}

