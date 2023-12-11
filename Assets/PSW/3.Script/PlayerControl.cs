using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class PlayerControl : NetworkBehaviour
{
    [Header("Components")]
    public NavMeshAgent agent;
    public Animator animator;
    public TextMesh healthBar;
    public SkinnedMeshRenderer MundoSkin;
    public Material[] ColorObj;
    public SkillControl skillControl;
    private GhostEffect ghostEffect;

    [Header("Shoot")]
    public KeyCode showLaneKey = KeyCode.Q;
    public GameObject OhDaeSick;
    public Transform ThrowPoint;
    public bool Instant = false;
    private bool oneTime = true;
    public float CoolTime = 5f;
    private bool canFlash = true;
    private bool canGhost = true;

    [Header("Stats")]
    [SyncVar] public int health = 5;
    public float moveSpeed;
    private float Speed;

    private void Awake()
    {
        Speed = moveSpeed;
        ghostEffect = GetComponent<GhostEffect>();
    }

    private void Update()
    {
        healthBar.text = new string('■', health);

        // take input from focused window only
        if (!Application.isFocused) return;


        if (isLocalPlayer)
        {
            MundoSkin.material = ColorObj[1];
            agent.speed = moveSpeed;

            //이동
            Move();
            CMDMove();

            //오대식
            if (Input.GetKeyDown(showLaneKey))
            {
                if (skillControl.skillOn)
                {
                    agent.enabled = false;
                    //스마트키 On
                    if (Instant)
                    {
                        skillControl.ShootOHDaeSick();
                    }
                    //스마트키 Off
                    else
                    {
                        skillControl.ShowThrowLine();
                    }
                }
            }
            skillControl.ShowAfterChoice();

            //점멸
            Flash();

            //유체화
            if (canGhost && Input.GetKeyDown(KeyCode.D))
            {
                StartCoroutine(Ghost());
            }
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
        if (canFlash && Input.GetKeyDown(KeyCode.F))
        {
            canFlash = false;
            agent.enabled = false;


            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane DP = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;
            if (DP.Raycast(cameraRay, out rayDistance))
            {
                Vector3 raypoint = cameraRay.GetPoint(rayDistance);
                Vector3 mousePos = new Vector3(raypoint.x, transform.position.y, raypoint.z);
                Vector3 targetPos = mousePos - transform.position;

                float mouseDistance = targetPos.magnitude;
                float FlashDistance = Mathf.Min(mouseDistance, 2f);

                transform.position += targetPos.normalized*FlashDistance;
                transform.LookAt(mousePos);
            }
        }
    }
    private IEnumerator Ghost()
    {
        canGhost = false;
        moveSpeed = moveSpeed * 1.24f;
        float ghostTime = 15f;
        while (ghostTime > 0)
        {
            ghostTime -= Time.deltaTime;
            ghostEffect.CreateGhostEffectObject(Color.cyan, 5f, 0.5f, 0.5f, 0.3f);
            yield return null;
        }
        moveSpeed = Speed;
    }


    [Command]
    private void CMDMove()
    {
        RPCWalking();
    }

    [Command]
    public void CMDGenOhDaeSick()
    {
        RPCShooting();
        Invoke(nameof(SpawnOhDaeSick), 0.41f);
    }

    private void SpawnOhDaeSick()
    {
        GameObject Daesick = Instantiate(OhDaeSick, ThrowPoint.position, transform.rotation);
        NetworkServer.Spawn(Daesick);
    }


    [ClientRpc]
    private void RPCWalking()
    {
        animator.SetBool("HasTarget", agent.velocity != Vector3.zero);
    }

    [ClientRpc]
    private void RPCShooting()
    {
        animator.SetTrigger("Throw");
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

    private void AniSpeed(float speed)
    {
        animator.speed = speed;
    }
}
