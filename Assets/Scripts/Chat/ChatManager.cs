using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using System;
using Unity.Netcode;
using System.IO;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SocialPlatforms;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private Image chatBackground;
    [SerializeField] private ScrollRect chatScrollRect;

    [SerializeField] private GameObject messagePrefab;


    //chat
    private Scrollbar chatScrollbar;
    private RectTransform chathandle;
    private Transform content;
    //

    //History
    public List<string> history = new List<string>();
    public int LastHistory;
    private int currentLastHistory = -1;
    private const int maxHistory = 10;
    //

    //Massages
    private const int maxLog = 20;
    private const int maxMessageOnScreen = 8;
    private const int timeToDisappear = 10;
    private bool isTimer = false;
    private  int LastIndex = 0;
    private int[] indexes = new int[maxMessageOnScreen];
    private int[] timers = new int[maxMessageOnScreen];
    //

    private bool isChat = false;

    private List<object> commandList;

    public static ChatManager instance { private set; get; }

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
                if(index < history.Count) chatInputField.text = history[index];
            }
            
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentLastHistory = math.clamp(--currentLastHistory, 0, history.Count - 1);
                int index = LastHistory - currentLastHistory;

                if (index < 0) index = history.Count + index;
                if (index < history.Count) chatInputField.text = history[index];
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
            ChatPrint(chatInputField.text);
            CheckCommands();
        }
        SwitchChat();
    }

    public void ChatPrint(string text)
    {
        Transform message = Instantiate(messagePrefab, content).transform;
        message.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
       
        if (history.Count < maxHistory)
        {
            history.Add(chatInputField.text);
            LastHistory = history.Count - 1;
        }
        else
        {
            LastHistory++;
            if (LastHistory >= maxHistory)
            {
                LastHistory = 0;
            }

            history[LastHistory] = chatInputField.text;
        }
        chatScrollbar.value = 0;
        SetTimerToDisappear();
    }

    private void CheckCommands()
    {
        string command = chatInputField.text;

        for (int i = 0; i < command.Length; i++)
        {
            char c = command[i];
            if (c == '/')
            {
                break;
            }
            else if (command[i] != ' ')
            {
                return;
            }
        }

        string[] properties = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var item in commandList)
        {        
            CommandBase commandBase = item as CommandBase;
            if (command.Contains(commandBase.commandId))
            {
                if (item is DebugCommand)
                {
                    Debug.Log("2jest!");
                    (item as DebugCommand).Invoke();
                }
                else if (item is DebugCommand<int>)
                {
                    int arg;
                    if (properties.Length > 1 && int.TryParse(properties[1], out arg))
                    {      
                        (item as DebugCommand<int>).Invoke(arg);
                    }
                    else ChatPrint($"/{commandBase.commandId} {commandBase.commandFormat}");
                } 
            }
        }

  

    }
    private void SetUp()
    {
        chatScrollbar = chatScrollRect.verticalScrollbar;
        chathandle = chatScrollbar.handleRect;
        content = chatScrollRect.content;
        commandList = this.AddComponent<DebugController>().GetCommandList();
    }
    private void SwitchChat()
    {
        currentLastHistory = -1;
        if (isChat) TurnOffChat();
        else TurnOnChat();        
    }
    private void TurnOnChat()
    {
        isChat = true;
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
    private void TurnOffChat()
    {
        isChat = false;
        chatInputField.gameObject.SetActive(false);
        chatBackground.color = new Color(1, 1, 1, 0);
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
