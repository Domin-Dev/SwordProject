using UnityEngine.EventSystems;
using UnityEngine;

public class BuildingObjectUI : MonoBehaviour, IPointerClickHandler
{
    public int ID;
    public void OnPointerClick(PointerEventData eventData)
    {
        BuildingManager.instance.SelectedBuildingObject(ID);
    }
}
