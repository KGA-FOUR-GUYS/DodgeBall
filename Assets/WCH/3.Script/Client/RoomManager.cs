using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Examples.NetworkRoom;
using NetworkRoom;

public class RoomManager : MonoBehaviour
{

    //private GameObject RM_Singleton;
    private NTRoomManager room_manager;

    private GameObject P1Slot;
    private MynameManager P1name;
    private NetworkRoomPlayer P1Component;

    private GameObject P2Slot;
    private MynameManager P2name;
    private NetworkRoomPlayer P2Component;

    [SerializeField] private Text p1name_Text;
    [SerializeField] private Image p1img;
    [SerializeField] private Text p1Ready;
    [SerializeField] private Text p2name_Text;
    [SerializeField] private Image p2img;
    [SerializeField] private Text p2Ready;

    [SerializeField] private Button ReadyBtn;
    [SerializeField] private Text CountDown_Text;
    private Coroutine CountDown_co;

    private void Start()
    {
        //RM_Singleton = GameObject.Find("NetworkRoomManager");
        //room_manager = RM_Singleton.GetComponent<NetworkRoomManagerExt>();
        room_manager = NTRoomManager.Instance;
        CountDown_co = null;
        ReadyBtn.gameObject.SetActive(true);
        CountDown_Text.enabled = false;
    }

    private void Update()
    {
        UpdateUI();
        isAllReady();
        Disconnet3rdPlayer();
    }

    private void UpdateUI()
    {
        if(room_manager.roomSlots.Count == 0)
        {
            p1name_Text.enabled = false;
            p1img.enabled = false;
            p1Ready.enabled = false;
            p2name_Text.enabled = false;
            p2img.enabled = false;
            p2Ready.enabled = false;
        }
        else if (room_manager.roomSlots.Count == 1)
        {
            P1Slot = room_manager.roomSlots[0].gameObject;
            P1Component = P1Slot.GetComponent<NetworkRoomPlayer>();
            P1name = P1Slot.GetComponent<MynameManager>();

            p1name_Text.enabled = true;
            p1img.enabled = true;
            p1Ready.enabled = true;
            if (P1Component.readyToBegin)
            {
                p1Ready.text = "Ready";
            }
            else
            {
                p1Ready.text = string.Empty;
            }
            //p1name_Text.text = $"Player[{P1Component.index}]";
            p1name_Text.text = P1name.myname;

            p2name_Text.enabled = false;
            p2img.enabled = false;
            p2Ready.enabled = false;
        }

        else if(room_manager.roomSlots.Count == 2)
        {
            P1Slot = room_manager.roomSlots[0].gameObject;
            P1Component = P1Slot.GetComponent<NetworkRoomPlayer>();
            P1name = P1Slot.GetComponent<MynameManager>();

            p1name_Text.enabled = true;
            p1img.enabled = true;
            p1Ready.enabled = true;
            if (P1Component.readyToBegin)
            {
                p1Ready.text = "Ready";
            }
            else
            {
                p1Ready.text = string.Empty;
            }
            //p1name_Text.text = $"Player[{P1Component.index}]";
            p1name_Text.text = P1name.myname;



            P2Slot = room_manager.roomSlots[1].gameObject;
            P2Component = P2Slot.GetComponent<NetworkRoomPlayer>();
            P2name = P2Slot.GetComponent<MynameManager>();

            p2name_Text.enabled = true;
            p2img.enabled = true;
            p2Ready.enabled = true;
            if (P2Component.readyToBegin)
            {
                p2Ready.text = "Ready";
            }
            else
            {
                p2Ready.text = string.Empty;
            }
            //p2name_Text.text = $"Player[{P2Component.index}]";
            p2name_Text.text = P2name.myname;

        }
        else
        {
            return;
        }
    }

    public void toggle_Ready_Btn() 
    {
        if (P1Component != null)
        {
            if (P1Component.isLocalPlayer)
            {
                if (P1Component.readyToBegin)
                {
                    P1Component.CmdChangeReadyState(false);
                }
                else
                {
                    P1Component.CmdChangeReadyState(true);
                }
            }
        }
        if (P2Component != null)
        {
            if (P2Component.isLocalPlayer)
            {
                if (P2Component.readyToBegin)
                {
                    P2Component.CmdChangeReadyState(false);
                }
                else
                {
                    P2Component.CmdChangeReadyState(true);
                }
            }
        }
    }

    [ServerCallback]
    public void Disconnet3rdPlayer()
    {
        if (room_manager.roomSlots.Count == 3)
        {
            GameObject P3Slot = room_manager.roomSlots[2].gameObject;
            NetworkRoomPlayer P3Component = P3Slot.GetComponent<NetworkRoomPlayer>();
            //NetworkConnectionToClient conn = P3Slot.GetComponent<NetworkConnectionToClient>();
            //GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
            //conn.Disconnect();
            //Transport.active.ServerDisconnect(conn.connectionId);

            P3Component.GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
        }
    }

    public void isAllReady()
    {
        if (room_manager.roomSlots.Count == 2)
        {
            if (P1Component.readyToBegin && P2Component.readyToBegin && CountDown_co == null)
            {
                ReadyBtn.gameObject.SetActive(false);
                CountDown_Text.enabled = true;
                CountDown_co = StartCoroutine(StartCount());
            }
        }
        else
            return;
    }

    private IEnumerator StartCount()
    {
        CountDown_Text.text = "5";
        yield return new WaitForSeconds(1f);
        CountDown_Text.text = "4";
        yield return new WaitForSeconds(1f);
        CountDown_Text.text = "3";
        yield return new WaitForSeconds(1f);
        CountDown_Text.color = Color.red;
        CountDown_Text.text = "2";
        yield return new WaitForSeconds(1f);
        CountDown_Text.text = "1";
        yield return new WaitForSeconds(1f);
        CountDown_Text.text = "0";
        room_manager.ServerChangeScene(room_manager.GameplayScene);
        yield break;

    }
}