using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHP;
    [SerializeField] private int hp;
    [SerializeField] private Transform hpBar;

    private Rigidbody2D rigidbody2D;
    private Transform aimTransform;
    private Transform child;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        aimTransform = transform.Find("Aim");
        child = aimTransform.GetChild(0);
        hp = maxHP;
    }

    private void Update()
    {
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.transform.parent != null && collision.transform.parent.tag == "Player") 
        {
            Follow(collision.transform); 
        }  
    }

    private void Follow(Transform target)
    {
        rigidbody2D.velocity = target.position - transform.position;
        Aim(target);
    }

    private void Aim(Transform target)
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
        bool flip = target.position.x < aimTransform.position.x;


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
        Debug.Log("hit");
    }
  
}
