using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
using NetworkRoom;

[RequireComponent(typeof(LocalDataManager))]
[RequireComponent(typeof(GlobalDataManager))]
public class PlayerControl : NetworkBehaviour
{
    // Players List to manage playerNumber
    static readonly List<PlayerControl> playersList = new List<PlayerControl>();

    private GlobalDataManager _globalDataManager;
    private LocalDataManager _localDataManager;

    [Header("Player UI")]
    public GameObject globalUIPrefab;
    public GameObject localUIPrefab;

    GameObject globalUIObject;
    GameObject localUIObject;
    GlobalUIManager globalUIManager;
    LocalUIManager localUIManager;

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
    public bool canFlash = true;
    public bool canGhost = true;

    [Header("Stats")]
    [SyncVar] public int health = 5;
    public float moveSpeed;
    private float Speed;

    public void IncreaseAttackCount()
    {
        _localDataManager.attackCount++;
    }

    public void IncreaseHitCount()
    {
        _localDataManager.hitCount++;
    }

    private void Awake()
    {
        Speed = moveSpeed;
        ghostEffect = GetComponent<GhostEffect>();
    }

    private void Start()
    {
        TryGetComponent(out _globalDataManager);
        TryGetComponent(out _localDataManager);

        // Add this to the static Players List
        playersList.Add(this);

        CreateLocalUI();
        CreateGlobalUI();
        CanvasUIManager.SetActive(true);

        // set the Player Color SyncVar
        _localDataManager.color = Random.ColorHSV(0f, 1f, 0.9f, 0.9f, 1f, 1f);

        // set the initial player data
        _localDataManager.hitCount = 0;
        _localDataManager.attackCount = 0;
    }

    private void OnDestroy()
    {
        playersList.Remove(this);

        // 승리화면 출력
        if (!isServer)
            CmdShowResult(SQLManager.Instance.userInfo.ID);

        CanvasUIManager.SetActive(false);
        DestroyLocalUI();
        DestroyGlobalUI();
    }

    [Command(requiresAuthority = false)]
    public void CmdShowResult(string loserID)
    {
        RPCShowResult(loserID);
        Debug.Log("[CmdShowResult]");
    }

    [ClientRpc]
    public void RPCShowResult(string loserID)
    {
        Debug.Log("[RPCShowResult]");

        if (loserID.Equals(SQLManager.Instance.userInfo.ID))
        {
            Debug.Log("내가 졌따!");
        }
        else
        {
            Debug.Log("내가 이겻따!");
        }
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
        IncreaseAttackCount();
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
            IncreaseHitCount();

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

    private void CreateLocalUI()
    {
        if (!isServer && !isLocalPlayer) return;

        if (!isServer) _localDataManager.name = SQLManager.Instance.userInfo.ID;

        localUIObject = Instantiate(localUIPrefab, CanvasUIManager.GetPlayersPanel());
        localUIManager = localUIObject.GetComponent<LocalUIManager>();

        _localDataManager.OnColorChanged += localUIManager.OnPlayerColorChanged;
        _localDataManager.OnNameChanged += localUIManager.OnPlayerNameChanged;
        _localDataManager.OnHitCountChanged += localUIManager.OnPlayerHitCountChanged;
        _localDataManager.OnAttackCountChanged += localUIManager.OnPlayerAttackCountChanged;
        _localDataManager.InvokeAll();
    }

    private void DestroyLocalUI()
    {
        if (!isServer && !isLocalPlayer) return;

        _localDataManager.OnColorChanged -= localUIManager.OnPlayerColorChanged;
        _localDataManager.OnNameChanged -= localUIManager.OnPlayerNameChanged;
        _localDataManager.OnHitCountChanged -= localUIManager.OnPlayerHitCountChanged;
        _localDataManager.OnAttackCountChanged -= localUIManager.OnPlayerAttackCountChanged;

        Destroy(localUIObject);
    }

    private void CreateGlobalUI()
    {
        if (!isServer && !isLocalPlayer) return;

        globalUIObject = Instantiate(globalUIPrefab, CanvasUIManager.GetMainPanel());
        globalUIManager = globalUIObject.GetComponent<GlobalUIManager>();

        _globalDataManager.OnGameTimeChanged += globalUIManager.OnGlobalTimeValueChanged;
    }

    private void DestroyGlobalUI()
    {
        if (!isServer && !isLocalPlayer) return;

        _globalDataManager.OnGameTimeChanged -= globalUIManager.OnGlobalTimeValueChanged;

        Destroy(globalUIObject);
    }

    public void SmartkeyBtn()
    {
        if (Instant)
        {
            Instant = false;
        }
        else
        {
            Instant = true;
        }
    }
}
