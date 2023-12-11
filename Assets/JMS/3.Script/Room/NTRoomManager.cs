using LitJson;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkRoom
{
    public class NTRoomManager : NetworkRoomManager
    {
        public static NTRoomManager Instance { get; private set; }
        public string ServerIP { get; set; }

        private string path;

        public override void Awake()
        {
            base.Awake();

            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != null)
            {
                Destroy(gameObject);
            }

            path = Application.dataPath + Path.DirectorySeparatorChar + "Database";
            SetConnectionInfo(path);
        }

#if UNITY_SERVER
        private void Start()
        {
            Instance.networkAddress = ServerIP;
            Instance.StartServer();
        }
#endif

        private void SetConnectionInfo(string path)
        {
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path);
                string jsonString = File.ReadAllText(path + Path.DirectorySeparatorChar + "config.json");

                JsonData itemData = JsonMapper.ToObject(jsonString);
                ServerIP = itemData[0]["IP"].ToString();
            }
        }

        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            //CanvasUIManager.Instance

            return base.OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer);
        }
    }
}