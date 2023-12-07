using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class PlayerControl : NetworkBehaviour
{
    [Header("Components")]
    public NavMeshAgent agent;
    public TextMesh healthBar;
    public MeshRenderer ColorObj;
    public GameObject ThrowLineObj;

    [Header("Shoot")]
    public KeyCode showLaneKey = KeyCode.Q;
    public GameObject OhDaeSick;
    public Transform ThrowPoint;
    public bool Instant = false;
    public bool ShowLine = false;
    private bool SkillOn = true;
    private bool oneTime = true;
    public float CoolTime = 5f;
    private float cool;

    [Header("Stats")]
    [SyncVar] public int health = 5;
    public float moveSpeed;

    private void Awake()
    {
        cool = CoolTime;
    }

    private void Update()
    {
        healthBar.text = new string('■', health);

        // take input from focused window only
        if (!Application.isFocused) return;

        // movement for local player
        //if (isLocalPlayer)
        {
            ColorObj.material.color = Color.green;
            agent.speed = moveSpeed;

            Move();
            if (Input.GetKeyDown(showLaneKey))
            {
                if (SkillOn)
                {
                    agent.enabled = false;
                    if (Instant)
                    {
                        CMDShootDaeSick();
                    }
                    else
                    {
                        ShowThrowLine();
                    }
                }
            }
            ShowAfterChoice();
            Flash();
        }
    }

    private void Move()
    {
        if (Input.GetMouseButtonDown(1))
        {
            agent.enabled = false;
            agent.enabled = true;
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane DP = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;
            if (DP.Raycast(cameraRay, out rayDistance))
            {
                Vector3 raypoint = cameraRay.GetPoint(rayDistance);
                Vector3 targetPos = new Vector3(raypoint.x, transform.position.y, raypoint.z);                
                agent.SetDestination(targetPos);
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            agent.enabled = false;
        }
    }

    private void Flash()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            agent.enabled = false;
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane DP = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;
            if (DP.Raycast(cameraRay, out rayDistance))
            {
                Vector3 raypoint = cameraRay.GetPoint(rayDistance);
                Vector3 targetPos = new Vector3(raypoint.x, transform.position.y, raypoint.z);
                float mousedis = Vector3.Distance(targetPos, transform.position);
                float Fdis = Mathf.Min(mousedis, 2f);
                Debug.Log(Fdis);
                Debug.Log(targetPos.normalized);
                transform.position = targetPos.normalized * Fdis;
            }
        }
    }


    private void ShowThrowLine()
    {
        ShowLine = true;
        ThrowLineObj.SetActive(ShowLine);
    }

    private void ShowAfterChoice()
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
                transform.rotation = Quaternion.Euler(0, RotValue, 0);
            }
            if (Input.GetMouseButtonDown(0))
            {
                CMDShootDaeSick();
                ShowLine = false;
            }
            if (Input.GetMouseButtonDown(1))
            {
                ShowLine = false;
            }
            ThrowLineObj.SetActive(ShowLine);
        }
    }

    [Command]
    private void CMDShootDaeSick()
    {
        if (SkillOn)
        {
            CoolTime = cool;
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane DP = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;
            if (DP.Raycast(cameraRay, out rayDistance))
            {
                Vector3 raypoint = cameraRay.GetPoint(rayDistance);
                Vector3 targetPos = new Vector3(raypoint.x, transform.position.y, raypoint.z);
                transform.LookAt(targetPos);
            }
            GameObject Daesick = Instantiate(OhDaeSick, ThrowPoint.position, transform.rotation);
            NetworkServer.Spawn(Daesick);
            RPCShooting();
            SkillOn = false;
        }
        if (!SkillOn && oneTime)
        {
            oneTime = false;
            StartCoroutine(Timer());
        }
    }

    private IEnumerator Timer()
    {
        while (CoolTime >= 0)
        {
            CoolTime -= Time.deltaTime;
            yield return null;
        }
        oneTime = true;
        SkillOn = true;
    }

    [ClientRpc]
    private void RPCShooting()
    {
        //슈팅 애니재생해줭.
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<OhDaeSick>() != null)
        {
            --health;
            if (health == 0)
            {
                NetworkServer.Destroy(gameObject);
            }
        }
    }



}
