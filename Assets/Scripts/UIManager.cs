using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] Transform itemBar;

    [SerializeField] Sprite selected;
    [SerializeField] Sprite unSelected;

    public static UIManager instance { private set; get; }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void SetUp(EquipmentManager eqManager)
    {
        eqManager.UpdateSelectedSlotInBar += UpdateSelectedSlot;
    }

    private void UpdateSelectedSlot(object sender, UpdateSelectedSlotInBarArgs e)
    {
        itemBar.GetChild(e.lastSlot).GetComponent<Image>().sprite = unSelected;
        itemBar.GetChild(e.currentSlot).GetComponent<Image>().sprite = selected;
    }
}
