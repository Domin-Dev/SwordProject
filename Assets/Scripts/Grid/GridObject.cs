using System.Numerics;
using UnityEngine;

public class GridObject
{
    public int ID;
    public Transform objectTransform;

    public GridObject(int ID,Transform obj)
    {
        this.ID = ID;
        this.objectTransform = obj;
    }
}


public class ObjectPlan : GridObject
{
    float completionPercentage;
    public ObjectPlan(int ID,Transform obj):base(ID,obj)
    {
        completionPercentage = 0;
    }




}
