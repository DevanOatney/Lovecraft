using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float clickRadius = 1.0f;
    [SerializeField] private float rotationSpeed = 100.0f;
    [SerializeField] private InputAction playerMovement;
    [SerializeField] private InputAction playerClick;
    [SerializeField] private Transform PlayerVisuals;
    [SerializeField] private int MaxHP = 100;

    private NavMeshAgent agent;
    private PathfindingCostGrid pfindCostGrid;
    private Vector2 movementInput;
    private bool isMovingWithInput;
    private bool isRotating = true;
    private int curHP = 0;
    private PlayerDashAbility dashAbility;

    public GameObject HitCircle;

    private void Start()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
        pfindCostGrid = GameObject.FindObjectOfType<PathfindingCostGrid>();
        dashAbility = GetComponent<PlayerDashAbility>();
        transform.rotation = Quaternion.identity;
        transform.Rotate(new Vector3(0, 120, 0));
        curHP = MaxHP;
    }

    private void OnEnable()
    {
        playerMovement.performed += OnMove;
        playerMovement.canceled += OnMoveCanceled;
        playerClick.performed += OnClick;
        playerMovement.Enable();
        playerClick.Enable();
    }

    private void OnDisable()
    {
        playerMovement.performed -= OnMove;
        playerMovement.canceled -= OnMoveCanceled;
        playerClick.performed -= OnClick;
        playerMovement.Disable();
        playerClick.Disable();
    }

    private void Update()
    {
        if (RoundController.Instance.IsSceneInitialized() == false)
            return;

        if( dashAbility.IsDashing() )
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        // Done
                        dashAbility.SetDashing(false);
                        agent.speed = 12;
                    }
                }
            }
        } else if (isMovingWithInput)
        {
            Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);
            moveDirection = transform.TransformDirection(moveDirection);
            agent.Move(moveDirection * agent.speed * Time.deltaTime);
        }

        if (isRotating)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000, LayerMask.NameToLayer("Ground")))
            {
                Vector3 LookAtPos = hit.point;
                LookAtPos.y = PlayerVisuals.position.y;
                PlayerVisuals.LookAt(LookAtPos);
                HitCircle.transform.position = LookAtPos;
            }
        }


        // Debug line to show forward direction
        Debug.DrawRay(transform.position, PlayerVisuals.forward * 2, Color.red);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        isMovingWithInput = true;
        agent.isStopped = true;
        agent.ResetPath();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        movementInput = Vector2.zero;
        isMovingWithInput = false;
        agent.isStopped = false;
    }

    private void OnClick(InputAction.CallbackContext context)
    {
    }

    public void TakeDamage(int damageToTake)
    {
        if (curHP > 0)
        {
            curHP -= damageToTake;
            if (curHP < 0)
            {
                GameEventSystem.Instance.TriggerEvent(GameEvent.PLAYER_KILLED);                
            }
        }
    }
}