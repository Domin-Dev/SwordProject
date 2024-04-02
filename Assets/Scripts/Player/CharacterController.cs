using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class CharacterController: NetworkBehaviour, ILifePoints, IUsesWeapons
{

    [SerializeField] private int maxHP;
    [SerializeField] private int hp;
    [SerializeField] private Transform hpBar;


    private Vector2 moveDir;
    private Rigidbody2D rigidbody2D;


    private Transform weaponTransform;
    private Transform shieldTransform;
    private Transform child;
    private Transform shield;

    public bool flip;
    private bool isRepulsed = false;

    public AttackModule attackModule { set; get; }

    #region StateMachine
    public HeroStateMachine heroStateMachine { set; get; }
    public IdleState idleState { set; get; }
    public AttackState attackState { set; get; }

    private void SetStateMachine()
    {
        heroStateMachine = new HeroStateMachine();
        idleState = new IdleState(this, heroStateMachine);
        attackState = new AttackState(this, heroStateMachine);
    }

    #endregion

    #region built-in functions
    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        attackModule = GetComponent<AttackModule>();
        attackModule.SetController(this,"Enemy");       
        SetStateMachine();
    }
    private void Start()
    {
        heroStateMachine.RunMachine(idleState);
    }
    private void Update()
    {
        if (!IsOwner) return;
        attackModule.Updateflip(MyTools.GetMouseWorldPosition());
        heroStateMachine.currentState.FrameUpdate();
    }
    bool foolow = true;
    private void FixedUpdate()
    {
      if(foolow)   heroStateMachine.currentState.FrameFixedUpdate();
    }

    #endregion

    #region Movement
    public void GetMovementInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveDir = new Vector2(x, y).normalized;
    }
    public void UpdateMovement()
    {
       if(!isRepulsed) rigidbody2D.velocity = moveDir;
    }
    #endregion

    #region ILifePoints
    void ILifePoints.Hit(int damage, Vector2 dir)
    {
        StartCoroutine(HitForce(((Vector2)transform.position - dir).normalized));
        hp = Mathf.Clamp(hp - damage, 0, maxHP);
        if (hp == 0) Destroy(gameObject);
        hpBar.localScale = new Vector3((float)hp / maxHP, 1, 1);
    }

    void ILifePoints.Kill()
    {

    }
    #endregion

    #region IUsesWeapons
    void IUsesWeapons.Block()
    {
        StartCoroutine(BlockTime());
    }

    IEnumerator BlockTime()
    {
        yield return new WaitForSeconds(.005f);
        attackModule.back = true;
    }
    void IUsesWeapons.EndAttack()
    {
        heroStateMachine.ChangeState(idleState);
    }

    public IEnumerator HitForce(Vector2 dir)
    {
        isRepulsed = true;
        rigidbody2D.AddForce(2 * dir.normalized, ForceMode2D.Impulse);
        attackModule.ResetAttack(); 
        yield return new WaitForSeconds(0.1f);
        isRepulsed = false;
    }
    #endregion

}

