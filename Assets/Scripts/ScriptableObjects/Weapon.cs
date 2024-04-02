
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "Weapon", menuName = "GameAsset/Items/Weapon")]
public class Weapon : Item
{
    [Header("Weapon Stats")]
    public int damage = 5;
    public int cooldown;
}
