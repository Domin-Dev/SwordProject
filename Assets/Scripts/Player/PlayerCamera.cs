using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float cameraSpeed;


    private void Update()
    {
        if (CanFollow())
        {
            Vector3 position = Vector3.Lerp(transform.position, player.position, Time.deltaTime * cameraSpeed);
            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }
    }

    private bool CanFollow()
    {
        return Math.Abs(transform.position.x - player.position.x) > 1 || Math.Abs(transform.position.y - player.position.y) > 1;
    }
}
