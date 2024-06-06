using UnityEngine.UI;
using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;
using TMPro;
using System;
using UnityEditor;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using System.Linq;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

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
    [Space(20f)]
    //equipment
    [SerializeField] private Canvas mainCanvas;
    [Header("Gun Info UI")]
    [SerializeField] private Transform ammoBar;
    [SerializeField] private GameObject ammoUI;
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
    [Space(20f)]
    #endregion
    #region Stats UI
    [Header("Stats UI")]
    [SerializeField] private Button ServerButton;
    [SerializeField] private Button HostButton;
    [SerializeField] private Button ClientButton;
    [SerializeField] Sprite selected;
    [SerializeField] Sprite unSelected;
    #endregion
    #region Crafting UI
    [Header("Crafting UI")]
    [SerializeField] Transform recipeDescription;
    [SerializeField] Transform recipes;
    [SerializeField] GameObject recipeIcon;
    [SerializeField] Image selectedRecipeIcon;
    [SerializeField] TextMeshProUGUI selectedRecipeNumberItems;
    [SerializeField] TextMeshProUGUI selectedRecipeName;
    [SerializeField] TextMeshProUGUI selectedRecipeDescription;
    [SerializeField] Transform ingredients;
    [SerializeField] Button craftButton;
    #endregion

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
        LoadRecipes();
    }
    private void Update()
    {
        UpdateButtonSize();
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
            slot.transform.SetAsFirstSibling();
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
        slot.transform.SetAsFirstSibling();
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
        transform.SetAsFirstSibling();

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

        if (!e.value)
        {
            TooltipSystem.Hide();
            openWindows.Remove(equipment);
        }
        else
        {
            openWindows.Add(equipment);
            ClearRecipe();
            CheckRecipes();
        }
    }
    private void CloseWindows()
    {
        foreach (Transform item in openWindows)
        {
            item.gameObject.SetActive(false);
        }
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
                if (!isMainBar)
                    slot.AddComponent<DropSlot>().SetSlotPosition(i, equipmentGrid.gridIndex);
                
                if (index > 9) index = 0;
                Instantiate(slotIndex,slot).GetComponent<TextMeshProUGUI>().text = index.ToString();
            }
        }
        else
        {
            for (int i = 0; i < number; i++)
            {
                Transform slot = Instantiate(itemSlot, equipmentGrid.gridTransform).transform;
                slot.AddComponent<DropSlot>().SetSlotPosition(i, equipmentGrid.gridIndex);
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

    private void LoadRecipes()
    {
        ReadOnlyCollection<Item> items = ItemsAsset.instance.GetRecipesCrafTable(-1);
        for (int i = 0; i < items.Count; i++)
        {
            Item item = items[i];
            Transform recipe = Instantiate(itemSlot, recipes).transform;
            recipe.name = item.ID.ToString();
            Transform icon = Instantiate(recipeIcon, recipe).transform;
            icon.GetComponent<Image>().sprite = item.icon;
            icon.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            recipe.AddComponent<Recipe>().SetID(item.ID);
        }
    }

    int selectedRecipe;
    public void SelectRecipe(int id)
    {
        selectedRecipe = id;
        recipeDescription.gameObject.SetActive(true);
        Item item = ItemsAsset.instance.GetItem(id);
        selectedRecipeIcon.sprite = item.icon;
        if (item.numberItem != 1) selectedRecipeNumberItems.text = item.numberItem.ToString();
        else selectedRecipeNumberItems.text = string.Empty;
        selectedRecipeName.text = item.name;
        selectedRecipeDescription.text = item.description;
        if(CanCraft(id))
        {
            craftButton.interactable = true;
        }
        else
        {
            craftButton.interactable= false;
        }

        LoadIngredients(item);
    }
    private void LoadIngredients(Item item)
    {
        for (int i = 0; i < 8; i++)
        {
            Transform slot = ingredients.GetChild(i);
            if (item.crafingIngredients.Length > i)
            {
                slot.gameObject.SetActive(true);
                Item.CrafingIngredient ingredient = item.crafingIngredients[i];          
                Sprite sprite = ItemsAsset.instance.GetIcon(ingredient.itemID);
                slot.GetChild(0).GetComponent<Image>().sprite = sprite;
                slot.GetComponentInChildren<TextMeshProUGUI>().text = ingredient.number.ToString();
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }        
    }
    public void ClearRecipe()
    {
        recipeDescription.gameObject.SetActive(false);
    }

    string lastValue = string.Empty;
    private bool CheckLastValue(string value)
    {
        return  (value.Length == 0 && lastValue.Length == 0) || 
            (lastValue.Length > 0 && value.Length >= lastValue.Length && lastValue.ToLower() == value.Substring(0, lastValue.Length).ToLower());
    }
    public void Search(string value)
    {
        if(CheckLastValue(value))
        {
            if (value.Length == lastValue.Length) return;
            for (int i = 0; i < recipes.childCount; i++)
            {
                Transform child = recipes.GetChild(i);
                if(child.gameObject.activeSelf)
                {
                    ComperName(child, value,int.Parse(child.name));
                }
            }
            lastValue = value;
            return;
        }

        for (int i = 0; i < recipes.childCount; i++)
        {
            Transform child = recipes.GetChild(i);
            ComperName(child, value, int.Parse(child.name));
        }
        lastValue = value;
    }
    private void ComperName(Transform child,string searchText,int itemID)
    {
        Item item = ItemsAsset.instance.GetItem(itemID);
        if (item.name.Length >= searchText.Length && item.name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
        {
            child.gameObject.SetActive(true);
        }
        else
        {
            child.gameObject.SetActive(false);
        }
    }
    private void CheckRecipes()
    {
        Dictionary<int,int> items = EquipmentManager.instance.GetItemDictionary();
        for (int i = 0; i < recipes.childCount; i++)
        {
            CheckRecipe(i, items);
        }
    }
    private void CheckRecipe(Dictionary<int, int> items, int itemID)
    {
        int index = recipes.Find(itemID.ToString()).transform.GetSiblingIndex();
        CheckRecipe(index, items);
    }
    private void CheckRecipe(int childIndex, Dictionary<int, int> items)
    {
        Transform child = recipes.GetChild(childIndex);
        Image backgroundItem = child.GetComponent<Image>();
        if (CanCraft(int.Parse(child.name), items))
        {
            if (backgroundItem.color == Color.white) return;
            backgroundItem.color = Color.white;
            child.GetChild(0).GetComponent<Image>().color = Color.white;
            child.SetAsFirstSibling();
        }
        else
        {
            if (backgroundItem.color != Color.white) return;
            backgroundItem.color = new Color(1, 1, 1, 0.5f);
            child.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            child.SetAsLastSibling();
        }
    }
    private bool CanCraft(int ID,Dictionary<int,int> items)
    {
        Item item = ItemsAsset.instance.GetItem(ID);
        for (int i = 0; i < item.crafingIngredients.Length; i++)
        {
            items.TryGetValue(item.crafingIngredients[i].itemID, out int value);
            if (value < item.crafingIngredients[i].number) return false;
        }
        return true;
    }
    private bool CanCraft(int ID)
    {
       return CanCraft(ID, EquipmentManager.instance.GetItemDictionary());
    }
    public void Craft()
    {
        EquipmentManager.instance.Craft(selectedRecipe);
        Sounds.instance.Click();
        Dictionary<int, int> items = EquipmentManager.instance.GetItemDictionary();
        CheckRecipe(items,selectedRecipe);
        SelectRecipe(selectedRecipe);
    }
    public void CheckRecipesWithItem(int id,bool increasedItemCount)
    {
        if (equipment.gameObject.activeSelf)
        {
            int counter = EquipmentManager.instance.CountItems(id);
            ReadOnlyCollection<Item> items = ItemsAsset.instance.GetRecipesCrafTable(-1);
            Dictionary<int, int> eq = EquipmentManager.instance.GetItemDictionary();
            foreach (var item in items)
            {
                foreach (var ingredient in item.crafingIngredients)
                {
                    if (UpdateCheck(item.ID,ingredient, counter, id, eq, increasedItemCount)) break;
                }
            }
        }
    }
    private bool UpdateCheck(int idRecipe, Item.CrafingIngredient ingredient,int counter,int id, Dictionary<int, int> eq,bool increasedItemCount)
    {
        if (ingredient.itemID == id)
        {
            if (increasedItemCount && ingredient.number <= counter)
            {
                CheckRecipe(eq, idRecipe);
                return true;
            }
            else if(!increasedItemCount && ingredient.number > counter)
            {
                CheckRecipe(eq, idRecipe);
                return true;
            }         
        }
        return false;
    }
    public bool WindowsAreClosed()
    {
        return openWindows.Count == 0;
    }
}
