using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MyNetworkPlayer : NetworkBehaviour
{
    // Server -> All Clients
    public event Action OnGameTimeChanged;

    // Client -> Server -> Other Clients
    public event Action<byte> OnPlayerNumberChanged;
    public event Action<Color32> OnPlayerColorChanged;
    public event Action<ushort> OnPlayerHitCountChanged;
    public event Action<ushort> OnPlayerAttackCountChanged;

    // Players List to manage playerNumber
    static readonly List<MyNetworkPlayer> playersList = new List<MyNetworkPlayer>();

    [Header("Player UI")]
    public GameObject playerUIPrefab;

    GameObject playerUIObject;
    MyNetworkPlayerUI playerUI;

    #region SyncVars

    [Header("SyncVars")]

    [SyncVar(hook = nameof(PlayerNumberChanged))]
    public byte playerNumber = 0;

    [SyncVar(hook = nameof(PlayerColorChanged))]
    public Color32 playerColor = Color.white;

    [SyncVar(hook = nameof(PlayerHitCountChanged))]
    public ushort playerHitCount = 0;

    [SyncVar(hook = nameof(PlayerAttackCountChanged))]
    public ushort playerAttackCount = 0;

    void PlayerNumberChanged(byte _, byte newData)
    {
        OnPlayerNumberChanged?.Invoke(newData);
    }

    void PlayerColorChanged(Color32 _, Color32 newColor)
    {
        OnPlayerColorChanged?.Invoke(newColor);
    }

    void PlayerHitCountChanged(ushort _, ushort newData)
    {
        OnPlayerHitCountChanged?.Invoke(newData);
    }

    void PlayerAttackCountChanged(ushort _, ushort newData)
    {
        OnPlayerAttackCountChanged?.Invoke(newData);
    }

    #endregion

    #region Server

    public override void OnStartServer()
    {
        //// Add this to the static Players List
        playersList.Add(this);

        // set the Player Color SyncVar
        playerColor = Random.ColorHSV(0f, 1f, 0.9f, 0.9f, 1f, 1f);

        // set the initial player data
        playerHitCount = 0;
        playerAttackCount = 0;
    }

    [ServerCallback]
    internal static void ResetPlayerNumbers()
    {
        byte playerNumber = 0;
        foreach (MyNetworkPlayer player in playersList)
            player.playerNumber = playerNumber++;
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
        playerUIObject = Instantiate(playerUIPrefab, MyNetworkCanvasUI.GetPlayersPanel());
        playerUI = playerUIObject.GetComponent<MyNetworkPlayerUI>();

        // wire up all events to handlers in PlayerUI
        OnPlayerColorChanged = playerUI.OnPlayerColorChanged;
        OnPlayerNumberChanged = playerUI.OnPlayerNumberChanged;
        OnPlayerHitCountChanged = playerUI.OnPlayerHitCountChanged;
        OnPlayerAttackCountChanged = playerUI.OnPlayerAttackCountChanged;

        // Invoke all event handlers with the initial data from spawn payload
        OnPlayerNumberChanged.Invoke(playerNumber);
        OnPlayerColorChanged.Invoke(playerColor);
        OnPlayerHitCountChanged.Invoke(playerHitCount);
        OnPlayerAttackCountChanged.Invoke(playerAttackCount);
    }

    public override void OnStartLocalPlayer()
    {
		// Activate the main panel
		MyNetworkCanvasUI.SetActive(true);
	}

    public override void OnStopLocalPlayer()
    {
        // Disable the main panel for local player
        MyNetworkCanvasUI.SetActive(false);
    }

    public override void OnStopClient()
    {
        // disconnect event handlers
        OnPlayerNumberChanged = null;
        OnPlayerColorChanged = null;
        OnPlayerHitCountChanged = null;
        OnPlayerAttackCountChanged = null;

        // Remove this player's UI object
        Destroy(playerUIObject);
    }

    #endregion
}
