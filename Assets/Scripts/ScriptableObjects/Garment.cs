using UnityEngine;


[CreateAssetMenu(fileName = "Garment", menuName = "GameAsset/Items/Garment")]

public class Garment : Item
{
    [Header("Garment Stats")]
    public GarmentType type;
    public Texture2D texture;
    public Sprite[] sprites;
    
}

public enum GarmentType
{
    headwear,
    faceCover,
    outerwear,
    shirt,
    pants,
    belt,
    accessory,
    bag
}

