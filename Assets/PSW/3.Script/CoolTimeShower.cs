using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CoolTimeShower : NetworkBehaviour
{
    [SerializeField] private PlayerControl playerControl;
    [SerializeField] private SkillControl skillControl;
    [SerializeField] private Image OhDaeSick;
    [SerializeField] private Image Ghost;
    [SerializeField] private Image Flash;
    private Color onColor;
    private Color offColor;

    private void Start()
    {
        onColor = Color.white;
        offColor = Color.gray;

    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (skillControl.skillOn)
            {
                OhDaeSick.color = onColor;
            }
            else
            {
                OhDaeSick.color = offColor;
            }

            if (playerControl.canGhost)
            {
                Ghost.color = onColor;
            }
            else
            {
                Ghost.color = offColor;
            }

            if (playerControl.canFlash)
            {
                Flash.color = onColor;
            }
            else
            {
                Flash.color = offColor;
            }
        }
    }
}
