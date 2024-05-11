using UnityEngine.UI;
using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;
using TMPro;
using System;
using UnityEditor;
using System.Collections.Generic;

public class EquipmentGrid
{
    public Transform gridTransform { private set; get; }
    public int gridIndex {private set; get; }

    public EquipmentGrid(Transform gridTransform, int gridIndex)
    {
        this.gridTransform = gridTransform;
        this.gridIndex = gridIndex;
    }
}
public class UIManager : MonoBehaviour
{
    //black background
    [SerializeField] private Transform background;
    [Space(30f)]
    //equipment
    [SerializeField] private Canvas mainCanvas;
    [Header("Gun Info UI")]
    [SerializeField] private Transform ammoBar;
    [SerializeField] private GameObject ammoUI;
    [Space]
    #region Equipment UI
    [Header("Equipment UI")]
    [SerializeField] private Transform mainItemBar;
    [SerializeField] private Transform equipment;
    [Space]
    [SerializeField] private GameObject itemSlot;
    [SerializeField] private GameObject slotIndex;
    [SerializeField] private GameObject item;
    [SerializeField] private GameObject lifePointsBar;
    [Space]
    
    [SerializeField] private Transform equipmentItemSlots;
    [SerializeField] private Transform equipmentItemBar;
    [SerializeField] private Transform equipmentDragItems;
    [Space(30f)]
    #endregion
    #region Building UI
    [Header("Building UI")]
    [SerializeField] private Transform building;
    [SerializeField] private Transform buildingObjectGrid;
    [Space]
    [SerializeField] private GameObject buildingObject;
    [Space(30f)]
    #endregion
    [Header("Stats UI")]
    [SerializeField] private Button ServerButton;
    [SerializeField] private Button HostButton;
    [SerializeField] private Button ClientButton;
    [SerializeField] Sprite selected;
    [SerializeField] Sprite unSelected;

    private const float buttonScale = 1f;
    private const float selectedButtonScale = 1.1f;

    private const float speedSelecting = 10f;
    private const float speedUnselecting = 15f;

    public Transform itemParent { get { return equipmentDragItems; } }
    public static UIManager instance { private set; get; }


    private EquipmentGrid barGrid;
    private EquipmentGrid equipmentBarGrid;
    private EquipmentGrid mainEquipmentGrid;

