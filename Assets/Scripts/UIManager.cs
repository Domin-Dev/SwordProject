using UnityEngine.UI;
using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;
using System.Drawing;

public class UIManager : MonoBehaviour
{
    //black background
    [SerializeField] private Transform background;
   
    [Space(30f)]

    //equipment
    [SerializeField] private Transform itemBar;
    [SerializeField] private Transform equipment;
    [SerializeField] private GameObject itemSlot;
    [Space]
    [SerializeField] private Transform itemEquipmentSlots;
    [SerializeField] private Transform itemEquipmentBar;
    //

    [Space(30f)]


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


    public static UIManager instance { private set; get; }


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        SetUpNetworkUI();
    }

    private void Update()
    {
        UpdateButtonSize();
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
        LoadSlots(itemEquipmentSlots,EquipmentManager.SlotCount, false);
        LoadSlots(itemEquipmentBar,EquipmentManager.BarSlotCount, true);
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

    private void LoadSlots(Transform parent,int number,bool numbering)
    {
        if (numbering)
        {
            for (int i = 0; i < number; i++)
            {
                Instantiate(itemSlot, parent);
            }
        }
        else
        {
            for (int i = 0; i < number; i++)
            {
                Instantiate(itemSlot, parent);
            }
        }
    }
}
