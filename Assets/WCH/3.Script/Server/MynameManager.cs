using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkRoom;
using Mirror;

public class MynameManager : NetworkBehaviour
{
    private SQLManager SqlManager;
    [SyncVar]
    public string myname;
    private void Start()
    {
        SqlManager = SQLManager.Instance;
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            myname = SqlManager.userInfo.ID;
        }
    }
}
