using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Transform equipmentClothes;
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
    [SerializeField] Transform ingredients;
    #endregion

    [SerializeField] Transform collectedItems;

    private const float buttonScale = 1f;
    private const float selectedButtonScale = 1.1f;

    private const float speedSelecting = 10f;
    private const float speedUnselecting = 15f;

    public Transform itemParent { get { return equipmentDragItems; } }
    public static UIManager instance { private set; get; }

    private EquipmentGrid barGrid;
    private EquipmentGrid equipmentBarGrid;
    private EquipmentGrid mainEquipmentGrid;
    private EquipmentGrid clothesGrid;

    private List<Transform> openWindows = new List<Transform>();

    public event EventHandler windowOpen;

    private CharacterSpriteController characterSpriteController;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        SetGrids();
        SetUpNetworkUI();
        SetUpNotices();
        LoadRecipes();
        characterSpriteController = FindObjectOfType<CharacterSpriteController>();
    }
    private void Update()
    {
        UpdateButtonSize();
    }
    public void FixedUpdate()
    {
        MultiCraft();
    }

    private int i = 0;
    private int k = 0;
    private bool isHold;
    private void MultiCraft()
    {
        if (isHold)
        {
            i++;
            k++;
            if (i >= (7 - k / 20) + 1)
            {
                Craft(selectedID);
                i = 0;
            }
        }
    }

    private void SetGrids()
    {
        barGrid = new EquipmentGrid(mainItemBar, 0);
        equipmentBarGrid = new EquipmentGrid(equipmentItemBar, 0);
        mainEquipmentGrid = new EquipmentGrid(equipmentItemSlots, 1);
        clothesGrid = new EquipmentGrid(equipmentClothes, 2);
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
        eqManager.TurnPlaceholder += TurnPlaceholder;
       

        LoadSlots(mainEquipmentGrid,EquipmentManager.SlotCount, false);
        LoadSlots(equipmentBarGrid,EquipmentManager.BarSlotCount, true);
        LoadSlots(barGrid,EquipmentManager.BarSlotCount,true);
        LoadClothesSlots(clothesGrid); 
    }

    private void TurnPlaceholder(object sender, PlaceholderArgs e)
    {
        SwitchPlaceholder(e.turn, GetGrid(e.slotPosition.gridIndex).GetChild(e.slotPosition.slotIndex));
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
        Transform grid = GetGrid(e.position.gridIndex);
        
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

    private void SwitchPlaceholder(bool turnOn,Transform parentSlot)
    {
        for (int i = 0; i < parentSlot.childCount; i++)
        {
            if(parentSlot.GetChild(i).CompareTag("Placeholder"))
            {
                if(turnOn)
                {
                    parentSlot.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    parentSlot.GetChild(i).gameObject.SetActive(false);
                }
                return;
            }
        }
    }

    private Transform GetGrid(int gridIndex)
    {
        switch(gridIndex)
        {
            case 0: return equipmentItemBar;
            case 1: return equipmentItemSlots;
            case 2: return equipmentClothes;
        }
        return null;
    }

    private void MoveItemUI(object sender, MoveItemUIArgs e)
    {
        Transform gridFrom = GetGrid(e.from.gridIndex);
        Transform gridTo = GetGrid(e.to.gridIndex);
        
        if(e.from.gridIndex == 2) 
        {
            SwitchPlaceholder(false, gridFrom.GetChild(e.from.slotIndex));
        }
        if(e.to.gridIndex == 2)
        {
            SwitchPlaceholder(true, gridFrom.GetChild(e.to.slotIndex));
        }
      

        if (e.to.gridIndex == 0 || e.from.gridIndex == 0) MoveMainBarItem(this, new MoveItemArgs(e.from, e.to));


        DragDrop slot = gridFrom.GetChild(e.from.slotIndex).GetComponentInChildren<DragDrop>();
        slot.transform.SetParent(gridTo.GetChild(e.to.slotIndex));
        slot.transform.SetAsFirstSibling();
        slot.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        slot.IsInSlot();           
    }
    private void CreateItemUI(object sender, CreateItemArgs e)
    {
        Transform gridUI = GetGrid(e.position.gridIndex);
        if (e.position.gridIndex == 2)
        {
            SwitchPlaceholder(false,gridUI.GetChild(e.position.slotIndex));
        }

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
            windowOpen(this,null);
            openWindows.Add(equipment);
            SelectItem(-1);
            CheckRecipes();
        }
    }
    private void CloseWindows()
    {
        foreach (Transform item in openWindows)
        {
            item.gameObject.SetActive(false);
        }
        isHold = false;
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

    private void LoadClothesSlots(EquipmentGrid grid)
    {
        for (int i = 0; i < grid.gridTransform.childCount; i++)
        {
            grid.gridTransform.GetChild(i).GetComponent<DropSlot>().SetSlotPosition(i, grid.gridIndex);
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

    Dictionary<int, Transform> itemRecipes;
    private void LoadRecipes()
    { 
        ReadOnlyCollection<Item> items = ItemsAsset.instance.GetRecipesCrafTable(-1);
        itemRecipes = new Dictionary<int, Transform>();
      //  craftButton.GetComponent<ButtonHold>().action = () => { Craft(); };

        for (int i = 0; i < items.Count; i++)
        {
            Item item = items[i];
            Transform recipe = Instantiate(itemSlot, recipes).transform;
            recipe.name = item.ID.ToString();
            Transform icon = Instantiate(recipeIcon, recipe).transform;
            icon.GetComponent<Image>().sprite = item.icon;
            if(item.numberItem != 1) icon.GetComponentInChildren<TextMeshProUGUI>().text = item.numberItem.ToString();
            icon.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            recipe.AddComponent<Recipe>().SetID(item.ID);
            itemRecipes.Add(item.ID, recipe);
        }
    }

    string lastValue = string.Empty;
    int selectedID = -1;
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
        foreach (var item in itemRecipes)
        {
            CheckRecipe(item.Key, items);
        }
    }
    private void CheckRecipe(int id, Dictionary<int, int> items)
    {
        Transform child = itemRecipes[id];
        Image backgroundItem = child.GetComponent<Image>();
        if (CanCraft(int.Parse(child.name),items))
        {
            if (backgroundItem.color == Color.white) return;
            backgroundItem.color = Color.white;
            child.GetChild(0).GetComponent<Image>().color = Color.white;
           // child.SetAsFirstSibling();
        }
        else
        {
            if (backgroundItem.color != Color.white) return;
            backgroundItem.color = new Color(1, 1, 1, 0.5f);
            child.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
           // child.SetAsLastSibling();
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
    public void Craft(int id)
    {
        if (selectedID != id)
        {
            SelectItem(id);
            return;
        }


        if (CanCraft(id))
        {
            EquipmentManager.instance.Craft(id);
            Sounds.instance.Click();
            Dictionary<int, int> items = EquipmentManager.instance.GetItemDictionary();
            CheckRecipe(id, items);      
        }
    }
    public void LoadIngredients(int id)
    {
        Item item = ItemsAsset.instance.GetItem(id);
        if (item != null)
        {
            for (int i = 0; i < 5; i++)
            {
                if (item.crafingIngredients.Length > i)
                {
                    Transform child = ingredients.GetChild(i);
                    Item.CrafingIngredient crafingIngredient = item.crafingIngredients[i];
                    Item ingredient = ItemsAsset.instance.GetItem(crafingIngredient.itemID);
                    child.gameObject.SetActive(true);
                    child.GetChild(0).GetComponent<Image>().sprite = ingredient.icon;
                    child.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = crafingIngredient.number.ToString();
                    child.GetChild(1).GetComponent<TextMeshProUGUI>().text = ingredient.name;
                }
                else
                {
                    ingredients.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    private void SelectItem(int id)
    {
        if (id >= 0)
        {
            if (itemRecipes.ContainsKey(selectedID)) itemRecipes[selectedID].GetComponent<Image>().sprite = unSelected;
            selectedID = id;
            LoadIngredients(id);
            itemRecipes[id].GetComponent<Image>().sprite = selected;
        }
        else
        {
            if (itemRecipes.ContainsKey(selectedID)) itemRecipes[selectedID].GetComponent<Image>().sprite = unSelected;
            selectedID = id;
            ClearIngredients();
        }
    }
    public void ClearIngredients()
    {
        selectedID = -1;
        for (int i = 0; i < 5; i++)
        {
            ingredients.GetChild(i).gameObject.SetActive(false);
        }
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
                CheckRecipe(idRecipe, eq);
                return true;
            }
            else if(!increasedItemCount && ingredient.number > counter)
            {
                CheckRecipe(idRecipe, eq);
                return true;
            }         
        }
        return false;
    }
    public bool WindowsAreClosed()
    {
        return openWindows.Count == 0;
    }

    Timer timer;

    public void ButtonIsHolding(bool value,int id)
    {
        if(value)
        {
            k = 0;
            timer = Timer.Create(0.2f, () =>
            {
                SelectItem(id);
                isHold = true;
                return false;
            });
        }
        else
        {
            isHold = false;
            timer.Cancel();
        }
    }

    public Timer[] collectedItemTimers = new Timer[5];
    public Transform[] notices = new Transform[5];

    private void SetUpNotices()
    {
        for (int i = 0; i < 5; i++)
        {
            notices[i] = collectedItems.GetChild(i);
        }
    }
    public void NewCollectedItem(ItemStats stats)
    {
        for (int i = 0; i < 5; i++)
        {
            if (collectedItemTimers[i] == null)
            {
                CreateNotice(stats,i);
                return;
            } 
        }

        int index = 0;
        float min = collectedItemTimers[0].GetTime();
        for (int i = 1; i < 5; i++)
        {
            float time = collectedItemTimers[i].GetTime();
            if (time < min)
            {
                index = i;
                min = time;
            }
        }
        collectedItemTimers[index].Cancel();
        CreateNotice(stats, index);
    }

    private void CreateNotice(ItemStats stats,int index)
    {
        Transform item = notices[index];
        CanvasGroup canvasGroup = item.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        item.gameObject.SetActive(true);
        item.SetAsLastSibling();
        SetCollectItem(stats, item);
        collectedItemTimers[index] = Timer.Create(1f, () =>
        {
            collectedItemTimers[index] = Timer.Create(() =>
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, Time.deltaTime * 7f);
                if (canvasGroup.alpha < 0.1f)
                {
                    return true;
                }
                return false;
            },
            () =>
            {
                collectedItemTimers[index] = null;
                item.gameObject.SetActive(false);
                return true;
            });
            return false;
        });
    }
    private void SetCollectItem(ItemStats stats,Transform obj)
    {
        Item item = ItemsAsset.instance.GetItem(stats.itemID);
        obj.GetChild(0).GetComponent<TextMeshProUGUI>().text = "+"+stats.itemCount;
        obj.GetChild(1).GetComponent<Image>().sprite = item.icon;
        obj.GetChild(2).GetComponent<TextMeshProUGUI>().text = item.name;
    }
}
