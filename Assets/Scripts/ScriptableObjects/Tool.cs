using UnityEngine;

public enum ToolType
{
    None,
    Hammer,
    Axe,
    Pickaxe,
    Hoe,
    Shovel,
}
[CreateAssetMenu(fileName = "Weapon", menuName = "GameAsset/Items/Tool")]
public class Tool : Weapon
{
    public ToolType toolType;
    public int efficiency;
}
