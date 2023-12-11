using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkRoom
{
    public class LogInControll : MonoBehaviour
    {
        public InputField inputID;
        public InputField inputPW;
        public Text txtLog;

        [SerializeField] private NTRoomManager _roomManager;


        private void Awake()
        {
            _roomManager = FindObjectOfType<NTRoomManager>();
        }

#if UNITY_SERVER
        private void Start()
        {
            gameObject.SetActive(false);
        }
#endif

        public void BtnLogIn()
        {
            if (string.IsNullOrEmpty(inputID.text) || string.IsNullOrEmpty(inputPW.text))
            {
                txtLog.text = "[LogInControll] ID 와 PW를 입력하세요.";
                return;
            }

            if (SQLManager.Instance.LogIn(inputID.text, inputPW.text))
            {
                UserInfo info = SQLManager.Instance.userInfo;
                Debug.Log($"[LogInControll] User connected with ID:{info.ID} / PW:{info.Password}");
                gameObject.SetActive(false);

                //_roomManager.networkAddress = _roomManager.ServerIP;
                _roomManager.StartClient();
            }
            else
            {
                txtLog.text = "[LogInControll] ID 또는 PW를 확인해주세요.";
            }
        }
    }
}