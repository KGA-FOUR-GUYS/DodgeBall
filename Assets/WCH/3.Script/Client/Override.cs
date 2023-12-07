using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Override : NetworkRoomManager
{
    private RoomManager Rm;
    public override void OnRoomServerPlayersReady()
    {
        Rm = FindObjectOfType<RoomManager>();

        Rm.isAllReady();
    }
}
