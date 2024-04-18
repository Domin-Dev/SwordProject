using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ammo", menuName = "GameAsset/Items/Weapons/Ammo")]
public class Ammo : Item
{
    public GameObject bullet;
    public Sprite UIbulletIcon;
    public int damage;
}
