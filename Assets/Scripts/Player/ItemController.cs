using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{   
    [SerializeField] private Transform backpack;
    [SerializeField] private Transform secondHand;
    [SerializeField] private Transform secondHandParent;
    [SerializeField] private SpriteRenderer itemInHand;
    [SerializeField] private PolygonCollider2D hitBox;
    [SerializeField] private Transform aimPoint;
    [SerializeField] private Transform reloadPoint;
    private Vector3 secondHandStartPosition;


    public ItemStats selectedItem;
    public event EventHandler UseItem;
    public event EventHandler<SetAmmoBarArgs> SetAmmoBar;
    public event EventHandler<UpdateAmmoBarArgs> UpdateAmmoBar;
    public event EventHandler HideAmmoBar;


    HandsController attackModule;
    private void Awake()
    {
        secondHandStartPosition = secondHand.localPosition;
        attackModule = GetComponent<HandsController>();       
    }

    private void Update()
    {
        UpdateReload();
    }
    private void Start()
    {
        SetUpEvents();
        UIManager.instance.SetUpUIPlayer(this);
        EquipmentManager.instance.SetUpEvent(this);
    }
    private void SetUpEvents()
    {
        EquipmentManager.instance.UpdateItemInHand += UpdateItemInHand;
        Debug.Log("now");
    }
    private void UpdateItemInHand(object sender, ItemStatsArgs e)
    {
        itemInHand.transform.localPosition = Vector3.zero;

        if (e.item != null)
        {
            Item item = ItemsAsset.instance.GetItem(e.item.itemID);
            selectedItem = e.item;
            if (item as Weapon != null)
                ItemIsWepon(item, e.item);
            else
                ItemIsNotWepon(item);
        }
        else
            ItemIsNull();
    }
    private void ItemIsNull()
    {
        selectedItem = null;
        itemInHand.sprite = null;
        attackModule.isGun = false;
        SecondHandReset();
        HideAmmoBar(this, null);
    }
    private void ItemIsNotWepon(Item item)
    {
        itemInHand.sprite = item.icon;
        itemInHand.sortingOrder = 20;
        SecondHandReset();
        HideAmmoBar(this, null);
    }
    private void ItemIsWepon(Item item, ItemStats itemStats)
    {
        Weapon weapon = (Weapon)item;
        itemInHand.sprite = weapon.weaponImage;
        hitBox.points = weapon.hitBoxPoints;
        hitBox.transform.localPosition = -weapon.gripPoint1;
        itemInHand.sortingOrder = 10;
        itemInHand.transform.localPosition = -weapon.gripPoint1;
        if (weapon.gripPoint2.x != -100)
        {
            secondHand.parent = itemInHand.transform;
            secondHand.localEulerAngles = Vector3.zero;
            secondHand.localPosition = weapon.gripPoint2;
        }
        else
        {
            SecondHandReset();
        }

        if (weapon as RangedWeapon != null)
        {
            attackModule.isGun = true;
            RangedWeapon rangedWeapon = (RangedWeapon)weapon;
            float posY = Mathf.Abs(rangedWeapon.aimPoint.y - rangedWeapon.gripPoint1.y);
            attackModule.SetTransformHand(posY);
            aimPoint.localPosition = (Vector2)itemInHand.transform.localPosition + rangedWeapon.aimPoint;
            reloadPoint.localPosition = (Vector2)itemInHand.transform.localPosition + rangedWeapon.reloadPoint;
            SetAmmoBar(this, new SetAmmoBarArgs(rangedWeapon.magazineCapacity, (itemStats as RangedWeaponItem).currentAmmoCount, rangedWeapon.ammoType));
       
        }
        else
        {
            attackModule.isGun = false;
            HideAmmoBar(this, null);
        }
    }
    private void SecondHandReset()
    {
        if (secondHand.parent != secondHandParent)
        {
            secondHand.parent = secondHandParent;
            secondHand.localPosition = secondHandStartPosition;
            secondHand.localEulerAngles = Vector3.zero;
        }
    }
    public void Use()
    {
        UseItem(this, null);
    }
    public void Shot()
    {
        (selectedItem as RangedWeaponItem).Shot();
        UpdateAmmoBar(this, new UpdateAmmoBarArgs((selectedItem as RangedWeaponItem).currentAmmoCount));
    }
    public void Reload()
    {
        ReloadAnim();
    }

    private Vector2? target = null;
    private Vector2 startPosition;
    private Transform startParent;
    private void ReloadAnim()
    {
        startPosition = secondHand.localPosition;
        startParent = secondHand.parent;

        secondHand.parent = transform;
        target = backpack.transform.position;
        Instantiate(ParticleAssets.instance.gunShells,itemInHand.transform.position,Quaternion.identity);
    }


    private bool back = false;
    private void UpdateReload()
    {
        if (target != null)
        {
            if (!back)
            {
                secondHand.position = Vector2.Lerp(secondHand.position, backpack.position, Time.deltaTime * 5f);
                if (Vector2.Distance(secondHand.position, backpack.position) < 0.01f)
                {
                    back = true;
                    Sounds.instance.Ammo();
                }
            }
            else
            {
                secondHand.position = Vector2.Lerp(secondHand.position, reloadPoint.position, Time.deltaTime * 5f);
                if (Vector2.Distance(secondHand.position, reloadPoint.position) < 0.01f)
                {
                    back = false;
                    (selectedItem as RangedWeaponItem).Reload(1);
                    Sounds.instance.Reload();
                    UpdateAmmoBar(this, new UpdateAmmoBarArgs((selectedItem as RangedWeaponItem).currentAmmoCount));
                    if (!(selectedItem as RangedWeaponItem).CanReload())
                    {
                        target = null;
                        Sounds.instance.Roll();
                        secondHand.parent = startParent;
                        secondHand.localPosition = startPosition;
                    }
                }
            }
        }
    }
    

}
