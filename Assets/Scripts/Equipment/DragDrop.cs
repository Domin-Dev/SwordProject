
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Transform parent;
    private CanvasGroup canvasGroup;

    private bool isInSlot;

    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetCanvas(Canvas canvas)
    {
        this.canvas = canvas;
    }

    public void IsInSlot()
    {
        isInSlot = true;
        parent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if(parent != null) EquipmentManager.instance.selectedSlotInEQ = parent.GetComponent<DropSlot>().GetSlotPosition();
            canvasGroup.alpha = 0.7f;
            canvasGroup.blocksRaycasts = false;
            rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            transform.SetParent(UIManager.instance.itemParent);
            isInSlot = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            EquipmentManager.instance.selectedSlotInEQ = new SlotPosition(-1, -1);
            rectTransform.localScale = Vector3.one;
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            if (!isInSlot)
            {
                if (parent != null) transform.SetParent(parent);
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }
    }
}
