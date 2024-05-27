using UnityEngine;
public abstract class BuildingItem : Item
{
    public int durability;
    public Texture2D texture;
}


public abstract class VariantItem : BuildingItem
{
    public ObjectVariant[] objectVariants;
//#if UNITY_EDITOR
//    public ObjectVariant[] _objectVariants { set
//        {
//            objectVariants = value;
//        }
//    }

//#endif

}


[System.Serializable]
public class ObjectVariant
{
    public Vector2[] hitbox;
    public Sprite sprite;

    public ObjectVariant(Vector2[] hitbox, Sprite sprite)
    {
        this.hitbox = hitbox;
        this.sprite = sprite;
    }
}
