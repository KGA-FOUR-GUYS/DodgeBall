using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyNetworkPlayerUI : MonoBehaviour
{
    [Header("Player UI root object")]
    public Image img_root;

    [Header("Player UI child objects")]
    public Text txt_name;
    public Text txt_hitCount;
    public Text txt_attackCount;

    public void OnPlayerColorChanged(Color32 newPlayerColor)
    {
        txt_name.color = newPlayerColor;
    }

    public void OnPlayerNumberChanged(byte newPlayerNumber)
    {
        txt_name.text = string.Format($"Player {newPlayerNumber:00}");
    }

    public void OnPlayerHitCountChanged(ushort newPlayerHitCount)
    {
        txt_hitCount.text = $"{newPlayerHitCount}";
    }

    public void OnPlayerAttackCountChanged(ushort newPlayerAttackCount)
    {
        txt_attackCount.text = $"{newPlayerAttackCount}";
    }
}
