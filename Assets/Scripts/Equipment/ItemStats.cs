
using UnityEngine;
using System;


public class ItemStats
{
    public int itemID = -1;
    public int itemCount;
    public ItemLife itemLife;

    public ItemStats(ItemStats itemStats)
    {
        this.itemID = itemStats.itemID;
        this.itemCount = itemStats.itemCount;
        if(itemStats.itemLife != null) this.itemLife = new ItemLife(itemStats.itemLife);
    }

    public ItemStats(int itemID, int itemCount,ItemLife itemLife)
    {
        this.itemID = itemID;
        this.itemCount = itemCount;
        this.itemLife = itemLife; 
    }
    public ItemStats(int itemID, int itemCount)
    {
        this.itemID = itemID;
        this.itemCount = itemCount;
        this.itemLife = null;
    }

    public ItemStats(int itemID)
    {
        this.itemID = itemID;
        this.itemCount = 1;
        this.itemLife = null;

    }

    public bool isNull()
    {
        if (itemID != -1) return false;
        else return true;
    }
}

[Serializable]
public class ItemLife
{
    public float maxLifePoints;
    public float currentLifePoints;

    public ItemLife(float maxLifePoints,float currentLifePoints)
    {
        this.maxLifePoints = maxLifePoints;
        this.currentLifePoints = currentLifePoints;
    }

    public ItemLife(ItemLife itemLife)
    {
        this.maxLifePoints = itemLife.maxLifePoints;
        this.currentLifePoints = itemLife.currentLifePoints;
    }
}

