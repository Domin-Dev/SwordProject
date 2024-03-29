using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using System;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private Image chatBackground;
    [SerializeField] private ScrollRect chatScrollRect;

    [SerializeField] private GameObject messagePrefab;

    private Scrollbar chatScrollbar;
    private RectTransform chathandle;
    private Transform content;

    public List<string> history = new List<string>();
    public int LastHistory;
    private int currentLastHistory = -1;

    private const int maxLog = 20;
    private const int maxHistory = 10;
    private const int maxMessageOnScreen = 8;
    private const int timeToDisappear = 10;



    bool isTimer = false;
    int LastIndex = 0;
    private int[] indexes = new int[maxMessageOnScreen];
    private int[] timers = new int[maxMessageOnScreen];
   
    bool isChat = false;

    private void Awake()
    {
        SetUp();
    
    }

    private void Start()
    {
        ClearIndexes();
    }

    float timer = 0;
    private void Update()
    {
        if(isTimer)
        {
            timer += Time.deltaTime;
            if(timer >= 1)
            {
                timer = 0;
                UpdateTimers();
            }
        }

        if (Input.GetKeyDown(KeyCode.T) && !chatInputField.isFocused)
        {
            SwitchChat();
        }



        if(isChat)
        {

            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessage();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SwitchChat();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentLastHistory = math.clamp(++currentLastHistory, 0, history.Count - 1);
                int index = LastHistory - currentLastHistory;


                if (index < 0) index = history.Count + index;
                chatInputField.text = history[index];
            }
            
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentLastHistory = math.clamp(--currentLastHistory, 0, history.Count - 1);
                int index = LastHistory - currentLastHistory;

                if (index < 0) index = history.Count + index;
                chatInputField.text = history[index];
            }

        }
    }
    private void UpdateTimers()
    {
        for (int i = 0; i < timers.Length; i++)
        {
            if (indexes[i] != -1)
            {
                timers[i]++;
                if (timers[i] >= timeToDisappear)
                {
                    int index = indexes[i];

                    if (index == content.childCount - 1) isTimer = false;
                    if (!isChat)
                    {
                        content.GetChild(index).gameObject.SetActive(false);
                        content.GetChild(indexes[i]).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    }
                    indexes[i] = -1;                 
                }
            }
        }
    }
    private void AddNewTimer(int index)
    {
        for (int i = 0; i < indexes.Length; i++)
        {
            if (indexes[i] == -1)
            {
                indexes[i] = index;
                timers[i] = 0;
                return;
            }
        }

        for (int i = 0; i < indexes.Length; i++)
        {
            if (LastIndex - maxMessageOnScreen == indexes[i])
            {
                if (!isChat)
                {
                    content.GetChild(indexes[i]).gameObject.SetActive(false);
                                            content.GetChild(indexes[i]).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }

                indexes[i] = index;
                timers[i] = 0;
                return;
            }
        }

    }
    private void ClearIndexes()
    {
        for (int i = 0; i < indexes.Length; i++)
        {
            indexes[i] = -1;
        }
    }
    private void SetTimerToDisappear()
    {
        isTimer = true;
        if (content.childCount > maxLog)
        {
            Destroy(content.GetChild(0).gameObject);
            for (int i = 0; i < indexes.Length; i++)
            {
                if (indexes[i] != -1) indexes[i]--;
            }
        }
        LastIndex = content.childCount - 1;
        AddNewTimer(LastIndex);
    }
    private void SendMessage()
    {
        if (chatInputField.text.Length > 0)
        {
            Transform message = Instantiate(messagePrefab, content).transform;
            message.GetChild(0).GetComponent<TextMeshProUGUI>().text = chatInputField.text;
            if (history.Count < maxHistory)
            {
                history.Add(chatInputField.text); 
                LastHistory = history.Count - 1;
            }
            else
            {
                LastHistory++;
                if(LastHistory >= maxHistory)
                {
                    LastHistory = 0;
                }

                history[LastHistory] = chatInputField.text;
            }
            chatScrollbar.value = 0;
            SetTimerToDisappear();

        }
        SwitchChat();
    }



    private void SetUp()
    {
        chatScrollbar = chatScrollRect.verticalScrollbar;
        chathandle = chatScrollbar.handleRect;
        content = chatScrollRect.content;
    }

    private void SwitchChat()
    {
        isChat = !isChat;
        currentLastHistory = -1;
        if (isChat)
        {
            chatInputField.gameObject.SetActive(true);
            chatInputField.text = string.Empty;
            chatInputField.ActivateInputField();
            chatBackground.color = Color.white;
            chatScrollbar.value = 0;
            chathandle.gameObject.SetActive(true);
            chatScrollRect.enabled = true;
            for (int i = 0; i < content.childCount; i++)
            {
                content.GetChild(i).gameObject.SetActive(true);
            }

            for (int i = 0; i < indexes.Length; i++)
            {
                if (indexes[i] != -1)
                {
                    content.GetChild(indexes[i]).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
            }
        }
        else
        {
            chatInputField.gameObject.SetActive(false);
            chatBackground.color = new Color(1,1,1,0);
            chatScrollbar.value = 0;
            chathandle.gameObject.SetActive(false);
            chatScrollRect.enabled = false;
            for (int i = 0; i < content.childCount; i++)
            {
                content.GetChild(i).gameObject.SetActive(false);
            }

            for (int i = 0; i < indexes.Length; i++)
            {
                if (indexes[i] != -1)
                {
                    content.GetChild(indexes[i]).gameObject.SetActive(true);
                    content.GetChild(indexes[i]).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                }
            }
        }
    }


}
