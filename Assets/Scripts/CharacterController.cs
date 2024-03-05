using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class CharacterController: MonoBehaviour
{
   
    private Rigidbody2D rigidbody2D;
    private Vector2 moveDir;

    private Transform weaponTransform;
    private Transform shieldTransform;
    private Transform child;

    public HeroStateMachine heroStateMachine { set; get; }
    public IdleState idleState { set; get; }
    public AttackState attackState { set; get; }

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        weaponTransform = transform.Find("Weapon");
        shieldTransform = transform.Find("Shield");
        child = weaponTransform.GetChild(0);
        SetStateMachine();
    }


    private void Start()
    {
        heroStateMachine.RunMachine(idleState);
    }
    private void Update()
    {
        heroStateMachine.currentState.FrameUpdate();


   //     Debug.Log(child.localEulerAngles);

    }

    private void SetStateMachine()
    {
        heroStateMachine = new HeroStateMachine();
        idleState = new IdleState(this, heroStateMachine);
        attackState = new AttackState(this, heroStateMachine);
    }


    private void FixedUpdate()
    {
        heroStateMachine.currentState.FrameFixedUpdate();
    }


    public void GetMovementInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveDir = new Vector2(x, y).normalized;
    }

    public void UpdateMovement()
    {
        rigidbody2D.velocity = moveDir;
    }

    private bool flip;
    private float GetAngle(Vector3 mousePos,Transform aimTransform,float addValue)
    {
        Vector3 aimDir = (mousePos - transform.position).normalized;
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
    public void Aim()
    {
        Vector3 mousePos = MyTools.GetMouseWorldPosition();
        float angle = GetAngle(mousePos,weaponTransform,0);
        float angle1 = GetAngle(mousePos,shieldTransform,0);

        weaponTransform.eulerAngles = Vector3.Lerp(weaponTransform.eulerAngles, new Vector3(0, 0, angle), Time.deltaTime * 8f);
        shieldTransform.eulerAngles = Vector3.Lerp(shieldTransform.eulerAngles, new Vector3(0, 0, angle1 ), Time.deltaTime * 2f);
        flip = mousePos.x < weaponTransform.position.x;
     

        if (flip)
        {
            child.localEulerAngles = Vector3.Lerp(child.localEulerAngles, new Vector3(0, child.localEulerAngles.y, 180), Time.deltaTime * 10f);
        }
        else
        {
          //  child.localEulerAngles = new Vector3(0, 180, child.localEulerAngles.z);
            child.localEulerAngles = Vector3.Lerp(child.localEulerAngles, new Vector3(0, child.localEulerAngles.y, 0), Time.deltaTime * 10);
        }
      //  Debug.Log(child.localPosition);
    }


    private Vector3 attackAngle;
    private Vector3 attackVector;
    private Vector3 lastWeaponPosition;
    public void SetAttackVector(Vector3 Angle,Vector3 position)
    {
        if(flip)
        {
            child.localEulerAngles = new Vector3(0 ,180, 180);
            attackAngle = child.localEulerAngles - Angle;
        }
        else
        {
            child.localEulerAngles = new Vector3(0, 180, child.localEulerAngles.z);
            attackAngle = child.localEulerAngles + Angle;
        }

        lastWeaponPosition = child.localPosition;
        attackVector = position + child.localPosition;
        back = false;
    }
    bool back;
    public void UpdateShield()
    {
        Vector3 mousePos = MyTools.GetMouseWorldPosition();
        float angle = GetAngle(mousePos, weaponTransform, 90);
        float angle1 = GetAngle(mousePos, shieldTransform, 180);

        weaponTransform.eulerAngles = Vector3.Lerp(weaponTransform.eulerAngles, new Vector3(0, 0, angle), Time.deltaTime * 8f);
        shieldTransform.eulerAngles = Vector3.Lerp(shieldTransform.eulerAngles, new Vector3(0, 0, angle1), Time.deltaTime * 8f);
    }
    public void UpdateAttack()
    {
        if(!back)
        {
            child.localEulerAngles = Vector3.Lerp(child.localEulerAngles, attackAngle, Time.deltaTime * 20f);
            child.localPosition = Vector3.Lerp(child.localPosition, attackVector, Time.deltaTime * 10f);

            if (Vector3.Distance(child.localEulerAngles, attackAngle) < 0.5f)
            {
                back = true;
            }
        }
        else
        {
            child.localPosition = Vector3.Lerp(child.localPosition, lastWeaponPosition, Time.deltaTime * 15f);
            if (Vector3.Distance(child.localPosition, lastWeaponPosition) < 0.5)
            {
                heroStateMachine.ChangeState(idleState);
                child.localPosition = lastWeaponPosition;
            }
        }
    }



}

