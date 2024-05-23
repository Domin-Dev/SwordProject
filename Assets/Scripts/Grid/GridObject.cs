using System.Numerics;
using UnityEngine;

public interface IGetBarValue
{
    public float GetBarValue();
}


public class GridObject: IGetBarValue
{
    public int ID;
    public float hitPoints;
    private float maxHitPoints;
    public Transform objectTransform;

    public GridObject(int ID,Transform obj)
    {
        this.ID = ID;
        this.objectTransform = obj;
    }

    public float GetBarValue()
    {
        return hitPoints / maxHitPoints;
    }


}



