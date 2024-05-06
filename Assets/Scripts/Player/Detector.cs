using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    CharacterController controller;
    private void Start()
    {
        controller = transform.parent.GetComponent<CharacterController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Tree")
        {
            collision.transform.parent.GetComponent<ITransparent>().Hide();
        }      
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Horse" && Input.GetKeyDown(KeyCode.E))
        {
            controller.Riding(collision.GetComponentInParent<Horse>());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Tree")
        {
            collision.transform.parent.GetComponent<ITransparent>().Show();
        }
    }
}
