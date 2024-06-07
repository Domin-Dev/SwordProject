using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class ButtonHold: MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isPressed;
    public Action action;
    Timer timer;
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        if (timer != null) timer.Cancel();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        timer = Timer.Create(0.2f, () => { isPressed = true; return false; });
        k = 0;
    }
    public void Cancel()
    {
        isPressed = false;
    }

    int i = 0;
    int k = 0;
    public void FixedUpdate()
    {
        if (isPressed)
        {
            i++;
            k++;
            if (i >= (5 - k / 20) + 1)
            {
                action();
                i = 0;
            }
        }
    }
}

