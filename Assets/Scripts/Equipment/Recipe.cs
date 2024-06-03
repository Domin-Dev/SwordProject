
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Recipe : MonoBehaviour, IPointerClickHandler
{
    public int id;
    public void SetID(int id)
    {
        this.id = id;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.SelectRecipe(id);
    }
}
