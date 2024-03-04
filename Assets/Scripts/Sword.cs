using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Sword")
        {
            Sounds.instance.Sword();
        }
        else if(collision.tag == "Head" || collision.tag == "Body")
        {
            Sounds.instance.Hit();
            if(collision.transform.parent.tag == "Enemy")
            {
                collision.transform.parent.GetComponent<Enemy>().Hit(10);
            }
        }
    }
}
