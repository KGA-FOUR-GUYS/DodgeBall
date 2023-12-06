using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkCanvasUI : MonoBehaviour
{
    public RectTransform mainPanel;
    public RectTransform playersPanel;

    private static MyNetworkCanvasUI Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != null)
        {
            Destroy(this);
        }
    }

    public static void SetActive(bool active)
    {
        Instance.mainPanel.gameObject.SetActive(active);
    }

    public static RectTransform GetPlayersPanel() => Instance.playersPanel;
}