    private List<Transform> openWindows = new List<Transform>();
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);


        SetGrids();
        SetUpNetworkUI();
        SetBuildingObjects();
    }
    private void Update()
    {
        UpdateButtonSize();
    }


    private void SetBuildingObjects()
    {
        BuildingObject[] array = ItemsAsset.instance.GetBuildingObjects();
        for (int i = 0; i < array.Length; i++)
        {
            Transform obj = Instantiate(buildingObject, buildingObjectGrid).transform;
            obj.GetChild(0).GetComponent<Image>().sprite = array[i].icon;
            obj.GetComponent<BuildingObjectUI>().ID = array[i].ID;
        }
    }
    private void SetGrids()
    {
        barGrid = new EquipmentGrid(mainItemBar, 0);
        equipmentBarGrid = new EquipmentGrid(equipmentItemBar, 0);

        mainEquipmentGrid = new EquipmentGrid(equipmentItemSlots, 1);
    }
    private void SetUpNetworkUI()
    {
        ServerButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });
        HostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
        ClientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }
    public void SetUpUIEquipment(EquipmentManager eqManager)
    {
        eqManager.UpdateSelectedSlotInBar += UpdateSelectedSlot;

        eqManager.OpenEquipmentUI += OpenEquipment;

        eqManager.CreateItemUI += CreateItemUI;
        eqManager.MoveItemUI += MoveItemUI;
        eqManager.RemoveItemUI += RemoveItemUI;
        eqManager.UpdateItemCount += UpdateItemCount;
        eqManager.UpdateDragItemCount += UpdateDragItemCount;
        eqManager.RemoveDragItemUI += RemoveDragItemUI;
        eqManager.MoveMainBarItem += MoveMainBarItem;
        eqManager.CreateMainBarItem += CreateMainBarItem;
        eqManager.RemoveMainBarItem += RemoveMainBarItem;
        eqManager.UpdateMainBarItemCount += UpdateMainBarItemCount;
        eqManager.UpdateItemLifeBar += UpdateItemLifeBar;
       

        LoadSlots(mainEquipmentGrid,EquipmentManager.SlotCount, false);
        LoadSlots(equipmentBarGrid,EquipmentManager.BarSlotCount, true);
        LoadSlots(barGrid,EquipmentManager.BarSlotCount,true);
        
    }
    public void SetUpUIPlayer(HandsController handsController)
    {
        handsController.SetAmmoBar += SetAmmoBar;
        handsController.UpdateAmmoBar += UpdateAmmoBar;
        handsController.HideAmmoBar += HideAmmoBar;
        handsController.SwitchBuildingUI += SwitchBuildingUI;
    }
    public void SetUpUIBuilding(BuildingManager buildingManager)
    {
        buildingManager.SwitchBuildingUI += SwitchBuildingUI;
    }
    private void HideAmmoBar(object sender, EventArgs e)
    {
        ammoBar.gameObject.SetActive(false);
    }
    private void UpdateAmmoBar(object sender, UpdateAmmoBarArgs e)
    {
        if(lastAmmoCount > e.currentCount)
        { 
            for(int i = lastAmmoCount -1;i >= e.currentCount;i--)
            {
                Image ammo = ammoBar.GetChild(i).GetComponent<Image>();
                ammo.color = Color.black;
                Debug.Log(i);
            }
        }
        else if (lastAmmoCount < e.currentCount)
        {
            int i;
            if (lastAmmoCount > 0) i = lastAmmoCount - 1;
            else i = 0;
            
            for (;i < e.currentCount; i++)
            {
                Image ammo = ammoBar.GetChild(i).GetComponent<Image>();
                ammo.color = Color.white;
            }
        }
        lastAmmoCount = e.currentCount;
    }

    private int lastAmmoCount = -1;
    private void SetAmmoBar(object sender, SetAmmoBarArgs e)
    {
        ammoBar.gameObject.SetActive(true);
        lastAmmoCount = e.currentCount;
        Sprite sprite = ItemsAsset.instance.GetAmmoSpriteUI(e.type);
        for (int i = 0; i < ammoBar.childCount; i++)
        {
            Image ammo= ammoBar.GetChild(i).GetComponent<Image>();
           
            if(i < e.magazineCapacity)
            {
                ammo.gameObject.SetActive(true);
                ammo.sprite = sprite;
                if(i < e.currentCount) ammo.color = Color.white;
                else ammo.color = Color.black;           
            }
            else
                ammo.gameObject.SetActive(false);
            
        }
        int toAdd = e.magazineCapacity - ammoBar.childCount;
        if (toAdd > 0)
        {
            for (int i = 0; i < toAdd; i++)
            {
                Image ammo = Instantiate(ammoUI, ammoBar).GetComponent<Image>();
                ammo.sprite = sprite;
                if (ammoBar.childCount <= e.currentCount) ammo.color = Color.white;
                else ammo.color = Color.black;
            }
        }
    }
    private void UpdateItemLifeBar(object sender, LifeBarArgs e)
    {
        Transform slot = GetItem(e.position);
        FindBar(e.barValue,slot);
        if(e.position.gridIndex == 0)
        {
            slot = GetItemFromMainBar(e.position);
            FindBar(e.barValue, slot);
        }
    }
    private void FindBar(float value,Transform slot)
    {
        if (slot == null) return;
        for (int i = 0; i < slot.childCount; i++)
        {
            if (slot.GetChild(i).CompareTag("Bar"))
            {
                UpdateBar(value, slot.GetChild(i).transform.GetChild(0));
                return;
            }
        }
    }
    private void UpdateBar(float value, Transform barTransform)
    {
        Image bar = barTransform.GetComponent<Image>();
        bar.transform.localScale = new Vector3(value, 1,1);
        bar.color = new Color(bar.color.r, value, bar.color.b);
    }
    private void UpdateMainBarItemCount(object sender, UpdateItemCountArgs e)
    {
        UpdateCount(mainItemBar, e);
    }
    private void RemoveMainBarItem(object sender, PositionArgs e)
    {
        RemoveItem(mainItemBar, e);
    }
    private void CreateMainBarItem(object sender, CreateItemArgs e)
    {
        NewItemUI(mainItemBar, e);
    }
    private void MoveMainBarItem(object sender, MoveItemArgs e)
    {
        if(e.from.gridIndex == 0 && e.to.gridIndex == 0)
        {
            DragDrop slot = mainItemBar.GetChild(e.from.slotIndex).GetComponentInChildren<DragDrop>();
            slot.transform.SetParent(mainItemBar.GetChild(e.to.slotIndex));
            slot.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
        else if(e.from.gridIndex == 0)
        {
            RemoveItem(mainItemBar, new PositionArgs(e.from));
        }
        else if (e.to.gridIndex == 0)
        {
            ItemStats item = EquipmentManager.instance.GetItemStatsValue(e.to);
            NewItemUI(mainItemBar, new CreateItemArgs(item,e.to,false));
        }   
    }
    private void RemoveDragItemUI(object sender, EventArgs e)
    {
        Transform slot = itemParent.GetComponentInChildren<DragDrop>().transform;
        slot.gameObject.SetActive(false);
        Destroy(slot.gameObject);
    }
    private void UpdateDragItemCount(object sender, ItemCountArgs e)
    {
        Transform slot = itemParent.GetComponentInChildren<DragDrop>().transform;
        if (e.count != 1) slot.GetChild(0).GetComponent<TextMeshProUGUI>().text = e.count.ToString();
        else slot.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }
    private void UpdateItemCount(object sender, UpdateItemCountArgs e)
    {
        Transform grid;
        if (e.position.gridIndex == 0) grid = equipmentItemBar;
        else grid = equipmentItemSlots;
        UpdateCount(grid, e);
        if(e.position.gridIndex == 0)
        {
            UpdateCount(mainItemBar,e);
        }    
        
    }
    private void UpdateCount(Transform gridUI, UpdateItemCountArgs e)
    {
        Transform slot = gridUI.GetChild(e.position.slotIndex).GetComponentInChildren<DragDrop>().transform;
        if (e.count != 1) slot.GetChild(0).GetComponent<TextMeshProUGUI>().text = e.count.ToString();
        else slot.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }
    private void RemoveItemUI(object sender, PositionArgs e)
    {
        Transform grid;
        if (e.position.gridIndex == 0) grid = equipmentItemBar;
        else grid = equipmentItemSlots;

        RemoveItem(grid, e);
        if(e.position.gridIndex == 0)
        {
            RemoveItem(mainItemBar, e);
        }
    }
    private void RemoveItem(Transform gridUI, PositionArgs e)
    {
        Transform slot = gridUI.GetChild(e.position.slotIndex).GetComponentInChildren<DragDrop>().transform;
        slot.gameObject.SetActive(false);
        Destroy(slot.gameObject);
    }
    private void MoveItemUI(object sender, MoveItemUIArgs e)
    {
        Transform gridFrom, gridTo;
        if (e.from.gridIndex == 0) gridFrom = equipmentItemBar;
        else gridFrom = equipmentItemSlots;

        if (e.to.gridIndex == 0) gridTo = equipmentItemBar;
        else gridTo = equipmentItemSlots;

        if (e.to.gridIndex == 0 || e.from.gridIndex == 0) MoveMainBarItem(this, new MoveItemArgs(e.from, e.to));


        DragDrop slot = gridFrom.GetChild(e.from.slotIndex).GetComponentInChildren<DragDrop>();
        slot.transform.SetParent(gridTo.GetChild(e.to.slotIndex));
        slot.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        slot.IsInSlot();           
    }
    private void CreateItemUI(object sender, CreateItemArgs e)
    {
        Transform gridUI;
        if (e.position.gridIndex == 0) gridUI = equipmentItemBar;
        else gridUI = equipmentItemSlots;

        NewItemUI(gridUI, e);
        if(e.position.gridIndex == 0 && !e.isDrag)
        {
            NewItemUI(mainItemBar, e);
        }
    }
    private void NewItemUI(Transform gridUI, CreateItemArgs e)
    {
        RectTransform transform = Instantiate(item, gridUI.GetChild(e.position.slotIndex)).GetComponent<RectTransform>();
        if (e.itemStats as DestroyableItem != null)
        {
            Transform bar = Instantiate(lifePointsBar, transform).transform.GetChild(0);
            UpdateBar((e.itemStats as DestroyableItem).GetLifePointsInPercent(),bar);
        }
        
        transform.anchoredPosition = Vector2.zero;

        transform.GetComponent<Image>().sprite = ItemsAsset.instance.GetIcon(e.itemStats.itemID);
        if (e.itemStats.itemCount != 1) transform.GetComponentInChildren<TextMeshProUGUI>().text = e.itemStats.itemCount.ToString();
        else transform.GetComponentInChildren<TextMeshProUGUI>().text = "";

        if (gridUI != mainItemBar)
        {
            transform.GetComponent<DragDrop>().SetCanvas(mainCanvas);
            transform.GetComponent<DragDrop>().IsInSlot();
        }
        else
        {
            transform.GetComponent<DragDrop>().enabled = false;
        }


    }
    private void OpenEquipment(object sender, BoolArgs e)
    {
        CloseWindows();
        background.gameObject.SetActive(e.value);
        equipment.gameObject.SetActive(e.value);
        if (!e.value) TooltipSystem.Hide();
        else openWindows.Add(equipment);
    }
    private void CloseWindows()
    {
        foreach (Transform item in openWindows)
        {
            item.gameObject.SetActive(false);
        }
    }
    private void SwitchBuildingUI(object sender, EventArgs e)
    {
        bool value = !building.gameObject.activeSelf;
        building.gameObject.SetActive(value);
        background.gameObject.SetActive(value);
        if (!value) TooltipSystem.Hide();
    }
    private void UpdateSelectedSlot(object sender, UpdateSelectedSlotInBarArgs e)
    {
        mainItemBar.GetChild(e.lastSlot).GetComponent<Image>().sprite = unSelected;
        mainItemBar.GetChild(e.currentSlot).GetComponent<Image>().sprite = selected;
        if(lastSlotUI != null) lastSlotUI.localScale = new Vector3(buttonScale, buttonScale,1);
        currentSlotUI = mainItemBar.GetChild(e.currentSlot).GetComponent<RectTransform>();
        lastSlotUI = mainItemBar.GetChild(e.lastSlot).GetComponent<RectTransform>();
    }
    private RectTransform lastSlotUI;
    private RectTransform currentSlotUI;
    private void UpdateButtonSize()
    {
        if(lastSlotUI != null && lastSlotUI.localScale.x != buttonScale)
        {
            float scale = math.lerp(lastSlotUI.localScale.x, buttonScale,Time.deltaTime * speedUnselecting);
            lastSlotUI.localScale = new Vector3(scale, scale,1);
        }

        if (currentSlotUI != null && currentSlotUI.sizeDelta.x != selectedButtonScale)
        {
            float scale = math.lerp(currentSlotUI.localScale.x, selectedButtonScale, Time.deltaTime * speedSelecting);
            currentSlotUI.localScale = new Vector3(scale, scale,1);
        }
    }
    private void LoadSlots(EquipmentGrid equipmentGrid,int number,bool numbering)
    {
        bool isMainBar = equipmentGrid == barGrid;
        if (numbering)
        {
            int index;
            for (int i = 0; i < number; i++)
            {
                index = i + 1;
                Transform slot = Instantiate(itemSlot, equipmentGrid.gridTransform).transform;
                if (isMainBar)
                    slot.GetComponent<DropSlot>().enabled = false;        
                else 
                    slot.GetComponent<DropSlot>().SetSlotPosition(i, equipmentGrid.gridIndex);
                
                if (index > 9) index = 0;
                Instantiate(slotIndex,slot).GetComponent<TextMeshProUGUI>().text = index.ToString();
            }
        }
        else
        {
            for (int i = 0; i < number; i++)
            {
                Transform slot = Instantiate(itemSlot, equipmentGrid.gridTransform).transform;
                slot.GetComponent<DropSlot>().SetSlotPosition(i, equipmentGrid.gridIndex);
            }
        }
    }
    private Transform GetItem(SlotPosition position)
    {
        Transform parent = null;
        if (position.gridIndex == 0) parent = equipmentItemBar;
        else if (position.gridIndex == 1) parent = equipmentItemSlots;

        if(parent != null)
        {
           return parent.GetChild(position.slotIndex).GetComponentInChildren<DragDrop>().transform;
        }
        else
        {
            return null;
        }

    }
    private Transform GetItemFromMainBar(SlotPosition position)
    {
        if (position.slotIndex < mainItemBar.childCount) return mainItemBar.GetChild(position.slotIndex).GetComponentInChildren<DragDrop>().transform;
        else return null;
    }
}
