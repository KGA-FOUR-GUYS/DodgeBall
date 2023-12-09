using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class RoleController : MonoBehaviour
{
    public static int IDLE_ANIM_HASH_ID = Animator.StringToHash("Idle");
    public static int RUN_ANIM_HASH_ID = Animator.StringToHash("Run");
    public static int RISING_ANIM_HASH_ID = Animator.StringToHash("RISING_P");
    public static int SCREW_KICK_ANIM_HASH_ID = Animator.StringToHash("ScrewKick");

    private Animator m_Animator;						
    private Animator cachedAnimator
    {
        get
        {
            if (m_Animator == null)
            {
                m_Animator = GetComponent<Animator>();
            }
            return m_Animator;
        }
    }
    private Camera m_MainCamera = null;
    private Camera cachedMainCamara
    {
        get
        {
            if(m_MainCamera == null)
            {
                m_MainCamera = Camera.main;
            }
            return m_MainCamera;
        }
    }
    private Transform m_Transform;
    private Transform cachedTransform
    {
        get
        {
            if(m_Transform == null)
            {
                m_Transform = transform;
            }
            return m_Transform;
        }
    }
    private GhostEffect m_GhostEffectCOM = null;
    private GhostEffect cachedGhostEffect
    {
        get
        {
            if (m_GhostEffectCOM == null)
            {
                m_GhostEffectCOM = GetComponent<GhostEffect>();
            }
            return m_GhostEffectCOM;
        }
    }

    private bool m_Moving = false;

    public float moveSpeed = 6;
    public float turnSpeed = 900;

    private float InputToAngle(Vector2 input)
    {
        Vector3 world_dir = InputToDirection(input);
        float angle = 90 - Mathf.Atan2(world_dir.z, world_dir.x) * Mathf.Rad2Deg;
        if (angle < 0)
        {
            angle += 360;
        }
        return angle;
    }

    private Vector3 InputToDirection(Vector2 input)
    {
        Vector3 world_dir = cachedMainCamara.transform.TransformDirection(new Vector3(input.x, 0, input.y));
        world_dir.y = 0;
        world_dir.Normalize();
        return world_dir;
    }

    void UpdateMove(Vector2 input)
    {
        AnimatorStateInfo currentState = cachedAnimator.GetCurrentAnimatorStateInfo(0);
        if(currentState.shortNameHash != IDLE_ANIM_HASH_ID && currentState.shortNameHash != RUN_ANIM_HASH_ID)
        {
            return;
        }
        cachedAnimator.SetBool(RUN_ANIM_HASH_ID, true);

        //Update Angle
        float targetAngle = InputToAngle(input);
        Vector3 eulerAngles = cachedTransform.eulerAngles;
        float newAngle = Mathf.MoveTowardsAngle(eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
        eulerAngles.y = newAngle;
        cachedTransform.eulerAngles = eulerAngles;

        //Update Motion
        Vector3 forward = cachedTransform.forward;
        float moveDist = moveSpeed * Time.deltaTime;
        Vector3 motion = forward * moveDist;
        Vector3 currentPosition = cachedTransform.position;

        Vector3 nextPosition = motion + currentPosition;
        nextPosition.y += 10;
        RaycastHit raycastHit;
        if (Physics.Raycast(nextPosition, Vector3.down, out raycastHit, 50f, -1))
        {
            nextPosition.y = raycastHit.point.y;
        }
        else
        {
            nextPosition = currentPosition; 
        }

        cachedTransform.position = nextPosition;
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        if (horizontal != 0 || vertical != 0)
        {
            m_Moving = true;
            Vector2 input = new Vector2(horizontal, vertical);
            UpdateMove(input);
        }
        else if (m_Moving)
        {
            m_Moving = false;
            cachedAnimator.SetBool(RUN_ANIM_HASH_ID, false);
        }
    }

    public void run_ghost(int intParameter)
    {
        cachedGhostEffect.CreateGhostEffectObject(Color.red, 1f, 1f, 0.5f, 15f, 0.5f);
    }

    public void risking_ghost()
    {
        cachedGhostEffect.CreateGhostEffectObject(Color.yellow, 1.2f, 1, 0.8f, 1.5f);
    }

    public void screw_ghost_1()
    {
        cachedGhostEffect.CreateGhostEffectObject(Color.blue, 0.8f, 1.2f, 0.8f, 1.5f);
    }

    public void screw_ghost_2()
    {
        cachedGhostEffect.CreateGhostEffectObject(Color.white, 0f, 0f, 1f, 0.85f, 1f);
    }

    void OnGUI()
	{	
		GUI.Box(new Rect(Screen.width - 200 , 45 ,120 , 350), "");
		if(GUI.Button(new Rect(Screen.width - 190 , 60 ,100, 20), "RISING"))
            cachedAnimator.Play(RISING_ANIM_HASH_ID);
		if(GUI.Button(new Rect(Screen.width - 190 , 100 ,100, 20), "SCREW KICK"))
            cachedAnimator.Play(SCREW_KICK_ANIM_HASH_ID);
	}
}
