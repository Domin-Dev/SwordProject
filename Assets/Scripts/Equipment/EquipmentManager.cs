using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

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

    public static SlotPosition NullSlot = new SlotPosition(-1, -1);

    public override string ToString()
    {
        return $"Grid Index:{gridIndex},Slot Index:{slotIndex}";
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
    public bool isDrag;
    public CreateItemUIArgs(Sprite sprite,int itemCount,SlotPosition slotPosition,bool isDrag)
    {
       this.isDrag = isDrag;
       this.sprite = sprite;
       this.itemCount = itemCount;
       this.position = slotPosition;
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

public class MoveMainBarItemArgs : EventArgs
{
    public SlotPosition from;
    public SlotPosition to;
    public MoveMainBarItemArgs(SlotPosition from, SlotPosition to)
    {
        this.from = from;
        this.to = to;   
    }
}

public class RemoveMainBarItemArgs : EventArgs
{
    public SlotPosition position;
    
    public RemoveMainBarItemArgs(SlotPosition position)
    {
        this.position = position;
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

    public event EventHandler<MoveMainBarItemArgs> MoveMainBarItem;
    public event EventHandler<CreateItemUIArgs> CreateMainBarItem;
    public event EventHandler<RemoveItemUIArgs> RemoveMainBarItem;
    public event EventHandler<UpdateItemCountArgs> UpdateMainBarItemCount;
   

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
        if(itemStats == null || itemStats.itemID == selectedItemStats.itemID)
        { 
            if (itemStats != null)
            {
                // itemStats.itemCount += selectedItemStats.itemCount;
                PutItems(selectedItemStats, SlotPosition.NullSlot, selectedSlotInEQ);
                if (selectedSlotInEQ.gridIndex == 0) UpdateMainItemCount(selectedSlotInEQ, GetItemStats(selectedSlotInEQ).itemCount);
            }
            else
            {
                SetItemStats(selectedSlotInEQ, selectedItemStats);
                if (selectedSlotInEQ.gridIndex == 0) NewMainBarItemUI(GetItemStats(selectedSlotInEQ),selectedSlotInEQ);
                NewItemUI(selectedItemStats, selectedSlotInEQ, true);
            }
        } 
        else
        {
            AddNewItem(selectedItemStats);
        }

        UpdateCount(selectedSlotInEQ);
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
        if(slotPosition.gridIndex == 0)  RemoveMainBarItemUI(slotPosition);
    }
    public void SelectedSlotTakeHalf(SlotPosition slotPosition)
    {
        ItemStats itemStats = GetItemStats(slotPosition);
        selectedSlotInEQ = slotPosition;
        if(itemStats.itemCount == 1)
        {
            SelectedSlotTakeAll(slotPosition);
            return;
        }

        int half = itemStats.itemCount / 2;
        selectedItemStats = new ItemStats(itemStats.itemID, itemStats.itemCount - half);
        itemStats.itemCount = half;

        NewItemUI(itemStats, selectedSlotInEQ, true);
        UpdateCount(selectedSlotInEQ);
        UpdateDragCount(selectedItemStats.itemCount);
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
        if (itemStats.itemCount > 0)
        {
            List<SlotPosition> itemList = FindItems(itemStats.itemID);
            int stackMax = ItemsAsset.instance.GetStackMax(itemStats.itemID);

            if (itemList.Count > 0)
            {
                for (int i = 0; i < itemList.Count; i++)
                {
                    ItemStats item = GetItemStats(itemList[i]);
                    int free = stackMax - item.itemCount;
                    if (free > 0)
                    {
                        if (itemStats.itemCount > free)
                        {
                            itemStats.itemCount -= free;
                            IncreaseItemCount(itemList[i], free);
                        }
                        else
                        {
                            IncreaseItemCount(itemList[i], itemStats.itemCount);
                            return true;
                        }

                    }
                }
            }

            for (int i = 0; i < equipmentBar.Length; i++)
            {
                if (equipmentBar[i] == null)
                {
                    if (itemStats.itemCount > stackMax)
                    {
                        itemStats.itemCount -= stackMax;
                        ItemStats newItem = new ItemStats(itemStats.itemID, stackMax);
                        equipmentBar[i] = newItem;
                        NewItemUI(newItem, new SlotPosition(0, i), false);
                    }
                    else
                    {
                        equipmentBar[i] = itemStats;
                        NewItemUI(itemStats, new SlotPosition(0, i), false);
                        return true;
                    }
                }
            }

            for (int i = 0; i < equipment.Length; i++)
            {
                if (equipment[i] == null)
                {
                    if (itemStats.itemCount > stackMax)
                    {
                        itemStats.itemCount -= stackMax;
                        ItemStats newItem = new ItemStats(itemStats.itemID, stackMax);
                        equipment[i] = newItem;
                        NewItemUI(newItem, new SlotPosition(1, i), false);
                    }
                    else
                    {
                        equipment[i] = itemStats;
                        NewItemUI(itemStats, new SlotPosition(1, i), false);
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public void NewItemUI(ItemStats itemStats,SlotPosition slotPosition,bool isDrag)
    {
        CreateItemUIArgs args = new CreateItemUIArgs(ItemsAsset.instance.GetIcon(itemStats.itemID),itemStats.itemCount,slotPosition,isDrag);
        CreateItemUI(this,args);
    }
    public void NewMainBarItemUI(ItemStats itemStats, SlotPosition slotPosition)
    {
        CreateItemUIArgs args = new CreateItemUIArgs(ItemsAsset.instance.GetIcon(itemStats.itemID), itemStats.itemCount, slotPosition, false);
        CreateMainBarItem(this, args);
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
            if (target.gridIndex == 0)
            {
                NewMainBarItemUI(selectedItemStats, target);
            }
        }
        else if(!target.Compare(selectedSlotInEQ))
        {
            ItemStats itemStatsTarget = GetItemStats(target);
            if (selectedItemStats.itemID != itemStatsTarget.itemID )
            {
                if(IsFreeSlot(selectedSlotInEQ))
                {
                    SetItemStats(target, selectedItemStats);
                    SetItemStats(selectedSlotInEQ, itemStatsTarget);
                    MoveItemUIArgs moveItemUIArgs = new MoveItemUIArgs(target, selectedSlotInEQ);
                    MoveItemUI(this, moveItemUIArgs);
                    if(selectedSlotInEQ.gridIndex == 0) NewMainBarItemUI(selectedItemStats,target);                    
                }
                else
                {
                    UnselectedSlot();
                }
            }
            else 
            {
                PutItems(selectedItemStats, new SlotPosition(-1,-1), target);
                RemoveDragItem();
            }
        }
        else
        {
            if (!IsFreeSlot(selectedSlotInEQ))
            {
                PutItems(selectedItemStats, SlotPosition.NullSlot, target);
            //    GetItemStats(selectedSlotInEQ).itemCount += selectedItemStats.itemCount;
                RemoveDragItem();
            }
            else SetItemStats(selectedSlotInEQ, selectedItemStats);

            UpdateCount(target);
        }
        ClearSelectedSlot();
    }
    public void PutOneItem(SlotPosition position)
    {
        int stackMax = ItemsAsset.instance.GetStackMax(selectedItemStats.itemID);
        if (selectedItemStats.itemCount >= 0)
        {
            ItemStats itemStats;
            if (IsFreeSlot(position))
            {
                itemStats = new ItemStats(selectedItemStats.itemID, 1);
                SetItemStats(position, itemStats);
                NewItemUI(itemStats, position, false);
            }
            else
            {
                itemStats = GetItemStats(position);
                if (itemStats.itemID != selectedItemStats.itemID || itemStats.itemCount >= stackMax)
                {
                    return;
                }
                itemStats.itemCount += 1;
            }
            selectedItemStats.itemCount--;
            UpdateCount(position);


            if (selectedItemStats.itemCount <= 0)
            {
                RemoveDragItem();
                ClearSelectedSlot();
            }
            else
            {
                UpdateDragCount(selectedItemStats.itemCount);  
            }
        }
    }
    private void PutItems(ItemStats item,SlotPosition lastPosition, SlotPosition target)
    {
        ItemStats itemStats = GetItemStats(target);
        if(itemStats == null)
        {
            Debug.Log(target.ToString());
            itemStats = SetItemStats(target, new ItemStats(item.itemID,0));
            NewItemUI(itemStats,target, false);
        }

        int stackMax = ItemsAsset.instance.GetStackMax(item.itemID);
        int free = stackMax - itemStats.itemCount;

        if (free > 0)
        {
            if (free >= item.itemCount)
            {
                IncreaseItemCount(target, item.itemCount);
            }
            else
            {
                item.itemCount -= free;
                IncreaseItemCount(target, free);
                FindSlotForIt(item, lastPosition);
            }
        }
        else
        {
            FindSlotForIt(item, lastPosition);
        }
    }
    private void FindSlotForIt(ItemStats itemStats,SlotPosition lastPosition)
    {
        if (itemStats != null)
        {
            ItemStats item = GetItemStats(lastPosition);

            if (!lastPosition.Compare(SlotPosition.NullSlot) && ((item != null && item.itemID == itemStats.itemID) || item == null))
            {
                PutItems(itemStats, new SlotPosition(-1, -1), lastPosition);
            } 
            else
            {
                AddNewItem(itemStats);           
            }
        }
    }
    public void CollectAll(SlotPosition position)
    {        
        ItemStats itemStats = GetItemStats(position);
        if (itemStats != null)
        {
            int stackMax = ItemsAsset.instance.GetStackMax(itemStats.itemID);
            int free = stackMax - itemStats.itemCount;

            if (free <= 0) return;
        
            List<SlotPosition> items = FindItems(itemStats.itemID);
            foreach (SlotPosition itemPosition in items)
            {
                if (!itemPosition.Compare(position))
                {
                    ItemStats item = GetItemStats(itemPosition);
                    if (item.itemCount == stackMax) continue;
                    if (item.itemCount > free)
                    {
                        itemStats.itemCount += free;
                        item.itemCount -= free;
                        UpdateCount(itemPosition);
                        break;
                    }
                    else
                    {
                        itemStats.itemCount += item.itemCount;
                        free -= item.itemCount;
                        ClearSlot(itemPosition);
                        RemoveItem(itemPosition);
                        if (free == 0) break;
                    }
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
    public ItemStats GetItemStatsValue(SlotPosition position)
    {
        if (position.slotIndex >= 0)
        {
            ItemStats itemStats = GetArray(position.gridIndex)[position.slotIndex];
            return new ItemStats(itemStats);
        }
        else return null;
    }
    private ItemStats SetItemStats(SlotPosition position,ItemStats itemStats)
    {
      return GetArray(position.gridIndex)[position.slotIndex] = itemStats;
    }
    private void UpdateCount(SlotPosition position)
    {
        UpdateItemCountArgs updateItemCountArgs = new UpdateItemCountArgs(position, GetItemStats(position).itemCount);  
        UpdateItemCount(this, updateItemCountArgs);
    }
    private void UpdateMainItemCount(SlotPosition position,int count)
    {
        UpdateItemCountArgs updateItemCountArgs = new UpdateItemCountArgs(position, count);
        UpdateMainBarItemCount(this, updateItemCountArgs);
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
        RemoveDragItemUI(this,null);
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
            if (equipmentBar[i] != null && equipmentBar[i].itemID == ItemId)
            {
                items.Add(new SlotPosition(0,i));
            }
        }

        for (int i = 0; i < equipment.Length; i++)
        {
            if (equipment[i] != null && equipment[i].itemID == ItemId)
            {
                items.Add(new SlotPosition(1, i));
            }
        }

        return items;
    }
    private void RemoveMainBarItemUI(SlotPosition position)
    {
        RemoveMainBarItem(this, new RemoveItemUIArgs(position));
    }
    private void IncreaseItemCount(SlotPosition position,int value) 
    {
        GetItemStats(position).itemCount += value;
        UpdateCount(position);
    }
}