using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameItem : MonoBehaviour
{
    public ItemStats itemStats;

    private void Awake()
    {
        itemStats = new ItemStats(Random.Range(0,4), Random.Range(30,200));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.parent != null && collision.transform.parent.CompareTag("Player"))
        {
           if(EquipmentManager.instance.AddNewItem(itemStats)) Destroy(gameObject);
        }
    }
}
