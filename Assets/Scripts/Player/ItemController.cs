using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{

    [SerializeField] private Transform secondHand;
    Vector3 secondHandStartPosition;
    [SerializeField] private Transform center;
    [SerializeField] private Transform secondHandParent;
    [SerializeField] private SpriteRenderer itemInHand;
    [SerializeField] private PolygonCollider2D hitBox;
    [SerializeField] private Transform mainHand;
    [SerializeField] private Transform aimPoint;


}
