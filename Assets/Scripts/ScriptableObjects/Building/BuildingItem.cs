using UnityEngine;
public abstract class BuildingItem : Item
{
    public int durability;
    public Texture2D texture;
}
public abstract class VariantItem : BuildingItem
{
    public GameObject HitParticles;
    public ObjectVariant[] objectVariants;
}

[System.Serializable]
public class ObjectVariant
{
    public Variant[] variants;
    public ObjectVariant(Variant[] variants)
    { 
        this.variants = variants;
    }
    public ObjectVariant Clone()
    {
        return new ObjectVariant(variants);
    }
}

[System.Serializable]
public class Variant
{
    public Vector2[] hitbox;
    public Vector2 particlePoint;
    public float minY;
    public Sprite sprite;

    public Variant(Vector2[] hitbox, Sprite sprite, float minY, Vector2 particlePoint)
    {
        this.particlePoint = particlePoint;
        this.hitbox = hitbox;
        this.sprite = sprite;
        this.minY = minY;
    }

    public Variant Clone()
    {
        return new Variant(hitbox, sprite, minY,particlePoint);
    }
}