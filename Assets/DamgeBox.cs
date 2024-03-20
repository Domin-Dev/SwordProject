using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamgeBox : MonoBehaviour
{

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Weapon");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Head" || collision.tag == "Body")
        {
            Sounds.instance.Hit();
            collision.transform.parent.GetComponent<ILifePoints>().Hit(10, transform.parent.position);
        }
    }
}
