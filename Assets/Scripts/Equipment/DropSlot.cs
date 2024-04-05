using UnityEngine.EventSystems;
using UnityEngine;

public class DropSlot : MonoBehaviour, IDropHandler
{
    private SlotPosition slotPosition;

    public void SetSlotPosition(int slotIndex,int gridIndex)
    {
        slotPosition = new SlotPosition(gridIndex,slotIndex);  
    }

    public SlotPosition GetSlotPosition()
    {
        return slotPosition;
    }

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {    
            if (eventData.pointerDrag != null)
            {
                eventData.pointerDrag.transform.SetParent(transform);
                eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                eventData.pointerDrag.GetComponent<DragDrop>().IsInSlot();
                EquipmentManager.instance.MoveSelectedItem(slotPosition);

                Sounds.instance.Shield();

            }
        }
    }
}
