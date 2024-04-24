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


    [SerializeField] private Transform secondHand;
    Vector3 secondHandStartPosition;
    [SerializeField] private Transform center;
    [SerializeField] private Transform secondHandParent;
    [SerializeField] private SpriteRenderer itemInHand;
    [SerializeField] private PolygonCollider2D hitBox;
    [SerializeField] private Transform mainHand;
    [SerializeField] private Transform aimPoint;



    private Animator animator;

    private Vector2 moveDir;
    private Vector2 sightDir;
    private Rigidbody2D rigidbody2D;

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

    private CharacterSpriteController characterSpriteController;

    public event EventHandler UseItem;

    private void Awake()
    {
        characterSpriteController = GetComponent<CharacterSpriteController>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        attackModule = GetComponent<AttackModule>();
        animator = GetComponent<Animator>();  
        
        attackModule.SetController(this,"Enemy");  
        secondHandStartPosition = secondHand.localPosition;
        SetUpEvents();
        SetStateMachine();
    }
    private void Start()
    {
        heroStateMachine.RunMachine(idleState);
        EquipmentManager.instance.SetUpEvent(this);
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

    private void SetUpEvents()
    {
        EquipmentManager.instance.UpdateItemInHand += UpdateItemInHand;   
    }

    private void UpdateItemInHand(object sender, ItemStatsArgs e)
    {
        itemInHand.transform.localPosition = Vector3.zero;
        attackModule.isGun = false;
        
        if (e.item != null)
        {
            Item item = ItemsAsset.instance.GetItem(e.item.itemID);
            if (item as Weapon != null)
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

                if(weapon as RangedWeapon != null)
                {
                    attackModule.isGun = true;
                    RangedWeapon rangedWeapon = (RangedWeapon)weapon;
                    float posY = Mathf.Abs(rangedWeapon.aimPoint.y - rangedWeapon.gripPoint1.y);
                    attackModule.SetTransformHand(posY);
                    aimPoint.localPosition = (Vector2)itemInHand.transform.localPosition + rangedWeapon.aimPoint;
                }
                else
                {

                }
            }
            else
            {
                itemInHand.sprite = item.icon;
                itemInHand.sortingOrder = 20;
                SecondHandReset();
            }
        }
        else
        {
            itemInHand.sprite = null;
            SecondHandReset();
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

    public void Use()
    {
        UseItem(this,null);
    }
}

