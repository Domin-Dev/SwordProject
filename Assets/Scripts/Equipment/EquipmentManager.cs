using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class UpdateSelectedSlotInBarArgs : EventArgs
{
    public int lastSlot;
    public int currentSlot;
    public UpdateSelectedSlotInBarArgs(int lastSlot, int currentSlot)
    {
        this.lastSlot = lastSlot;
        this.currentSlot = currentSlot;
    }
}
public class OpenEquipmentUIArgs : EventArgs
{
    public bool open;
    public OpenEquipmentUIArgs(bool open)
    {
        this.open = open;
    }
}


public class EquipmentManager : MonoBehaviour
{
    public event EventHandler<UpdateSelectedSlotInBarArgs> UpdateSelectedSlotInBar;
    public event EventHandler<OpenEquipmentUIArgs> OpenEquipmentUI;

    private int lastSelectedSlot { get; set; } = 2;
    private bool equipmentIsOpen = false;

    public static readonly int BarSlotCount = 10;
    public static readonly int SlotCount = 30;
   
    private ItemSlot[] equipment = new ItemSlot[SlotCount];

    public static Transform itemParent { private set; get; }

    private void Awake()
    {
        itemParent = GameObject.FindWithTag("Items").transform;
    }
    private void Start()
    {
        UIManager.instance.SetUpUIEquipment(this);
        ChangeSelectedSlot(0);
    } 
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeSelectedSlot(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ChangeSelectedSlot(9);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            equipmentIsOpen = !equipmentIsOpen;
            OpenEquipmentUIArgs args = new OpenEquipmentUIArgs(equipmentIsOpen);
            OpenEquipmentUI(this, args);
        }

        if (Input.mouseScrollDelta.y > 0)
        {
            NextSlot();
        }
        else if(Input.mouseScrollDelta.y < 0)
        {
            BackSlot();
        }
    }

    private void NextSlot()
    {
        if(lastSelectedSlot == BarSlotCount - 1)
        {
            ChangeSelectedSlot(0);
        }
        else
        {
            ChangeSelectedSlot(lastSelectedSlot + 1);
        }
    }
    private void BackSlot()
    {
        if (lastSelectedSlot == 0)
        {
            ChangeSelectedSlot(BarSlotCount - 1);
        }
        else
        {
            ChangeSelectedSlot(lastSelectedSlot - 1);
        }
    }
    private void ChangeSelectedSlot(int newSlot)
    {
        newSlot = math.clamp(newSlot, 0, BarSlotCount -1);
        UpdateSelectedSlotInBarArgs args = new UpdateSelectedSlotInBarArgs(lastSelectedSlot,newSlot);
        UpdateSelectedSlotInBar(this,args);
        lastSelectedSlot = newSlot;
    }

}
