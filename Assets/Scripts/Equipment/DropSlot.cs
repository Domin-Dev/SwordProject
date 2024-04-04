using UnityEngine.EventSystems;
using UnityEngine;

public class DropSlot : MonoBehaviour, IDropHandler
{
    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            eventData.pointerDrag.transform.SetParent(transform.parent);
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = this.GetComponent<RectTransform>().anchoredPosition;
            eventData.pointerDrag.GetComponent<DragDrop>().IsInSlot();
            Sounds.instance.Shield();

        }
    }
}
