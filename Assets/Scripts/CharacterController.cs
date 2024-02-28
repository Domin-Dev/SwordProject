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

    bool attack;

    public PlayerStateMachine playerStateMachine {set; get; }
    public IdleState idleState { set; get; }
    

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        aimTransform = transform.Find("Aim");
        child = aimTransform.GetChild(0);
        playerStateMachine = new PlayerStateMachine();
        idleState = new IdleState(this, playerStateMachine);
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
        Inputk();
        playerStateMachine.currentState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        rigidbody2D.velocity = moveDir;      
    }




 


    private void Inputk()
    {
        if (Input.GetMouseButtonDown(0))
        {
            attack = true;
           // if (flip)
         //   {
                child.localEulerAngles = child.localEulerAngles + new Vector3(0, 0, -120);
          //  }
        //    else
        //    {
                child.localEulerAngles = child.localEulerAngles + new Vector3(0, 0, 120);
         //   }
        }
    }
    public void Aim()
    {
        bool flip;
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
        aimTransform.eulerAngles = Vector3.Lerp(aimTransform.eulerAngles, new Vector3(0, 0, angle), Time.deltaTime * 5f);
        flip = mousePos.x < aimTransform.position.x;

        if (attack)
        {
            if (flip)
            {
                child.localEulerAngles = Vector3.Lerp(child.localEulerAngles, new Vector3(0, child.localEulerAngles.y, 180), Time.deltaTime * 8f);
            }
            else
            {
                child.localEulerAngles = new Vector3(0, 180, child.localEulerAngles.z);
                child.localEulerAngles = Vector3.Lerp(child.localEulerAngles, new Vector3(0, child.localEulerAngles.y, 0), Time.deltaTime * 8f);
            }
        }

    }
}
