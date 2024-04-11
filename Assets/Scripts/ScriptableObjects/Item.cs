using UnityEngine;
[CreateAssetMenu(fileName = "Item", menuName = "GameAsset/Items/Item")]
public class Item : ScriptableObject
{
    [Header("Item Stats")]
    public string name;
    public string description;
    public int ID;
    public int stackMax = 50;

    [Header("Item graphic")]
    public Sprite icon;
}
