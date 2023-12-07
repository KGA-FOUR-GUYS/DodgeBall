using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    private Text[] messageBox;

    public Action<string> OnMessage;

    string currentMsg = string.Empty;
    string lastMsg;

    private void Awake()
    {
        OnMessage += AddMessage;
    }

    private void Start()
    {
        messageBox = transform.GetComponentsInChildren<Text>();
        lastMsg = currentMsg;
    }

    private void Update()
    {
        if (currentMsg.Equals(lastMsg))
            return;

        ShowMessage(currentMsg);
        lastMsg = currentMsg;
    }

    public void AddMessage(string message)
    {
        currentMsg = message;
    }

    public void ShowMessage(string message)
    {
        bool isInput = false;
        for (int i = 0; i < messageBox.Length; i++)
        {
            if (messageBox[i].text.Equals(string.Empty))
            {
                messageBox[i].text = message;
                isInput = true;
                break;
            }
        }

        if (!isInput)
        {
            for (int i = messageBox.Length - 1; i > 0; i--)
            {
                messageBox[i].text = messageBox[i - 1].text;
            }

            messageBox[0].text = message;
        }
    }
}
