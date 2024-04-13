using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class AttackModule : MonoBehaviour
{
    // Transforms
    private Transform mainHand;
    private Transform shieldTransform;
    private Transform weapon;
    private Transform shield;

    //Attack
    private Vector3 attackAngle;
    private Vector3 attackVector;
    private Vector3 lastWeaponPosition;

    private Transform attackItem;
    public bool back;

    //Aim
    private Vector3 targetPos;
    private bool flip;

    //other
    private FirstHand firstHand;
    private IUsesWeapons usesWeapons;

    public bool canAttack { get; private set; }
    public float timer;
    private const float setTime = 0.45f;

    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }
    private void Update()
    {
        if (!canAttack)
        {
            timer += Time.deltaTime;
            if (timer >= setTime)
            {
                canAttack = true;
                timer = 0;
            }
        }
    }
    public void Aim()
    {
        float angle = GetAngle(targetPos, mainHand, 0);
        float angle1; 

        if (flip)
        {
            weapon.localEulerAngles = Vector3.Lerp(weapon.localEulerAngles, new Vector3(0, weapon.localEulerAngles.y, 180), Time.deltaTime * 10f);
            angle1 = GetAngle(targetPos, shieldTransform, -100);
        }
        else
        {
            weapon.localEulerAngles = new Vector3(0, 180, weapon.localEulerAngles.z);
            weapon.localEulerAngles = Vector3.Lerp(weapon.localEulerAngles, new Vector3(0, weapon.localEulerAngles.y, 0), Time.deltaTime * 10);
            angle1 = GetAngle(targetPos, shieldTransform, 100);
        }


        mainHand.eulerAngles = Vector3.Lerp(mainHand.eulerAngles, new Vector3(0, 0, angle), Time.deltaTime * 12f);
        shieldTransform.eulerAngles = Vector3.Lerp(shieldTransform.eulerAngles, new Vector3(0, 0, angle1), Time.deltaTime * 2f);

        weapon.GetChild(0).GetComponent<SpriteRenderer>().flipX = !flip;
        weapon.GetComponent<SpriteRenderer>().flipX = !flip;

    }
    public void SetController(IUsesWeapons usesWeapons,string enemyTag)
    {
        this.usesWeapons = usesWeapons;
        // Transform hero = transform.Find("Hero");
        mainHand = transform.Find("MainHand");
        shieldTransform = transform.Find("Shield");
        weapon = mainHand.GetChild(0);
        shield = shieldTransform.GetChild(0);

        firstHand = transform.Find("MainHand").GetChild(0).GetChild(0).GetComponent<FirstHand>();
        firstHand.SetController(usesWeapons,enemyTag,transform);
    }
    private float GetAngle(Vector3 mousePos, Transform aimTransform, float addValue)
    {
        Vector3 aimDir = (mousePos - aimTransform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        angle += addValue;
        if (angle < 0) angle = 180 + (180 + angle);
        if (aimTransform.eulerAngles.z - angle > 180)
        {
            angle = 360 + angle;
        }
        else if (aimTransform.eulerAngles.z - angle < -180)
        {
            angle = -(360 - angle);
        }
        return angle;
    }
    public void SetAttackVector(Vector3 Angle, Vector3 position, bool firstHand)
    {
        canAttack = false;
        if (firstHand)
        {
            attackItem = weapon;
        }
        else
        {
            attackItem = shield;
        }

        if (flip)
        {
            attackItem.localEulerAngles = new Vector3(0, 180, 180);
            attackAngle = attackItem.localEulerAngles - Angle;
        }
        else
        {
            attackItem.localEulerAngles = new Vector3(0, 180, attackItem.localEulerAngles.z);
            attackAngle = attackItem.localEulerAngles + Angle;
        }

        this.firstHand.AttackSwitch(true);
        lastWeaponPosition = attackItem.localPosition;
        attackVector = position + attackItem.localPosition;
        back = false;
    }
    public void Updateflip(Vector3 targetPos)
    {
        this.targetPos = targetPos;
        flip = this.targetPos.x < mainHand.position.x;
    }
    public void UpdateAttack()
    {
        if (!back)
        {
            attackItem.localEulerAngles = Vector3.Lerp(attackItem.localEulerAngles, attackAngle, Time.deltaTime * 30);
            attackItem.localPosition = Vector3.Lerp(attackItem.localPosition, attackVector, Time.deltaTime * 10f);

            if (Vector3.Distance(attackItem.localEulerAngles, attackAngle) < 0.2f && Vector3.Distance(attackItem.localPosition, attackVector) < 0.1f)
            {
                back = true;
            }
        }
        else
        {
            attackItem.localPosition = Vector3.Lerp(attackItem.localPosition, lastWeaponPosition, Time.deltaTime * 25f);
            if (Vector3.Distance(attackItem.localPosition, lastWeaponPosition) < 0.0015)
            {
                ResetAttack();
            }
        }
    }

    //public void UpdateShield()
    //{
    //    float angle;
    //    if (flip) angle = GetAngle(targetPos, weaponTransform, -20);
    //    else angle = GetAngle(targetPos, weaponTransform, 20);


    //    weapon.localEulerAngles = new Vector3(weapon.localEulerAngles.x, 0, weapon.localEulerAngles.z);
    //    if (flip)
    //    {
    //        weapon.localEulerAngles = Vector3.Lerp(weapon.localEulerAngles, new Vector3(0, weapon.localEulerAngles.y, 180), Time.deltaTime * 10f);
    //    }
    //    else
    //    {
    //        weapon.localEulerAngles = Vector3.Lerp(weapon.localEulerAngles, new Vector3(0, weapon.localEulerAngles.y, 0), Time.deltaTime * 10);
    //    }


    //    float angle1 = GetAngle(targetPos, shieldTransform, 180);

    //    weaponTransform.eulerAngles = Vector3.Lerp(weaponTransform.eulerAngles, new Vector3(0, 0, angle), Time.deltaTime * 12f);
    //    shieldTransform.eulerAngles = Vector3.Lerp(shieldTransform.eulerAngles, new Vector3(0, 0, angle1), Time.deltaTime * 4f);
    //}
    public void ResetAttack()
    {
        if(attackItem != null)
        {
            attackItem.localPosition = lastWeaponPosition;
            attackItem = null;
            firstHand.AttackSwitch(false);
            usesWeapons.EndAttack();
        }
    }


}
