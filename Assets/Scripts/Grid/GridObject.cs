using System.Numerics;
using UnityEngine;

public interface IGetBarValue
{
    public float GetBarValue();
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
        this.indexVariant = indexVariant;
        this.objectTransform = obj;
    }
    public float GetBarValue()
    {
        return hitPoints / maxHitPoints;
    }
}




