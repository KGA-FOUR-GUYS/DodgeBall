using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkRoom
{
    [RequireComponent(typeof(LocalDataManager))]
    [RequireComponent(typeof(GlobalDataManager))]
    public class PlayerController : NetworkBehaviour
    {
        // Players List to manage playerNumber
        static readonly List<PlayerController> playersList = new List<PlayerController>();

        private GlobalDataManager _globalDataManager;
        private LocalDataManager _localDataManager;

        [Header("Player UI")]
        public GameObject globalUIPrefab;
        public GameObject localUIPrefab;

        GameObject globalUIObject;
        GameObject localUIObject;
        GlobalUIManager globalUIManager;
        LocalUIManager localUIManager;

        private void Awake()
        {
            TryGetComponent(out _globalDataManager);
            TryGetComponent(out _localDataManager);
        }

        #region Server

        public override void OnStartServer()
        {
            //// Add this to the static Players List
            playersList.Add(this);

            // set the Player Color SyncVar
            _localDataManager.color = Random.ColorHSV(0f, 1f, 0.9f, 0.9f, 1f, 1f);

            // set the initial player data
            _localDataManager.hitCount = 0;
            _localDataManager.attackCount = 0;
        }

        [ServerCallback]
        internal static void ResetPlayerNumbers()
        {
            byte playerNumber = 0;
            foreach (PlayerController player in playersList)
                player._localDataManager.number = playerNumber++;
        }

        public override void OnStopServer()
        {
            playersList.Remove(this);
        }

        #endregion

        #region Client

        public override void OnStartClient()
        {
            // Instantiate the player UI as child of the Players Panel
            globalUIObject = Instantiate(globalUIPrefab, MyNetworkCanvasUI.GetMainPanel());
            globalUIManager = globalUIObject.GetComponent<GlobalUIManager>();

            localUIObject = Instantiate(localUIPrefab, MyNetworkCanvasUI.GetPlayersPanel());
            localUIManager = localUIObject.GetComponent<LocalUIManager>();

            _globalDataManager.OnGameTimeChanged += globalUIManager.OnGlobalTimeValueChanged;

            _localDataManager.OnColorChanged += localUIManager.OnPlayerColorChanged;
            _localDataManager.OnNumberChanged += localUIManager.OnPlayerNumberChanged;
            _localDataManager.OnHitCountChanged += localUIManager.OnPlayerHitCountChanged;
            _localDataManager.OnAttackCountChanged += localUIManager.OnPlayerAttackCountChanged;
            _localDataManager.InvokeAll();
        }

        public override void OnStartLocalPlayer()
        {
            // Activate the main panel
            MyNetworkCanvasUI.SetActive(true);
        }

        public override void OnStopLocalPlayer()
        {
            // Deactivate the main panel
            MyNetworkCanvasUI.SetActive(false);
        }

        public override void OnStopClient()
        {
            _globalDataManager.OnGameTimeChanged -= globalUIManager.OnGlobalTimeValueChanged;

            _localDataManager.OnColorChanged -= localUIManager.OnPlayerColorChanged;
            _localDataManager.OnNumberChanged -= localUIManager.OnPlayerNumberChanged;
            _localDataManager.OnHitCountChanged -= localUIManager.OnPlayerHitCountChanged;
            _localDataManager.OnAttackCountChanged -= localUIManager.OnPlayerAttackCountChanged;

            Destroy(localUIObject);
        }

        #endregion
    }
}
