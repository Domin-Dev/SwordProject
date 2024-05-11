using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingObject", menuName = "GameAsset/BuildingObjects/Object")]
public class BuildingObject : ScriptableObject
{
    public Sprite icon;
    public string name;
    [Multiline()] public string description;
    public Texture2D texture;
    public Sprite[] sprites;
    public int ID;
    public int durability;
}
