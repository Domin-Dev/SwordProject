
using UnityEngine;
using System;


public class ItemStats
{
    public int ItemID = -1;
    public int ItemCount;
    public ItemLife itemLife;


    public ItemStats(int itemID, int itemCount,ItemLife itemLife)
    {
        this.ItemID = itemID;
        this.ItemCount = itemCount;
        this.itemLife = itemLife; 
    }
    public ItemStats(int itemID, int itemCount)
    {
        this.ItemID = itemID;
        this.ItemCount = itemCount;
        this.itemLife = null;
    }

    public ItemStats(int itemID)
    {
        this.ItemID = itemID;
        this.ItemCount = 1;
        this.itemLife = null;

    }

    public bool isNull()
    {
        if (ItemID != -1) return false;
        else return true;
    }
}

[Serializable]
public class ItemLife
{
    private float maxLifePoints;
    public float currentLifePoints;

    public ItemLife(float maxLifePoints,float currentLifePoints)
    {
        this.maxLifePoints = maxLifePoints;
        this.currentLifePoints = currentLifePoints;
    }
}

