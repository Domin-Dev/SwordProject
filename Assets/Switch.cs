using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchArgs : EventArgs
{
    int newValue;
    public SwitchArgs(int newValue)
    {
        this.newValue = newValue;
    }
}

public class Switch : MonoBehaviour
{
    [SerializeField] private Button left;
    [SerializeField] private Button rigth;

    private int value = 0;
    private int minValue;
    private int maxValue;
    private string nameSwitch;

    public event EventHandler<SwitchArgs> OnChangedValue;

    private void Start()
    {
        left.onClick.AddListener(() =>
        {
            DecreaseValue();
        });

        rigth.onClick.AddListener(() =>
        {
            IncreaseValue();
        });
    }

    public void SetUpSwitch(int minValue,int maxValue,string nameSwitch)
    {
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.nameSwitch = nameSwitch;
    }

    private void DecreaseValue()
    {
        if(value == minValue)
        {
            value = maxValue;
        }
        else
        {
            value--;
        }
        OnChangedValue?.Invoke(this, new SwitchArgs(value));
    }

    private void IncreaseValue()
    {
        if (value + 1 >= maxValue)
        {
            value = minValue;
        }
        else
        {
            value++;
        }
        OnChangedValue?.Invoke(this, new SwitchArgs(value));
    }



}
