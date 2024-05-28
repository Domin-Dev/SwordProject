using UnityEngine;
public abstract class BuildingItem : Item
{
    public int durability;
    public Texture2D texture;
}


public abstract class VariantItem : BuildingItem
{
    public ObjectVariant[] objectVariants;
}



[System.Serializable]
public class ObjectVariant
{
    public Vector2[] hitbox;
    public float minY;
    public Sprite[] sprites;

    public ObjectVariant(Vector2[] hitbox, Sprite[] sprites, float minY)
    {
        this.hitbox = hitbox;
        this.sprites = sprites;
        this.minY = minY;
    }

    public ObjectVariant Clone()
    {
        return new ObjectVariant(hitbox, sprites, minY);
    }
}