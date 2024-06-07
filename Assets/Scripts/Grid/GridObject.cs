using System.Numerics;
using UnityEngine;

public interface IGetBarValue
{
    public float GetBarValue();
    public void IncreaseHitPoints(float value);
    public void DecreaseHitPoints(float value);
}
public class GridDoor : GridObject
{
    public bool doorIsClosed;
    public GridDoor(int ID, int indexVariant , Transform obj,bool doorIsClosed = true) : base(ID,indexVariant, obj)
    {
        this.doorIsClosed = doorIsClosed;
    }
}
public class GridObject: IGetBarValue
{
    public int ID;
    public int indexVariant;
    public Transform objectTransform;

    public float hitPoints;
    private float maxHitPoints;

    public GridObject(int ID,int indexVariant,Transform obj)
    {
        this.ID = ID;
        this.maxHitPoints = (ItemsAsset.instance.GetItem(ID) as BuildingItem).durability;
        this.hitPoints = maxHitPoints;
        this.indexVariant = indexVariant;
        this.objectTransform = obj;
    }
    public float GetBarValue()
    {
        return hitPoints / maxHitPoints;
    }
    public void DecreaseHitPoints(float value)
    {
        hitPoints = Mathf.Clamp(hitPoints - value, 0, maxHitPoints);
    }
    public void IncreaseHitPoints(float value)
    {
        hitPoints = Mathf.Clamp(hitPoints + value, 0, maxHitPoints);
    }
}




