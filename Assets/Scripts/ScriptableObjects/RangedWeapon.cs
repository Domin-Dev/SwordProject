
using UnityEngine;
[CreateAssetMenu(fileName = "RangedWeapon", menuName = "GameAsset/Items/Weapons/RangedWeapon")]
public class RangedWeapon: Weapon
{
    [Header("Weapon Stats")]
    public float timeToReload;
    public int magazineCapacity;
    public Vector2 aimPoint;
    public AmmoType ammoType;


    public override ItemStats GetItemStats()
    {
        Debug.Log(durability);
        return new GunMagazineItem(ID,durability, magazineCapacity);
    }
}
