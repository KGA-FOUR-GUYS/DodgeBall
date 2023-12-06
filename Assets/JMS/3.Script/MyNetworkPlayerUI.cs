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

    public void Initialize()
    {
        // add a visual background for the local player in the UI
        img_root.color = new Color(1f, 1f, 1f, 0.1f);
    }

    public void OnPlayerNumberChanged(byte newPlayerNumber)
    {
        txt_name.text = string.Format("Player {0:00}", newPlayerNumber);
    }

    public void OnPlayerColorChanged(Color32 newPlayerColor)
    {
        txt_name.color = newPlayerColor;
    }

    public void OnPlayerHitCountChanged(ushort newPlayerHitCount)
    {
        txt_hitCount.text = $"Hit: {newPlayerHitCount}";
    }

    public void OnPlayerAttackCountChanged(ushort newPlayerAttackCount)
    {
        txt_hitCount.text = $"Attack: {newPlayerAttackCount}";
    }
}
