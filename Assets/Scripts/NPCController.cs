using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class NPCController : MonoBehaviour
{
    public bool isTarget;


    [SerializeField] private int maxHP;
    [SerializeField] private int hp;
    [SerializeField] private Transform hpBar;


    private Rigidbody2D rigidbody2D;
    private Transform aimTransform;
    private Transform child;

    public HeroStateMachine heroStateMachine;

    public NPCIdleState idleState;
    public NPCFollowState followState;
    public NPCAttackState attackState;

    private Transform target;

    float timer;
    float setTime = 1.5f;
    public bool canAttack;

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rigidbody2D = GetComponent<Rigidbody2D>();
        aimTransform = transform.Find("Aim");
        child = aimTransform.GetChild(0);
        hp = maxHP;
        SetStateMachine();
    }
    private void Start()
    {
       heroStateMachine.RunMachine(idleState);  
    }
    private void SetStateMachine()
    {
        heroStateMachine = new HeroStateMachine();
        idleState = new NPCIdleState(this,heroStateMachine);
        followState = new NPCFollowState(this, heroStateMachine);
        attackState = new NPCAttackState(this, heroStateMachine);
    }
    private void Update()
    {
        if(!canAttack)
        {
            timer += Time.deltaTime;
            if(timer >= setTime)
            {
                canAttack = true;
                timer = 0;
            }
        }
        heroStateMachine.currentState.FrameUpdate();
    }
    private void FixedUpdate()
    {
        heroStateMachine.currentState.FrameFixedUpdate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent != null && collision.transform.parent.tag == "Player")
        {
            target = collision.transform;
            isTarget = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.parent != null && collision.transform.parent.tag == "Player")
        {
            Debug.Log("siema");
            target = null;
            isTarget = false;
        }
    }
    public void Attack()
    {

    }
    public void Follow()
    {
        rigidbody2D.velocity = target.position - transform.position;    
    }
    public void StopFollow()
    {
        rigidbody2D.velocity = Vector3.zero;
    }
    public float GetDistance()
    {
        return Vector3.Distance(transform.position, target.position);
    }
    public void Aim()
    {
        Vector3 aimDir = (target.position - transform.position).normalized;
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
        aimTransform.eulerAngles = Vector3.Lerp(aimTransform.eulerAngles, new Vector3(0, 0, angle), Time.deltaTime * 4f);
        flip = target.position.x < aimTransform.position.x;


        if (flip)
        {
            child.localEulerAngles = Vector3.Lerp(child.localEulerAngles, new Vector3(0, child.localEulerAngles.y, 180), Time.deltaTime * 10f);
        }
        else
        {
            //  child.localEulerAngles = new Vector3(0, 180, child.localEulerAngles.z);
            child.localEulerAngles = Vector3.Lerp(child.localEulerAngles, new Vector3(0, child.localEulerAngles.y, 0), Time.deltaTime * 10);
        }
    }
    public void Hit(int damage)
    {
        hp = Mathf.Clamp(hp - damage, 0, maxHP);
        if(hp == 0) Destroy(gameObject);
        hpBar.localScale = new Vector3((float)hp / maxHP, 1, 1);
    }
   
    private bool flip;
    private Vector3 attackAngle;
    private Vector3 attackVector;
    private Vector3 lastWeaponPosition;
    public bool back;
    public void SetAttackVector(Vector3 Angle, Vector3 position)
    {
        if (flip)
        {
            child.localEulerAngles = new Vector3(0, 180, 180);
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

    public void UpdateAttack()
    {
        if (!back)
        {
            child.localEulerAngles = Vector3.Lerp(child.localEulerAngles, attackAngle, Time.deltaTime * 10f);
            child.localPosition = Vector3.Lerp(child.localPosition, attackVector, Time.deltaTime * 12f);

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
                heroStateMachine.ChangeState(followState);
                child.localPosition = lastWeaponPosition;
            }
        }
    }
}
