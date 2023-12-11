using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace NetworkRoom
{
    public class ChatManager : NetworkBehaviour
    {
        [Header("Message")]
        public GameObject messagePrefab;
        public RectTransform chatList;
        public InputField inputMessage;

        private void Start()
        {
            if (isServer) return;

            Invoke("AddListenerToSendButton", 3f);
        }

        [ClientCallback]
        public void AddListenerToSendButton()
        {
            if (!isLocalPlayer) return;

            GameObject obj1 = GameObject.FindGameObjectWithTag("ChatList");
            chatList = obj1.GetComponent<RectTransform>();
            GameObject obj2 = GameObject.FindGameObjectWithTag("Input");
            inputMessage = obj2.GetComponent<InputField>();

            GameObject obj = GameObject.FindGameObjectWithTag("Send");
            Button sendButton = obj.GetComponent<Button>();
            sendButton.onClick.AddListener(CmdSendMessage);
        }

        [Command]
        public void CmdSendMessage()
        {
            var obj = Instantiate(messagePrefab, chatList);
            Text msg = obj.GetComponent<Text>();
            msg.text = inputMessage.text;
            inputMessage.text = string.Empty;

            NetworkServer.Spawn(obj);
            RPCSyncMessage();
        }

        [ClientRpc]
        public void RPCSyncMessage()
        {
            var obj = Instantiate(messagePrefab, chatList);
            Text msg = obj.GetComponent<Text>();
            msg.text = inputMessage.text;
            inputMessage.text = string.Empty;
        }
    }
}
