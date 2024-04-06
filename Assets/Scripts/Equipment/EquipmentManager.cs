using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

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

public class UpdateDragItemCountArgs : EventArgs
{
    public int count;
    public UpdateDragItemCountArgs(int count)
    {
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
    public event EventHandler<UpdateDragItemCountArgs> UpdateDragItemCount;
    public event EventHandler RemoveDragItemUI;

    private int lastSelectedSlot { get; set; } = 2;
    private bool equipmentIsOpen = false;

    public static readonly int BarSlotCount = 10;
    public static readonly int SlotCount = 30;


    public ItemStats[] equipment = new ItemStats[SlotCount];
    public ItemStats[] equipmentBar = new ItemStats[BarSlotCount];

    private SlotPosition selectedSlotInEQ;
    private ItemStats selectedItemStats;
    public PointerEventData.InputButton input;

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


    public void UnselectedSlot()
    {
        ItemStats itemStats = GetItemStats(selectedSlotInEQ);
        if(itemStats == null || itemStats.ItemID == selectedItemStats.ItemID)
        { 
            if (itemStats != null)
            {
                itemStats.ItemCount += selectedItemStats.ItemCount;
            }
            else
            {
                SetItemStats(selectedSlotInEQ, selectedItemStats);
                NewItemUI(selectedItemStats, selectedSlotInEQ);
            }
            UpdateCount(selectedSlotInEQ);
        }
        else
        {
            AddNewItem(selectedItemStats);
        }
        RemoveDragItem();
        ClearSelectedSlot();
    }

    public void ClearSelectedSlot()
    {
        selectedSlotInEQ = new SlotPosition(-1, -1);
        selectedItemStats = null;
    }
    public void SelectedSlotTakeAll(SlotPosition slotPosition)
    {
        selectedSlotInEQ = slotPosition;
        selectedItemStats = GetItemStats(slotPosition);
        ClearSlot(slotPosition);
    }

    public void SelectedSlotTakeHalf(SlotPosition slotPosition)
    {
        ItemStats itemStats = GetItemStats(slotPosition);
        selectedSlotInEQ = slotPosition;
        if(itemStats.ItemCount == 1)
        {
            SelectedSlotTakeAll(slotPosition);
            return;
        }

        int half = itemStats.ItemCount / 2;
        selectedItemStats = new ItemStats(itemStats.ItemID, itemStats.ItemCount - half);
        itemStats.ItemCount = half;

        NewItemUI(itemStats, selectedSlotInEQ);
        UpdateCount(selectedSlotInEQ);
        UpdateDragCount(selectedItemStats.ItemCount);
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
                NewItemUI(itemStats,new SlotPosition(0, i));
                return true;
            }
        }

        for (int i = 0; i < equipment.Length; i++)
        {
            if (equipment[i] == null)
            {
                equipment[i] = itemStats;
                NewItemUI(itemStats,new SlotPosition(1, i));
                return true;
            }
        }

        return false;
    }

    public void NewItemUI(ItemStats itemStats,SlotPosition slotPosition)
    {
        CreateItemUIArgs args = new CreateItemUIArgs(ItemsAsset.instance.GetIcon(itemStats.ItemID),itemStats.ItemCount,slotPosition.gridIndex,slotPosition.slotIndex);
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
            SetItemStats(target, selectedItemStats);
        }
        else if(!target.Compare(selectedSlotInEQ))
        {
            ItemStats itemStatsTarget = GetItemStats(target);
            if (selectedItemStats.ItemID != itemStatsTarget.ItemID )
            {
                if(IsFreeSlot(selectedSlotInEQ))
                {
                    SetItemStats(target, selectedItemStats);
                    SetItemStats(selectedSlotInEQ, itemStatsTarget);
                    MoveItemUIArgs moveItemUIArgs = new MoveItemUIArgs(target, selectedSlotInEQ);
                    MoveItemUI(this, moveItemUIArgs);
                }
                else
                {
                    UnselectedSlot();
                }
            }
            else 
            {     
                GetItemStats(target).ItemCount += selectedItemStats.ItemCount;
                RemoveDragItem();
                UpdateCount(target);
            }
        }
        else
        {
            if (!IsFreeSlot(selectedSlotInEQ))
            {
                GetItemStats(selectedSlotInEQ).ItemCount += selectedItemStats.ItemCount;
                RemoveDragItem();
            }
            else SetItemStats(selectedSlotInEQ, selectedItemStats);

            UpdateCount(target);
        }


        ClearSelectedSlot();
    }

    public void PutItem(SlotPosition position)
    {
        if (selectedItemStats.ItemCount >= 0)
        {
            ItemStats itemStats;
            if (IsFreeSlot(position))
            {
                itemStats = new ItemStats(selectedItemStats.ItemID, 1);
                SetItemStats(position, itemStats);
                NewItemUI(itemStats, position);
            }
            else
            {
                itemStats = GetItemStats(position);
                if (itemStats.ItemID != selectedItemStats.ItemID)
                {
                    return;
                }
                itemStats.ItemCount += 1;
            }
            selectedItemStats.ItemCount--;
            UpdateCount(position);


            if (selectedItemStats.ItemCount <= 0)
            {
                RemoveDragItem();
                ClearSelectedSlot();
            }
            else
            {
                UpdateDragCount(selectedItemStats.ItemCount);
            }
        }
    }

    public void CollectAll(SlotPosition position)
    {      
        ItemStats itemStats = GetItemStats(position);
        if(itemStats != null)
        {
            Debug.Log("juz" +
                "");
            List<SlotPosition> items = FindItems(itemStats.ItemID);
            Debug.Log(items.Count);
            foreach (SlotPosition itemPosition in items)
            {
                
                if(!itemPosition.Compare(position))
                {
                    Debug.Log(itemPosition);
                    ItemStats item = GetItemStats(itemPosition);
                    itemStats.ItemCount += item.ItemCount;
                    ClearSlot(itemPosition);
                    RemoveItem(itemPosition);
                }
            }
            UpdateCount(position);
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
       if(position.slotIndex >= 0) return GetArray(position.gridIndex)[position.slotIndex];
       else return null;
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
    private void UpdateDragCount(int count)
    {
        UpdateDragItemCountArgs updateDragItemCountArgs = new UpdateDragItemCountArgs(count);
        UpdateDragItemCount(this, updateDragItemCountArgs);
    }
    private void RemoveItem(SlotPosition position)
    {
        RemoveItemUIArgs removeItemUIArgs = new RemoveItemUIArgs(position);
        RemoveItemUI(this, removeItemUIArgs);
    }
    private void RemoveDragItem()
    {
        RemoveDragItemUI(this, null);
    }

    public bool IsNotSelected()
    {
        Debug.Log(selectedSlotInEQ.gridIndex.ToString() + " "+ selectedSlotInEQ.slotIndex.ToString());
        return selectedSlotInEQ.Compare(new SlotPosition(-1, -1));  
    }

    private List<SlotPosition> FindItems(int ItemId)
    {
        List<SlotPosition> items = new List<SlotPosition>();

        for (int i = 0; i < equipmentBar.Length; i++)
        {
           
            if (equipmentBar[i] != null && equipmentBar[i].ItemID == ItemId)
            {
                items.Add(new SlotPosition(0,i));
            }
        }

        for (int i = 0; i < equipment.Length; i++)
        {
            if (equipment[i] != null && equipment[i].ItemID == ItemId)
            {
                items.Add(new SlotPosition(1, i));
            }
        }

        return items;
    }
}
