using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CoolTimeShower : NetworkBehaviour
{
    private GameObject g;
    [SerializeField] private Text text;
    [SerializeField] private SkillControl skillControl;

    private void Start()
    {        
        text = FindObjectOfType<Text>();
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (skillControl.skillOn)
            {
                text.color = Color.white;
                text.text = "On";
            }
            else
            {
                text.color = Color.red;
                text.text = "Off";
            }
        }
    }
}
