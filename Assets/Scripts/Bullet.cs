using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;

    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.right * bulletSpeed;
    }
}
