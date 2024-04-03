using UnityEngine;

public class ItemSlot
{
    public int ItemID;
    public int ItemCount;
    public ItemLife itemLife;
}

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

