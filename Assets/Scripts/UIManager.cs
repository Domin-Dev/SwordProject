using UnityEngine.UI;
using UnityEngine;
using Unity.Netcode;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Transform itemBar;
    [SerializeField] private Transform chat;


    //temporary
    [SerializeField] private Button ServerButton;
    [SerializeField] private Button HostButton;
    [SerializeField] private Button ClientButton;


    //temporary
    [SerializeField] Sprite selected;
    [SerializeField] Sprite unSelected;

    public static UIManager instance { private set; get; }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        SetUpNetworkUI();
       
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
    }

    private void UpdateSelectedSlot(object sender, UpdateSelectedSlotInBarArgs e)
    {
        itemBar.GetChild(e.lastSlot).GetComponent<Image>().sprite = unSelected;
        itemBar.GetChild(e.currentSlot).GetComponent<Image>().sprite = selected;
    }
}
