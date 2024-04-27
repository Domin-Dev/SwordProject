using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterController: NetworkBehaviour, ILifePoints, IUsesWeapons
{

    [SerializeField] private int maxHP;
    [SerializeField] private int hp;
    [SerializeField] private Transform hpBar;

    [SerializeField] private Transform center;

    private Animator animator;

    private Vector2 moveDir;
    private Vector2 sightDir;
    private Rigidbody2D rigidbody2D;

    private bool isRepulsed = false;
    public HandsController attackModule { private set; get; }
    public ItemController itemController {private set; get; }

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

    private CharacterSpriteController characterSpriteController;


    private void Awake()
    {
        characterSpriteController = GetComponent<CharacterSpriteController>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        attackModule = GetComponent<HandsController>();
        itemController = GetComponent<ItemController>();

        animator = GetComponent<Animator>();  
        
        attackModule.SetController(this,"Enemy");  
        SetStateMachine();
    }
    private void Start()
    {
        heroStateMachine.RunMachine(idleState);
    }
    private void Update()
    {
        //  if (!IsOwner) return;
        sightDir = MyTools.GetMouseWorldPosition();
        attackModule.Updateflip(sightDir);
        heroStateMachine.currentState.FrameUpdate();
    }
    private void FixedUpdate()
    {
        heroStateMachine.currentState.FrameFixedUpdate();
    }
   

    #region Movement
    public void GetMovementInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveDir = new Vector2(x, y).normalized;
    }
    public void UpdateMovement()
    {
        if (!isRepulsed)
        {
            characterSpriteController.UpdateSprite(moveDir,(sightDir - (Vector2)center.position).normalized);
            rigidbody2D.velocity = moveDir;
            if(moveDir == new Vector2(0,0))
            {
                animator.SetBool("Idle", true);
            }
            else
            {
                animator.SetBool("Idle", false);
            }
        }
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

