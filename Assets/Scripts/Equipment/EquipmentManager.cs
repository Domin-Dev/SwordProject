using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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

    public static SlotPosition NullSlot = new SlotPosition(-1, -1);

    public override string ToString()
    {
        return $"Grid Index:{gridIndex},Slot Index:{slotIndex}";
    }

}
public class TooltipInfo
{
    public string content;
    public string header;
    public TooltipInfo(string content,string header)
    {
        this.content = content;
        this.header = header;
    }

    public TooltipInfo(string content)
    {
        this.content = content;
        this.header = "";
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
public class BoolArgs : EventArgs
{
    public bool value;
    public BoolArgs(bool value)
    {
        this.value = value;
    }
}
public class CreateItemArgs : EventArgs
{
    public ItemStats itemStats;
    public SlotPosition position;
    public bool isDrag;
    public CreateItemArgs(ItemStats itemStats,SlotPosition slotPosition,bool isDrag)
    {
       this.isDrag = isDrag;
       this.itemStats = itemStats;
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
public class PositionArgs : EventArgs
{
    public SlotPosition position;

    public PositionArgs(SlotPosition position)
    {
        this.position = position;
    } 
}
public class UpdateItemCountArgs : PositionArgs
{
    public int count;

    public UpdateItemCountArgs(SlotPosition position, int count):base(position)
    {
        this.count = count;
    }
}
public class ItemCountArgs : EventArgs
{
    public int count;
    public ItemCountArgs(int count)
    {
        this.count = count;
    }
}
public class MoveItemArgs : EventArgs
{
    public SlotPosition from;
    public SlotPosition to;
    public MoveItemArgs(SlotPosition from, SlotPosition to)
    {
        this.from = from;
        this.to = to;   
    }
}
public class ItemStatsArgs : EventArgs
{
    public ItemStats item;
    
    public ItemStatsArgs(ItemStats item)
    {
        this.item = item;
    }
}
public class LifeBarArgs : PositionArgs
{
   public float barValue;
    public LifeBarArgs(SlotPosition slotPosition, float barValue) : base(slotPosition)
    {
        this.barValue = barValue;
    }
}
public class SetAmmoBarArgs : EventArgs
{
    public int magazineCapacity;
    public int currentCount;
    public AmmoType type;

    public SetAmmoBarArgs(int magazineCapacity,int currentCount, AmmoType ammoType)
    {
        this.magazineCapacity = magazineCapacity;
        this.currentCount = currentCount;
        this.type = ammoType;
    }
}
public class UpdateAmmoBarArgs : EventArgs
{
    public int currentCount;

    public UpdateAmmoBarArgs(int currentCount)
    {
        this.currentCount = currentCount;
    }
}

public class PlaceholderArgs : EventArgs
{
    public bool turn;
    public SlotPosition slotPosition;

    public PlaceholderArgs(bool turn, SlotPosition slotPosition)
    {
        this.turn = turn;
        this.slotPosition = slotPosition;
    }
}

public class EquipmentManager : MonoBehaviour
{
    //UI
    public event EventHandler<UpdateSelectedSlotInBarArgs> UpdateSelectedSlotInBar;
    public event EventHandler<BoolArgs> OpenEquipmentUI;

    public event EventHandler<CreateItemArgs> CreateItemUI;
    public event EventHandler<MoveItemUIArgs> MoveItemUI;   
    public event EventHandler<PositionArgs> RemoveItemUI;  
    public event EventHandler<UpdateItemCountArgs> UpdateItemCount;

    public event EventHandler<ItemCountArgs> UpdateDragItemCount;
    public event EventHandler RemoveDragItemUI;

    public event EventHandler<MoveItemArgs> MoveMainBarItem;
    public event EventHandler<CreateItemArgs> CreateMainBarItem;
    public event EventHandler<PositionArgs> RemoveMainBarItem;
    public event EventHandler<UpdateItemCountArgs> UpdateMainBarItemCount;

    public event EventHandler<PlaceholderArgs> TurnPlaceholder;

    public event EventHandler<LifeBarArgs> UpdateItemLifeBar;

    public event EventHandler<ItemStatsArgs> UpdateItemInHand;

    private int slotInHand { get; set; } = 1;
    private int ammoCount;
    private int ammoID;

    private bool equipmentIsOpen = false;

    public static readonly int BarSlotCount = 10;
    public static readonly int SlotCount = 30;
    public static readonly int clothesCount = 8;

    private ItemStats[] equipment = new ItemStats[SlotCount];
    private ItemStats[] equipmentBar = new ItemStats[BarSlotCount];
    private ItemStats[] clothes = new ItemStats[clothesCount];

    private SlotPosition selectedSlotInEQ;
    private ItemStats selectedItemStats;
    [HideInInspector]public PointerEventData.InputButton input;

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
        selectedSlotInEQ = new SlotPosition(-1,-1);
        ChangeSelectedSlot(0);
    }

    private void Update()
    {
        if (!ChatManager.instance.isChatting)
        {
            if      (Input.GetKeyDown(KeyCode.Alpha1)) ChangeSelectedSlot(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeSelectedSlot(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeSelectedSlot(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeSelectedSlot(3);
            else if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeSelectedSlot(4);
            else if (Input.GetKeyDown(KeyCode.Alpha6)) ChangeSelectedSlot(5);
            else if (Input.GetKeyDown(KeyCode.Alpha7)) ChangeSelectedSlot(6);
            else if (Input.GetKeyDown(KeyCode.Alpha8)) ChangeSelectedSlot(7);
            else if (Input.GetKeyDown(KeyCode.Alpha9)) ChangeSelectedSlot(8);
            else if (Input.GetKeyDown(KeyCode.Alpha0)) ChangeSelectedSlot(9);


            if (Input.GetKeyDown(KeyCode.I))
            {
                equipmentIsOpen = !equipmentIsOpen;
                BoolArgs args = new BoolArgs(equipmentIsOpen);
                OpenEquipmentUI(this, args);
            }

            if (Input.mouseScrollDelta.y > 0)
            {
                NextSlot();
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                BackSlot();
            }
        }    
    }
    public void SetUpEvent(HandsController handsController)
    {
        handsController.UseItem += UseSelectedItem;
        BuildingManager.instance.builtObject += BuiltObject;
    }

    private void BuiltObject(object sender, EventArgs e)
    {
        if(DecreaseItemCount(new SlotPosition(0, slotInHand), 1) <= 0)
        UpdateItemInHand(this, new ItemStatsArgs(equipmentBar[slotInHand]));
    }

    private void UseSelectedItem(object sender, EventArgs e)
    {
        DestroyableItem item = equipmentBar[slotInHand] as DestroyableItem;
        if (item != null)
        {
            item.Use();
            UpdateItemLifeBar(this, new LifeBarArgs(new SlotPosition(0,slotInHand), item.GetLifePointsInPercent()));
        } 
    }
    public void UnselectedSlot()
    {
        ItemStats itemStats = GetItemStats(selectedSlotInEQ);
        if(itemStats == null || itemStats.itemID == selectedItemStats.itemID)
        { 
            if (itemStats != null)
            {
                PutItems(selectedItemStats, SlotPosition.NullSlot, selectedSlotInEQ);
                if (selectedSlotInEQ.gridIndex == 0) UpdateMainItemCount(selectedSlotInEQ, GetItemStats(selectedSlotInEQ).itemCount);
            }
            else
            {
                Debug.Log(selectedSlotInEQ.gridIndex);
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
        if(slotInHand == BarSlotCount - 1)
        {
            ChangeSelectedSlot(0);
        }
        else
        {
            ChangeSelectedSlot(slotInHand + 1);
        }
    }
    private void BackSlot()
    {
        if (slotInHand == 0)
        {
            ChangeSelectedSlot(BarSlotCount - 1);
        }
        else
        {
            ChangeSelectedSlot(slotInHand - 1);
        }
    }
    private void ChangeSelectedSlot(int newSlot)
    {
        newSlot = math.clamp(newSlot, 0, BarSlotCount -1);
        if (slotInHand != newSlot)
        {
            UpdateSelectedSlotInBarArgs args = new UpdateSelectedSlotInBarArgs(slotInHand, newSlot);
            UpdateSelectedSlotInBar(this, args);
            slotInHand = newSlot;
            UpdateItemInHand(this, new ItemStatsArgs(equipmentBar[newSlot]));    
        }
    }
    public bool AddNewItem(ItemStats itemStats)
    {
        if (itemStats.itemCount > 0)
        {
            UIManager.instance.NewCollectedItem(itemStats);

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
                            UIManager.instance.CheckRecipesWithItem(itemStats.itemID,true);
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
                        ItemStats newItem = itemStats.Clon();
                        newItem.itemCount = stackMax;

                        equipmentBar[i] = newItem;
                        NewItemUI(newItem, new SlotPosition(0, i), false);
                    }
                    else
                    {
                        equipmentBar[i] = itemStats;
                        NewItemUI(itemStats, new SlotPosition(0, i), false);
                        UIManager.instance.CheckRecipesWithItem(itemStats.itemID, true);
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
                        ItemStats newItem = itemStats.Clon();
                        newItem.itemCount = stackMax;
                    
                        equipment[i] = newItem;
                        NewItemUI(newItem, new SlotPosition(1, i), false);
                    }
                    else
                    {
                        equipment[i] = itemStats;
                        NewItemUI(itemStats, new SlotPosition(1, i), false);
                        UIManager.instance.CheckRecipesWithItem(itemStats.itemID, true);
                        return true;
                    }
                }
            }

        }
        return false;
    }
    public void NewItemUI(ItemStats itemStats,SlotPosition slotPosition,bool isDrag)
    {
        if (slotPosition.Compare(new SlotPosition(0, slotInHand)))
        {
            UpdateItemInHand(this, new ItemStatsArgs(equipmentBar[slotInHand]));
        }

        CreateItemArgs args = new CreateItemArgs(itemStats,slotPosition,isDrag);
        CreateItemUI(this,args);
    }
    public void NewMainBarItemUI(ItemStats itemStats, SlotPosition slotPosition)
    {
        CreateItemArgs args = new CreateItemArgs(itemStats, slotPosition, false);
        CreateMainBarItem(this, args);
    }
    public bool IsFreeSlot(SlotPosition position)
    {
        var grid = GetGrid(position.gridIndex);
        if(grid != null)
        {
            return grid[position.slotIndex] == null; 
        }
        return false;
    }   
    private bool IsSlotInHand(SlotPosition slotPosition)
    {
        return slotPosition.Compare(new SlotPosition(0, slotInHand));
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
            else if(target.gridIndex == 2)
            {
                TurnPlaceholder(this, new PlaceholderArgs(false, target));
            }
        }
        else if(!target.Compare(selectedSlotInEQ))
        {
            ItemStats itemStatsTarget = GetItemStats(target);
            int maxStack = ItemsAsset.instance.GetStackMax(itemStatsTarget.itemID);
            if(selectedItemStats.itemID != itemStatsTarget.itemID || maxStack == 1)
            {  
                if(IsFreeSlot(selectedSlotInEQ))
                {                   
                    SetItemStats(target, selectedItemStats);
                    SetItemStats(selectedSlotInEQ, itemStatsTarget);
                    MoveItemUIArgs moveItemUIArgs = new MoveItemUIArgs(target, selectedSlotInEQ);
                    MoveItemUI(this, moveItemUIArgs); 
                    if(target.gridIndex == 0)
                    {
                        NewMainBarItemUI(selectedItemStats, target);
                    }
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
                RemoveDragItem();
            }
            else SetItemStats(selectedSlotInEQ, selectedItemStats);

            UpdateCount(target);
        }


        if (IsSlotInHand(target) || IsSlotInHand(selectedSlotInEQ))
        {
            UpdateItemInHand(this, new ItemStatsArgs(GetItemStats(new SlotPosition(0, slotInHand))));
        }

        if(IsFreeSlot(selectedSlotInEQ) &&  selectedSlotInEQ.gridIndex == 2)
        {
            TurnPlaceholder(this, new PlaceholderArgs(true,selectedSlotInEQ));
        }

        ClearSelectedSlot();
    }
    public void PutOneItem(SlotPosition position)
    {
        int stackMax = ItemsAsset.instance.GetStackMax(selectedItemStats.itemID);
        if (selectedItemStats.itemCount > 0)
        {
            ItemStats itemStats;
            if (IsFreeSlot(position))
            {
                itemStats = selectedItemStats.Clon();
                itemStats.itemCount = 1;
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
        GetGrid(position.gridIndex)[position.slotIndex] = null; 
    }     
    private ItemStats[] GetGrid(int gridIndex)
    {
        switch (gridIndex)
        { 
            case 0:
                return equipmentBar;
            case 1:
                return equipment;
            case 2:
                return clothes;
        }
        return null;
    }
    private ItemStats GetItemStats(SlotPosition position)
    {
       if(position.slotIndex >= 0) return GetGrid(position.gridIndex)[position.slotIndex];
       else return null;
    }
    public ItemStats GetItemStatsValue(SlotPosition position)
    {
        if (position.slotIndex >= 0)
        {
            ItemStats itemStats = GetGrid(position.gridIndex)[position.slotIndex];
            return itemStats.Clon();
        }
        else return null;
    }
    private ItemStats SetItemStats(SlotPosition position,ItemStats itemStats)
    {
      return GetGrid(position.gridIndex)[position.slotIndex] = itemStats;
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
        ItemCountArgs updateDragItemCountArgs = new ItemCountArgs(count);
        UpdateDragItemCount(this, updateDragItemCountArgs);
    }
    private void RemoveItem(SlotPosition position)
    {
        PositionArgs removeItemUIArgs = new PositionArgs(position);
        RemoveItemUI(this, removeItemUIArgs);
    }
    private void RemoveDragItem()
    {
        RemoveDragItemUI(this,null);
    }
    public bool IsNotSelected()
    {
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
        RemoveMainBarItem(this, new PositionArgs(position));
    }
    private void IncreaseItemCount(SlotPosition position,int value) 
    {
        GetItemStats(position).itemCount += value;
        UpdateCount(position);
    }

    private void DecreaseItemCount(int itemID,int value = 1)
    {
        var list = FindItems(itemID);
        for (int i = 0; i < list.Count; i++)
        {
            int balance = DecreaseItemCount(list[i], value);
            if (balance < 0) value = -balance;
            else return;
        }
    }
    private int DecreaseItemCount(SlotPosition position, int value = 1)
    {
        ItemStats itemStats = GetItemStats(position);
        int balance = itemStats.itemCount - value;
        if (balance <= 0)
        {
            ClearSlot(position);
            RemoveItem(position);
            UIManager.instance.CheckRecipesWithItem(itemStats.itemID,false);
            return balance;
        }
        itemStats.itemCount = balance;
        UpdateCount(position);
        UIManager.instance.CheckRecipesWithItem(itemStats.itemID, false);
        return 1;
    }
    private SlotPosition Find(int itemId)
    {
        for (int i = 0; i < equipmentBar.Length; i++)
        {
            if (equipmentBar[i] != null && equipmentBar[i].itemID == itemId)
            {
                return new SlotPosition(0, i);
            }
        }

        for (int i = 0; i < equipment.Length; i++)
        {
            if (equipment[i] != null && equipment[i].itemID == itemId)
            {
                return new SlotPosition(1, i);
            }
        }

        return SlotPosition.NullSlot;
    }
    public TooltipInfo GetTooltipInfo(SlotPosition position)
    {
        ItemStats itemStats = GetItemStats(position);
        if (itemStats == null) return null;
        else
        {
            return ItemsAsset.instance.GetTooltipInfo(itemStats.itemID);
        }
    }
    public bool CountAmmo(RangedWeaponItem rangedWeaponItem)
    {
        ammoID = ItemsAsset.instance.GetAmmoID(rangedWeaponItem.itemID);
        List<SlotPosition> ammoList = FindItems(ammoID);
        ammoCount = 0;
        for (int i = 0; i < ammoList.Count; i++)
        {
            ammoCount += GetItemStats(ammoList[i]).itemCount;
        }
        return ammoCount > 0;
    }
    public int CountItems(int id)
    {
        var items = FindItems(id);
        int counter = 0;
        foreach (var item in items)
        {
            counter += GetItemStats(item).itemCount;
        }
        return counter;
    }

    public int Reload()
    {
        DecreaseItemCount(Find(ammoID));
        ammoCount--;
        return ammoCount;
    } 
    public Dictionary<int,int> GetItemDictionary()
    {
        Dictionary<int,int> items = new Dictionary<int,int>();
        AddItemsToDictionary(items, equipment);
        AddItemsToDictionary(items, equipmentBar);
        return items;
    }
    private void AddItemsToDictionary(Dictionary<int,int> items, ItemStats[] itemStats)
    {
        foreach (var item in itemStats)
        {
            if (item != null)
            {
                if (items.ContainsKey(item.itemID))
                {
                    items[item.itemID] += item.itemCount;
                }
                else
                {
                    items.Add(item.itemID, item.itemCount);
                }
            }
        }
    }
    public void Craft(int itemID)
    {
        Item item = ItemsAsset.instance.GetItem(itemID);
        ItemStats itemStats = item.GetItemStats();
        itemStats.itemCount = item.numberItem;
        AddNewItem(itemStats);
        for (int i = 0; i < item.crafingIngredients.Length; i++)
        {
            Item.CrafingIngredient ingredient = item.crafingIngredients[i];
            DecreaseItemCount(ingredient.itemID, ingredient.number);
        }
    }

}
