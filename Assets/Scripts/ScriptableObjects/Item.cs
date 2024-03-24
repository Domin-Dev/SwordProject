using System.Xml.Schema;
using UnityEngine;

public enum ItemType
{
    Weapon,

}

[CreateAssetMenu(fileName = "Item", menuName = "GameAsset/Item")]
public class Item : ScriptableObject
{
    public string name;
    public string description;
    public int ID { set; get; }

    public Sprite icon; 
}
