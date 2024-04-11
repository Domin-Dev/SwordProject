using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FirstHand : MonoBehaviour
{

    private bool isAttacking;
    IUsesWeapons usesWeapons;
    string enemyTag;
    Transform controllerTransform;

    List<Collider2D> colliders;

    private void Start()
    {
        StartCoroutine(CheckColliders());
        colliders = new List<Collider2D> ();
    }
    public void SetController(IUsesWeapons usesWeapons, string enemyTag, Transform controllerTransform)
    {
        this.enemyTag = enemyTag;
        this.usesWeapons = usesWeapons;
        this.controllerTransform = controllerTransform;
    }

    public void AttackSwitch(bool value)
    {
        isAttacking = value;
    }
    IEnumerator CheckColliders()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if(isAttacking)
            {
                foreach (Collider2D collider in colliders)
                {              
                    if (collider.tag == "Head" || collider.tag == "Body")
                    {
                        if (collider.transform.parent.tag == enemyTag)
                        {
                            Sounds.instance.Hit();
                            collider.transform.parent.GetComponent<ILifePoints>().Hit(10,controllerTransform.position);
                            Block();
                            break;
                        }
                    }
                    else if (collider.tag == "Shield")
                    {
                        Block();
                        Sounds.instance.Shield();
                        break;
                    }
                    else if(collider.tag == "Tree")
                    {
                        Block();
                        collider.transform.parent.GetComponent<ILifePoints>().Hit(10, controllerTransform.position);
                        Sounds.instance.Shield();
                        break;
                    }
                }
                colliders.Clear();
            }
        }
    }

    private void Block()
    {
        isAttacking = false;
        usesWeapons.Block();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {        
        if(isAttacking) colliders.Add(collision);
    }
}
