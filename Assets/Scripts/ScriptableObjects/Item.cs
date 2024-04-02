
using UnityEngine;
public abstract class Item : ScriptableObject
{
    [Header("Item Stats")]
    public string name;
    public string description;
    public int ID;

    [Header("Item graphic")]
    public Sprite icon;
}
