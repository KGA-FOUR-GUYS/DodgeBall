using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkRoom
{
    public class RoomManager : NetworkRoomManager
    {
        public static RoomManager Instance { get; private set; }

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
        }

        // 새로운 플레이어가 접속한 경우
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            
            PlayerController.ResetPlayerNumbers();
        }

        // 플레이어 중 누군가 접속해제한 경우
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);

            PlayerController.ResetPlayerNumbers();
        }

        public override void OnRoomServerSceneChanged(string sceneName)
        {
            // spawn the initial batch of Rewards
            if (sceneName == GameplayScene)
            {
                //Spawner.InitialSpawn();
            }
        }

        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            LocalDataManager playerDataManager = gamePlayer.GetComponent<LocalDataManager>();
            playerDataManager.number = roomPlayer.GetComponent<NetworkRoomPlayer>().index;
            return true;
        }

        public override void OnRoomStopClient()
        {
            base.OnRoomStopClient();
        }

        public override void OnRoomStopServer()
        {
            base.OnRoomStopServer();
        }

        public void Btn_OnStartGame()
        {
            ServerChangeScene(GameplayScene);
        }
    }
}