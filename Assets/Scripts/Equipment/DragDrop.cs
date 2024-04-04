
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IDragHandler
{
    private RectTransform rectTransform;
    private Transform parent;
    private CanvasGroup canvasGroup;

    private bool isInSlot;
    private Vector2 lastPosition;

    [SerializeField] Canvas canvas;
  

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void IsInSlot()
    {
        isInSlot = true;
        parent = transform.parent;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
        rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        transform.SetParent(EquipmentManager.itemParent);
        isInSlot = false;
        lastPosition = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.localScale = Vector3.one;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        if(!isInSlot)
        {
            rectTransform.anchoredPosition = lastPosition;
            if(parent != null) transform.SetParent(parent);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       
    }
}
