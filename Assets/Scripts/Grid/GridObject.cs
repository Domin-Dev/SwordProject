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


public class ObjectPlan : GridObject,IGetBarValue
{
    int constructionPoints;
    int constructionPointsTocomplete;
    public ObjectPlan(int ID,int constructionPointsTocomplete, Transform obj):base(ID,obj)
    {
        constructionPoints = Random.Range(0,100);
        this.constructionPointsTocomplete = constructionPointsTocomplete;
    }

    public float GetBarValue()
    {
        return (float)constructionPoints/constructionPointsTocomplete;
    }

    public void Building(int value)
    {
        constructionPoints += value;
    }
}
