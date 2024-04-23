using UnityEngine;

public class GameItem : MonoBehaviour
{
    public ItemStats itemStats;

    private void Awake()
    {
        itemStats = ItemsAsset.instance.GetItemStats(Random.Range(0, 2));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.parent != null && collision.transform.parent.CompareTag("Player"))
        {
           if(EquipmentManager.instance.AddNewItem(itemStats)) Destroy(gameObject);
        }
    }
}
