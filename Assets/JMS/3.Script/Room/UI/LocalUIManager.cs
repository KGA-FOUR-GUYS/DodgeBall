using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkRoom
{
    public class LocalUIManager : MonoBehaviour
    {
        [Header("Local UI root object")]
        public Image img_root;

        [Header("Local UI child objects")]
        public Text userName;
        public Text hitCount;
        public Text attackCount;

        public void OnPlayerColorChanged(Color32 newPlayerColor)
        {
            userName.color = newPlayerColor;
        }

        public void OnPlayerNameChanged(string newPlayerName)
        {
            userName.text = string.Format($"{newPlayerName}");
        }

        public void OnPlayerHitCountChanged(ushort newHitCount)
        {
            hitCount.text = $"{newHitCount}";
        }

        public void OnPlayerAttackCountChanged(ushort newAttackCount)
        {
            attackCount.text = $"{newAttackCount}";
        }

        public void OnPlayerGameTimeChanged(DateTime dateTime)
        {
            Debug.Log($"Time : {dateTime.ToLongTimeString()}");
        }
    }
}