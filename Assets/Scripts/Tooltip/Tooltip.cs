using UnityEngine.UI;
using TMPro;
using UnityEngine;


public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerField;
    [SerializeField] private TextMeshProUGUI contentField;
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private int characterWrapLimit;
    [SerializeField] private RectTransform rectTransform;

    bool isShow = false;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void SetText(string content, string header = "")
    {
        UpdatePosition();
        Show();
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        contentField.text = content;
        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;     
    }

    public void Show()
    {
        isShow = true;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        isShow = false;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if(isShow)
        {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        Vector2 position = Input.mousePosition;
        float pivotX = 0, pivotY = 0;
        if (Screen.width - position.x < rectTransform.rect.width) pivotX = 1;
        if (Screen.height - position.y < rectTransform.rect.height) pivotY = 1;
        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = position;
    }
}

