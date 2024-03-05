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
            if(collision.transform.parent.tag == "Enemy")
            {
                Sounds.instance.Hit();
                collision.transform.parent.GetComponent<NPCController>().Hit(10);
            }
        }
    }
}
