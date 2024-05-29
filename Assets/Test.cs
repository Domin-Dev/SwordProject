using UnityEngine.EventSystems;
using UnityEngine;

public class Test : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("skok");
    }
}
