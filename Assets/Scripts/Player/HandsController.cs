using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class HandsController : MonoBehaviour
{
    // Transforms
    private Transform mainHand;
    private Transform shieldTransform;
    private Transform weapon;
    private Transform fliper;
    private Transform aimPoint;

    private Transform shield;

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

    public bool isGun = false;
    public bool canAttack { get; private set; }
    public float timer;
    private const float setTime = 0.45f;

    private CharacterController characterController;

    public GameObject bullet;

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
            angle1 = GetAngle(targetPos, shieldTransform, -100);
        else
            angle1 = GetAngle(targetPos, shieldTransform, 100);        
        
        float z;
        if (weapon.localEulerAngles.z > 180)
             z = Mathf.Lerp(weapon.localEulerAngles.z - 360, 0, Time.deltaTime * 10);
        else
            z = Mathf.Lerp(weapon.localEulerAngles.z , 0, Time.deltaTime * 10);

        weapon.localEulerAngles = new Vector3(weapon.localEulerAngles.x, weapon.localEulerAngles.y, z);
        mainHand.eulerAngles = Vector3.Lerp(mainHand.eulerAngles, new Vector3(0, 0, angle), Time.deltaTime * 12f);
        shieldTransform.eulerAngles = Vector3.Lerp(shieldTransform.eulerAngles, new Vector3(0, 0, angle1), Time.deltaTime * 2f);
    }
    public void SetController(IUsesWeapons usesWeapons,string enemyTag)
    {
        this.usesWeapons = usesWeapons;
        // Transform hero = transform.Find("Hero");
        mainHand = transform.Find("MainHand");
        shieldTransform = transform.Find("Shield");
        weapon = mainHand.GetChild(0).GetChild(0);
        aimPoint = weapon.GetChild(0).Find("AimPoint");
        fliper = mainHand.GetChild(0);
        shield = shieldTransform.GetChild(0);

        firstHand = transform.Find("MainHand").GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<FirstHand>();
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
            attackItem = fliper;
        }
        else
        {
            attackItem = shield;
        }

        attackAngle = weapon.localEulerAngles - Angle;  
        this.firstHand.AttackSwitch(true);
        lastWeaponPosition = attackItem.localPosition;
        attackVector = position + attackItem.localPosition;
        back = false;
        
    }
    public void Updateflip(Vector3 targetPos)
    {
        this.targetPos = targetPos;
        if(flip != this.targetPos.x < mainHand.position.x)
        {
            fliper.localPosition = new Vector3(fliper.localPosition.x, -fliper.localPosition.y, 0);
            if(flip)
            {
                fliper.localEulerAngles = new Vector3(0,0, 0);
            }
            else
            {
                fliper.localEulerAngles = new Vector3(180,0,0);
            }
        }
        flip = this.targetPos.x < mainHand.position.x;
    }
    public void Shot()
    {
        Vector3 aimDir = (MyTools.GetMouseWorldPosition() - mainHand.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle = 180 + (180 + angle);
        Instantiate(bullet,aimPoint.position, Quaternion.identity).transform.localEulerAngles = new Vector3(0, 0, angle + Random.Range(-5,5));
        Sounds.instance.Shot();
        ShotParticle(angle);
    }
    private void ShotParticle(float angle)
    {
        Instantiate(ParticleAssets.instance.shotSmoke, aimPoint.TransformPoint(aimPoint.localPosition + new Vector3(-0.05f, 0)), Quaternion.identity);
        Instantiate(ParticleAssets.instance.shotFire, aimPoint.position , Quaternion.identity).transform.localEulerAngles = new Vector3(0,0,angle);
    }
    public void UpdateAttack()
    {
          UpdateMeleeWeapon(); 
    }
    private void UpdateMeleeWeapon()
    {
        if (!back)
        {
            weapon.localEulerAngles = new Vector3(0,0,GetEulerAnglesLerp(attackAngle,30));

            attackItem.localPosition = Vector3.Lerp(attackItem.localPosition, attackVector, Time.deltaTime * 12f);

            if (CheckAngle(attackAngle,1f) && Vector3.Distance(attackItem.localPosition, attackVector) < 0.1f)
            {
                back = true;
            }
        }
        else
        {
             attackItem.localPosition = Vector3.Lerp(attackItem.localPosition, lastWeaponPosition, Time.deltaTime * 30f);

            if (Vector3.Distance(attackItem.localPosition, lastWeaponPosition) < 0.015) 
            {
                ResetAttack();
            }
        }
    }
    private float GetEulerAnglesLerp(Vector3 target,float speed)
    {
        Vector3 angle = weapon.localEulerAngles;
        if(target.z < 0) angle = new Vector3(0,0, angle.z - 360);

        return Mathf.LerpAngle(angle.z, target.z, Time.deltaTime * speed);
    }
    private bool CheckAngle(Vector3 target, float border)
    {
        Vector3 angle = target;
        if (angle.z > 360) angle = new Vector3(target.x, target.y, target.z % 360);


        if (angle.z >= 0) return Mathf.Abs(weapon.localEulerAngles.z - angle.z) < border;
        else
        {
            return Mathf.Abs(weapon.localEulerAngles.z - (360 + angle.z)) < border;
        }
    
    }

    public void Reload()
    {
        characterController.itemController.Reload();
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
            fliper.transform.localPosition = lastWeaponPosition;
            SetTransformHand(Mathf.Abs(lastWeaponPosition.y));
            attackItem = null;
            firstHand.AttackSwitch(false);
            usesWeapons.EndAttack();
        }
    }

    public void SetTransformHand(float posY)
    {
        if (flip)
        {
            fliper.localPosition = new Vector3(fliper.localPosition.x, posY, 0);
        }
        else
        {
            fliper.localPosition = new Vector3(fliper.localPosition.x, -posY, 0);
        }
    }


}
