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

        private static ChatManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != null)
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            Invoke("AddListenerToSendButton", 3f);
        }

        public void AddListenerToSendButton()
        {
            GameObject obj1 = GameObject.FindGameObjectWithTag("ChatList");
            chatList = obj1.GetComponent<RectTransform>();
            GameObject obj2 = GameObject.FindGameObjectWithTag("Input");
            inputMessage = obj2.GetComponent<InputField>();

            GameObject obj = GameObject.FindGameObjectWithTag("Send");
            Button sendButton = obj.GetComponent<Button>();

            //if (!isLocalPlayer) return;
            sendButton.onClick.AddListener(SendMessage_temp);
        }

        public void SendMessage_temp()
        {
            if (string.IsNullOrWhiteSpace(inputMessage.text)) return;

            CmdSendMessage(inputMessage.text);
            inputMessage.text = string.Empty;
        }

        [Command(requiresAuthority = false)]
        public void CmdSendMessage(string message)
        {
            RPCReceiveMessgae(message);
            inputMessage.text = string.Empty;
        }

        [ClientRpc]
        public void RPCReceiveMessgae(string message)
        {
            var obj = Instantiate(messagePrefab, chatList);
            Text msg = obj.GetComponent<Text>();
            msg.text = message;
        }
    }
}
