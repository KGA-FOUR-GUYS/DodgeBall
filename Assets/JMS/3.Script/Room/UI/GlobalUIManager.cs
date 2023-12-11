using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkRoom
{
    public class GlobalUIManager : MonoBehaviour
    {
        [Header("Global UI root object")]
        public RectTransform root;

        [Header("Global UI child objects")]
        public Text globalTime;

        public void OnGlobalTimeValueChanged(DateTime newTime)
        {
            globalTime.text = string.Format($"{newTime.Minute:00} : {newTime.Second:00}");

            // 남은 시간에 따라 Color 변경
            // globalTime.color = newColor;
        }
    }
}