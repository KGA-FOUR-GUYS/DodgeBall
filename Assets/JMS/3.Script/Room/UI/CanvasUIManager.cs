using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasUIManager : MonoBehaviour
{
    public RectTransform mainPanel;
    public RectTransform playersPanel;

    public static CanvasUIManager Instance;

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
        if (Instance == null) return;

        Instance.mainPanel.gameObject.SetActive(active);
    }

    public static RectTransform GetMainPanel() => Instance.mainPanel;
    public static RectTransform GetPlayersPanel() => Instance.playersPanel;
}
