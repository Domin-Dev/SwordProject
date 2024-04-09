
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    List<object> commandList = new List<object>();
    public List<object> GetCommandList()
    {
        commandList.Add(new DebugCommand("set_time", "sets the time", "", () =>
        {
            Debug.Log("time set to 10");
        }));

        commandList.Add(new DebugCommand("host", "start host", "", () =>
        {
            NetworkManager.Singleton.StartHost();
        }));

        commandList.Add(new DebugCommand<int>("spawn", "spawn", "[Number]", (x) =>
        {
            Debug.Log("spawn" + x);
        }));

        commandList.Add(new DebugCommand<int,int>("give", "Adds item to player equipment", "[ID] [Count]", (ID, count) =>
        {
            Debug.Log(ID + " "+ count);
            EquipmentManager.instance.AddNewItem(new ItemStats(ID,count));
        }));


        commandList.Add(new DebugCommand("help", "command list", "", () =>
        {
            string text = "Command list:\n";
            for (int i = 0; i < commandList.Count; i++)
            {
                CommandBase commandBase = commandList[i] as CommandBase;
                text += $"/{commandBase.commandId} {commandBase.commandFormat} - {commandBase.commandDescription}\n";
            }
            ChatManager.instance.ChatPrint(text);
        }));

        return commandList;
    }

}
