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
            // Add this to the static Players List
            playersList.Add(this);

            // set the Player Color SyncVar
            _localDataManager.color = Random.ColorHSV(0f, 1f, 0.9f, 0.9f, 1f, 1f);

            // set the initial player data
            _localDataManager.hitCount = 0;
            _localDataManager.attackCount = 0;
        }

        public override void OnStopServer()
        {
            playersList.Remove(this);
        }

		#endregion

		#region Client

		public override void OnStartClient()
		{
            _localDataManager.name = SQLManager.Instance.userInfo.ID;

            localUIObject = Instantiate(localUIPrefab, CanvasUIManager.GetPlayersPanel());
            localUIManager = localUIObject.GetComponent<LocalUIManager>();

            _localDataManager.OnColorChanged += localUIManager.OnPlayerColorChanged;
            _localDataManager.OnNameChanged += localUIManager.OnPlayerNameChanged;
            _localDataManager.OnHitCountChanged += localUIManager.OnPlayerHitCountChanged;
            _localDataManager.OnAttackCountChanged += localUIManager.OnPlayerAttackCountChanged;
            _localDataManager.InvokeAll();
        }

		public override void OnStartLocalPlayer()
        {   
            globalUIObject = Instantiate(globalUIPrefab, CanvasUIManager.GetMainPanel());
            globalUIManager = globalUIObject.GetComponent<GlobalUIManager>();

            _globalDataManager.OnGameTimeChanged += globalUIManager.OnGlobalTimeValueChanged;
            
            CanvasUIManager.SetActive(true);
        }

        public override void OnStopLocalPlayer()
        {
            CanvasUIManager.SetActive(false);

            _globalDataManager.OnGameTimeChanged -= globalUIManager.OnGlobalTimeValueChanged;

            Destroy(globalUIObject);
        }

		public override void OnStopClient()
		{
            _localDataManager.OnColorChanged -= localUIManager.OnPlayerColorChanged;
            _localDataManager.OnNameChanged -= localUIManager.OnPlayerNameChanged;
            _localDataManager.OnHitCountChanged -= localUIManager.OnPlayerHitCountChanged;
            _localDataManager.OnAttackCountChanged -= localUIManager.OnPlayerAttackCountChanged;

            Destroy(localUIObject);
        }

		#endregion
	}
}
