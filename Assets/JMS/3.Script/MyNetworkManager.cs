using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkUI
{
    public class MyNetworkManager : NetworkManager
    {
        public static MyNetworkManager Instance { get; private set; }

        /// <summary>
        /// Runs on both Server and Client
        /// Networking is NOT initialized when this fires
        /// </summary>
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

        /// <summary>
        /// Called on the server when a client adds a new player with NetworkClient.AddPlayer.
        /// <para>The default implementation for this function creates a new player object from the playerPrefab.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            // 새로운 플레이어가 접속한 경우!
            MyNetworkPlayer.ResetPlayerNumbers();
        }

        /// <summary>
        /// Called on the server when a client disconnects.
        /// <para>This is called on the Server when a Client disconnects from the Server. Use an override to decide what should happen when a disconnection is detected.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);

            // 플레이어 중 누군가 접속해제한 경우!
            MyNetworkPlayer.ResetPlayerNumbers();
        }
    }
}