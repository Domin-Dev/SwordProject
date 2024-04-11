
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "Weapon", menuName = "GameAsset/Items/Weapons/Weapon")]
public class Weapon: Item
{
    [Header("Weapon Stats")]
    public Sprite weaponImage;

    public Vector2[] hitBoxPoints;
    public Vector2 gripPoint;

    public int damage = 5;
    public int cooldown;

}
