using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class CharacterController: MonoBehaviour
{

    private Rigidbody2D rigidbody2D;
    private Vector2 moveDir;

    private Transform aimTransform;
    private Transform child;

    public PlayerStateMachine playerStateMachine {set; get; }
    public IdleState idleState { set; get; }
    public AttackState attackState { set; get; }

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        aimTransform = transform.Find("Aim");
        child = aimTransform.GetChild(0);
        playerStateMachine = new PlayerStateMachine();

        idleState = new IdleState(this, playerStateMachine);
        attackState = new AttackState(this, playerStateMachine);
    }

    private void Start()
    {
        playerStateMachine.RunMachine(idleState);
    }
    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveDir = new Vector2(x, y).normalized;
        playerStateMachine.currentState.FrameUpdate();
   //     Debug.Log(child.localEulerAngles);

    }

    private void FixedUpdate()
    {
        rigidbody2D.velocity = moveDir;      
    }

    private bool flip;
    public void Aim()
    {
        Vector3 mousePos = MyTools.GetMouseWorldPosition();
        Vector3 aimDir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;


        if (angle < 0) angle = 180 + (180 + angle);
        if (aimTransform.eulerAngles.z - angle > 180)
        {
            angle = 360 + angle;
        }
        else if (aimTransform.eulerAngles.z - angle < -180)
        {
            angle = -(360 - angle);
        }
        aimTransform.eulerAngles = Vector3.Lerp(aimTransform.eulerAngles, new Vector3(0, 0, angle), Time.deltaTime * 8f);
        flip = mousePos.x < aimTransform.position.x;


        if (flip)
        {
            child.localEulerAngles = Vector3.Lerp(child.localEulerAngles, new Vector3(0, child.localEulerAngles.y, 180), Time.deltaTime * 10f);
        }
        else
        {
          //  child.localEulerAngles = new Vector3(0, 180, child.localEulerAngles.z);
            child.localEulerAngles = Vector3.Lerp(child.localEulerAngles, new Vector3(0, child.localEulerAngles.y, 0), Time.deltaTime * 10);
        }
        Debug.Log(child.localPosition);
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
                playerStateMachine.ChangeState(idleState);
                child.localPosition = lastWeaponPosition;
            }
        }
    }



}
