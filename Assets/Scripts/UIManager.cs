using UnityEngine.UI;
using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;
using TMPro;
using System;
using Unity.VisualScripting;

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
    [Space]
    [Header("Equipment UI")]
    [SerializeField] private Transform itemBar;
    [SerializeField] private Transform equipment;
    [Space]
    [SerializeField] private GameObject itemSlot;
    [SerializeField] private GameObject slotIndex;
    [SerializeField] private GameObject item;
    [SerializeField] private GameObject lifePointsBar;
    [Space]
    [SerializeField] private Transform itemEquipmentSlots;
    [SerializeField] private Transform itemEquipmentBar;
    [SerializeField] private Transform items;
    [Space(30f)]
    [Header("Stats UI")]


    //temporary
    [SerializeField] private Button ServerButton;
    [SerializeField] private Button HostButton;
    [SerializeField] private Button ClientButton;

    //temporary
    [SerializeField] Sprite selected;
    [SerializeField] Sprite unSelected;

    private const float buttonScale = 1f;
    private const float selectedButtonScale = 1.1f;

    private const float speedSelecting = 10f;
    private const float speedUnselecting = 15f;



    public Transform itemParent { get { return items; } }
    public static UIManager instance { private set; get; }


    private EquipmentGrid barGrid;
    private EquipmentGrid equipmentBarGrid;
    private EquipmentGrid mainEquipmentGrid;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);


        SetGrids();
        SetUpNetworkUI();
    }

    private void Update()
    {
        UpdateButtonSize();
    }


    private void SetGrids()
    {
        barGrid = new EquipmentGrid(itemBar, 0);
        equipmentBarGrid = new EquipmentGrid(itemEquipmentBar, 0);

        mainEquipmentGrid = new EquipmentGrid(itemEquipmentSlots, 1);
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

    private void UpdateItemLifeBar(object sender, LifeBarArgs e)
    {
        Transform slot = itemBar.GetChild(e.position.slotIndex).GetComponentInChildren<DragDrop>().transform;
        Image bar = null;
        for (int i = 0; i < slot.childCount; i++)
        {
            if(slot.GetChild(i).CompareTag("Bar"))
            {
                bar = slot.GetChild(i).transform.GetChild(0).GetComponent<Image>();
            }
        }
        
      

        if (bar == null) return;


        bar.transform.localScale = new Vector3(e.barValue, 1);
        bar.color = new Color(bar.color.r, e.barValue, bar.color.b);
    }

    private void UpdateMainBarItemCount(object sender, UpdateItemCountArgs e)
    {
        UpdateCount(itemBar, e);
    }

    private void RemoveMainBarItem(object sender, PositionArgs e)
    {
        RemoveItem(itemBar, e);
    }

    private void CreateMainBarItem(object sender, CreateItemArgs e)
    {
        NewItemUI(itemBar, e);
    }

    private void MoveMainBarItem(object sender, MoveItemArgs e)
    {
        if(e.from.gridIndex == 0 && e.to.gridIndex == 0)
        {
            DragDrop slot = itemBar.GetChild(e.from.slotIndex).GetComponentInChildren<DragDrop>();
            slot.transform.SetParent(itemBar.GetChild(e.to.slotIndex));
            slot.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
        else if(e.from.gridIndex == 0)
        {
            RemoveItem(itemBar, new PositionArgs(e.from));
        }
        else if (e.to.gridIndex == 0)
        {
            ItemStats item = EquipmentManager.instance.GetItemStatsValue(e.to);
            NewItemUI(itemBar, new CreateItemArgs(item,e.to,false));
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
        if (e.position.gridIndex == 0) grid = itemEquipmentBar;
        else grid = itemEquipmentSlots;
        UpdateCount(grid, e);
        if(e.position.gridIndex == 0)
        {
            UpdateCount(itemBar,e);
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
        if (e.position.gridIndex == 0) grid = itemEquipmentBar;
        else grid = itemEquipmentSlots;

        RemoveItem(grid, e);
        if(e.position.gridIndex == 0)
        {
            RemoveItem(itemBar, e);
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
        if (e.from.gridIndex == 0) gridFrom = itemEquipmentBar;
        else gridFrom = itemEquipmentSlots;

        if (e.to.gridIndex == 0) gridTo = itemEquipmentBar;
        else gridTo = itemEquipmentSlots;

        if (e.to.gridIndex == 0 || e.from.gridIndex == 0) MoveMainBarItem(this, new MoveItemArgs(e.from, e.to));


        DragDrop slot = gridFrom.GetChild(e.from.slotIndex).GetComponentInChildren<DragDrop>();
        slot.transform.SetParent(gridTo.GetChild(e.to.slotIndex));
        slot.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        slot.IsInSlot();           
    }

    private void MoveItem(Transform gridFrom,Transform gridTo, MoveItemUIArgs e)
    {
        DragDrop slot = gridFrom.GetChild(e.from.slotIndex).GetComponentInChildren<DragDrop>();
        slot.transform.SetParent(gridTo.GetChild(e.to.slotIndex));
        slot.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        slot.IsInSlot();
    }

    private void CreateItemUI(object sender, CreateItemArgs e)
    {
        Transform gridUI;
        if (e.position.gridIndex == 0) gridUI = itemEquipmentBar;
        else gridUI = itemEquipmentSlots;

        NewItemUI(gridUI, e);
        if(e.position.gridIndex == 0 && !e.isDrag)
        {
            NewItemUI(itemBar, e);
        }
    }

    private void NewItemUI(Transform gridUI, CreateItemArgs e)
    {
        RectTransform transform = Instantiate(item, gridUI.GetChild(e.position.slotIndex)).GetComponent<RectTransform>();
        if(e.itemStats as DestroyableItem != null) Instantiate(lifePointsBar, transform);
        
        transform.anchoredPosition = Vector2.zero;

        transform.GetComponent<Image>().sprite = ItemsAsset.instance.GetIcon(e.itemStats.itemID);
        if (e.itemStats.itemCount != 1) transform.GetComponentInChildren<TextMeshProUGUI>().text = e.itemStats.itemCount.ToString();
        else transform.GetComponentInChildren<TextMeshProUGUI>().text = "";

        if (gridUI != itemBar)
        {
            transform.GetComponent<DragDrop>().SetCanvas(mainCanvas);
            transform.GetComponent<DragDrop>().IsInSlot();
        }
        else
        {
            transform.GetComponent<DragDrop>().enabled = false;
        }


    }

    private void OpenEquipment(object sender, OpenEquipmentUIArgs e)
    {
        if (e.open)
        {
            background.gameObject.SetActive(true);
            equipment.gameObject.SetActive(true);
        }
        else
        {
            TooltipSystem.Hide();
            background.gameObject.SetActive(false);
            equipment.gameObject.SetActive(false);
        }
    }
    private void UpdateSelectedSlot(object sender, UpdateSelectedSlotInBarArgs e)
    {
        itemBar.GetChild(e.lastSlot).GetComponent<Image>().sprite = unSelected;
        itemBar.GetChild(e.currentSlot).GetComponent<Image>().sprite = selected;
        if(lastSlotUI != null) lastSlotUI.localScale = new Vector3(buttonScale, buttonScale,1);
        currentSlotUI = itemBar.GetChild(e.currentSlot).GetComponent<RectTransform>();
        lastSlotUI = itemBar.GetChild(e.lastSlot).GetComponent<RectTransform>();
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
        if (position.gridIndex == 0) parent = itemEquipmentBar;
        else if (position.gridIndex == 1) parent = itemEquipmentSlots;

        if(parent != null)
        {
           return parent.GetComponentInChildren<DropSlot>().transform;
        }
        else
        {
            return null;
        }

    }
}
