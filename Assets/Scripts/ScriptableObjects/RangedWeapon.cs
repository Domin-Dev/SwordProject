
using UnityEngine;
[CreateAssetMenu(fileName = "RangedWeapon", menuName = "GameAsset/Items/Weapons/RangedWeapon")]
public class RangedWeapon: Weapon
{
    [Header("Weapon Stats")]
    public float timeToReload;
    public int magazineCapacity;
    public int ammoID;
    public Vector2 aimPoint;
}
