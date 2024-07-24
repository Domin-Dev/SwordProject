using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public ItemStats itemStats;
    Timer timer;
    Timer timerTransform;
    Timer timerFollow;
    private void SetUp(Vector2 target)
    {
        transform.localScale = new Vector2(0, 0);
        timer = Timer.Create
        (() =>
        {
            float scaleX = Mathf.Lerp(transform.localScale.x, 1.5f, Time.deltaTime * 18f); ;
            float scaleY = Mathf.Lerp(transform.localScale.y, 1.5f, Time.deltaTime * 15f);
            transform.localScale = new Vector3(scaleX, scaleY);
            if (transform.localScale.y > 0.99f)
            {
                return true;
            }
            return false;
        },
        () =>
        {
            float scaleX = Mathf.Lerp(transform.localScale.x, 1f, Time.deltaTime * 25);
            float scaleY = Mathf.Lerp(transform.localScale.y, 1f, Time.deltaTime * 20);
            transform.localScale = new Vector3(scaleX, scaleY);
            if (transform.localScale.x <= 1.01f)
            {
                transform.localScale = new Vector2(1f, 1f);
                return true;
            }
            return false;
        }
        );

        timerTransform = Timer.Create
        (() =>
        {
            Vector2 pos = Vector2.Lerp(transform.position, target, Time.deltaTime * 10f);
            transform.position = pos;
            if (Vector2.Distance(transform.position,target) < 0.03f)
            {
                return true;
            }
            return false;
        },
        () =>
        {
                return true;
        }
        );
    }
    public void SetItem(ItemStats itemStats,Vector2 target)
    {
        this.itemStats = itemStats;
        GetComponent<SpriteRenderer>().sprite = ItemsAsset.instance.GetIcon(itemStats.itemID);
        SetUp(target);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.parent != null && collision.transform.parent.CompareTag("Player"))
        {
            timerFollow = Timer.Create(() =>
            {
                Vector2 pos = Vector2.Lerp(transform.position, collision.transform.position, Time.deltaTime * 13f);
                transform.position = pos;
                if (Vector2.Distance(transform.position,collision.transform.position) < 0.16f)
                {
                    AddItem();
                    return true;
                }
                return false;
            },()=>
            {
                return false;
            }); 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.parent != null && collision.transform.parent.CompareTag("Player"))
        {
            if (timerFollow != null) timerFollow.Cancel();
        }
    }

    private void AddItem()
    {
        if (EquipmentManager.instance.AddNewItem(itemStats))
        {
            Sounds.instance.Click();
            if(timerFollow != null) timerFollow.Cancel();
            if(timer != null) timer.Cancel();
            if(timerTransform != null) timerTransform.Cancel();
            Destroy(gameObject);
        }
    }
}
