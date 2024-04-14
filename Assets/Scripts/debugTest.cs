using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugTest : MonoBehaviour
{
    public Weapon weapon;
    void Start()
    {
       PolygonCollider2D pol =  GetComponent<PolygonCollider2D>();
        pol.points = weapon.hitBoxPoints;
    }

    
}
