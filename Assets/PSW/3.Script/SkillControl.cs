using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillControl : MonoBehaviour
{
    public GameObject ThrowLineObj;
    public bool ShowLine = false;
    public bool skillOn = true;
    public float CoolTime = 5f;
    public PlayerControl playerControl;


    public void ShowThrowLine()
    {
        ShowLine = true;
        ThrowLineObj.SetActive(ShowLine);
    }

    public void ShowAfterChoice()
    {
        if (ShowLine)
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane DP = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;
            if (DP.Raycast(cameraRay, out rayDistance))
            {
                Vector3 raypoint = cameraRay.GetPoint(rayDistance);
                Vector3 lookPos = raypoint - transform.position;
                float RotValue = Mathf.Atan2(lookPos.x, lookPos.z) * Mathf.Rad2Deg;
                ThrowLineObj.transform.rotation = Quaternion.Euler(0, RotValue, 0);
            }
            if (Input.GetMouseButtonDown(0))
            {
                ShootOHDaeSick();
                ShowLine = false;
            }
            if (Input.GetMouseButtonDown(1))
            {
                ShowLine = false;
            }
            ThrowLineObj.SetActive(ShowLine);
        }

    }
    public void ShootOHDaeSick()
    {
        if (skillOn)
        {
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane DP = new Plane(Vector3.up, Vector3.zero);
            float rayLength;
            if (DP.Raycast(camRay, out rayLength))
            {
                Vector3 rayPoint = camRay.GetPoint(rayLength);
                Vector3 targetPoint = new Vector3(rayPoint.x, transform.position.y, rayPoint.z);
                transform.LookAt(targetPoint);
            }
            playerControl.CMDGenOhDaeSick();
            StartCoroutine(CoolDown());
            return;
        }
        else if (!skillOn)
        {
            return;
        }
    }
    private IEnumerator CoolDown()
    {
        skillOn = false;
        yield return new WaitForSeconds(CoolTime);
        skillOn = true;
    }



}
