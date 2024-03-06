using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.tag == "Sword" || collision.tag == "Shield")
        {
            Sounds.instance.Sword();
            if (collision.transform.parent.tag == "Enemy")
            {
                Debug.LogWarning("hit!");
                collision.transform.parent.parent.parent.GetComponent<NPCController>().back = true;
            }
            else if (collision.transform.parent.tag == "Player")
            {                
                collision.transform.parent.GetComponent<CharacterController>().back = true;

            }
        }
        else if(collision.tag == "Head" || collision.tag == "Body")
        {
                Sounds.instance.Hit();
                collision.transform.parent.GetComponent<NPCController>().Hit(10);
        }
    }
}
