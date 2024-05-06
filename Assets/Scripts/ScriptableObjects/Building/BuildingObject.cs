using UnityEngine;

[CreateAssetMenu(fileName = "BuildingObject", menuName = "GameAsset/BuildingObjects/Object")]
public class BuildingObject : ScriptableObject
{
    public Sprite icon;
    public string name;
    [Multiline()] public string description;
    public Texture2D spriteObject;
    public int ID;
    public int durability;
}
