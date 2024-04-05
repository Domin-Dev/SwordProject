using Newtonsoft.Json;
using System;
using Unity.Mathematics;
using UnityEngine;

public struct SlotPosition
{
    public int gridIndex;
    public int slotIndex;

    public SlotPosition(int gridIndex, int slotIndex)
    {
        this.gridIndex = gridIndex;
        this.slotIndex = slotIndex;
    }

    public bool Compare(SlotPosition slotPosition)
    {
        if(slotPosition.gridIndex == gridIndex && slotPosition.slotIndex == slotIndex) return true;
        else return false;
    }
}

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
public class CreateItemUIArgs : EventArgs
{
    public Sprite sprite;
    public int itemCount;
    public SlotPosition position;
    public CreateItemUIArgs(Sprite sprite,int itemCount, int gridIndex,int slotIndex)
    {
       this.sprite = sprite;
       this.itemCount = itemCount;
       this.position = new SlotPosition(gridIndex,slotIndex);
    }
}

public class MoveItemUIArgs : EventArgs
{
    public SlotPosition from;
    public SlotPosition to;

    public MoveItemUIArgs(SlotPosition from, SlotPosition to)
    {
        this.from = from;
        this.to = to;
    }
}

public class RemoveItemUIArgs : EventArgs
{
    public SlotPosition position;

    public RemoveItemUIArgs(SlotPosition position)
    {
        this.position = position;
    } 
}

public class UpdateItemCountArgs : EventArgs
{
    public SlotPosition position;
    public int count;

    public UpdateItemCountArgs(SlotPosition position, int count)
    {
        this.position = position;
        this.count = count;
    }
}

public class EquipmentManager : MonoBehaviour
{
    public event EventHandler<UpdateSelectedSlotInBarArgs> UpdateSelectedSlotInBar;
    public event EventHandler<OpenEquipmentUIArgs> OpenEquipmentUI;
    public event EventHandler<CreateItemUIArgs> CreateItemUI;
    public event EventHandler<MoveItemUIArgs> MoveItemUI;   
    public event EventHandler<RemoveItemUIArgs> RemoveItemUI;  
    public event EventHandler<UpdateItemCountArgs> UpdateItemCount;  

    private int lastSelectedSlot { get; set; } = 2;
    private bool equipmentIsOpen = false;

    public static readonly int BarSlotCount = 10;
    public static readonly int SlotCount = 30;


    public ItemStats[] equipment = new ItemStats[SlotCount];
    public ItemStats[] equipmentBar = new ItemStats[BarSlotCount];

    public SlotPosition selectedSlotInEQ { private get; set; }
    public static EquipmentManager instance { private set; get; }


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        UIManager.instance.SetUpUIEquipment(this);
        ChangeSelectedSlot(0);
        selectedSlotInEQ = new SlotPosition(-1,-1);
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

    public bool AddNewItem(ItemStats itemStats)
    {
        for (int i = 0; i < equipmentBar.Length; i++)
        {
            if (equipmentBar[i] == null)
            {
                equipmentBar[i] = itemStats;
                NewItemUI(itemStats,0,i);
                return true;
            }
        }

        for (int i = 0; i < equipment.Length; i++)
        {
            if (equipment[i] == null)
            {
                equipment[i] = itemStats;
                NewItemUI(itemStats, 1, i);
                return true;
            }
        }

        return false;
    }

    public void NewItemUI(ItemStats itemStats,int gridIndex,int slotIndex)
    {
        CreateItemUIArgs args = new CreateItemUIArgs(ItemsAsset.instance.GetIcon(itemStats.ItemID),itemStats.ItemCount,gridIndex,slotIndex);
        CreateItemUI(this,args);
    }

    public bool IsFreeSlot(SlotPosition position)
    {
        if (position.gridIndex == 0)
        {
            if(equipmentBar[position.slotIndex] == null) return true;
            Debug.Log(equipmentBar[position.slotIndex].ToString());
        }
        else if(position.gridIndex == 1)
        {
            if(equipment[position.slotIndex] == null) return true;
        }

        return false;
    }

    public void MoveSelectedItem(SlotPosition target)
    {
        if (IsFreeSlot(target))
        {
            ItemStats itemStats = GetItemStats(selectedSlotInEQ);
            ClearSlot(selectedSlotInEQ);
            SetItemStats(target, itemStats);
        }
        else if(!target.Compare(selectedSlotInEQ))
        {          
            ItemStats itemStats = GetItemStats(selectedSlotInEQ);
            ItemStats itemStatsTarget = GetItemStats(target);
            if (itemStats.ItemID != itemStatsTarget.ItemID)
            {
                SetItemStats(target, itemStats);
                SetItemStats(selectedSlotInEQ, itemStatsTarget);
                MoveItemUIArgs moveItemUIArgs = new MoveItemUIArgs(target, selectedSlotInEQ);
                MoveItemUI(this, moveItemUIArgs);
            }
            else
            {
                GetItemStats(selectedSlotInEQ).ItemCount += GetItemStats(target).ItemCount;
                UpdateCount(target);

                ClearSlot(target);
                RemoveItemUIArgs removeItemUIArgs = new RemoveItemUIArgs(target);
                RemoveItemUI(this, removeItemUIArgs);
            }    
        }
    }

    private void ClearSlot(SlotPosition position)
    {
        GetArray(position.gridIndex)[position.slotIndex] = null; 
    }     
    private ItemStats[] GetArray(int gridIndex)
    {
        switch (gridIndex)
        { 
            case 0:
                return equipmentBar;
            case 1:
                return equipment;
        }
        return null;
    }
    private ItemStats GetItemStats(SlotPosition position)
    {
        return GetArray(position.gridIndex)[position.slotIndex];
    }
    private void SetItemStats(SlotPosition position,ItemStats itemStats)
    {
        GetArray(position.gridIndex)[position.slotIndex] = itemStats;
    }
    private void UpdateCount(SlotPosition position)
    {
        UpdateItemCountArgs updateItemCountArgs = new UpdateItemCountArgs(position, GetItemStats(position).ItemCount);
        UpdateItemCount(this, updateItemCountArgs);
    }

    

}
