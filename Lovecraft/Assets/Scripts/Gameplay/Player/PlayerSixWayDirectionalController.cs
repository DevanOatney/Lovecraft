using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public enum MovementControlCombo
{
    SINGLE_W,
    SINGLE_A,
    SINGLE_S,
    SINGLE_D,
    COMBO_WA,
    COMBO_AS,
    COMBO_SD,
    COMBO_DW,
    NONE,
}

public class PlayerSixWayDirectionalController : MonoBehaviour
{
    [SerializeField] private float clickRadius = 1.0f;
    [SerializeField] private float rotationSpeed = 100.0f;
    [SerializeField] private InputAction playerMovement;
    [SerializeField] private InputAction playerClick;
    [SerializeField] private int MaxHP = 100;
    [SerializeField] private Transform PlayerVisuals;

    private NavMeshAgent agent;
    private Vector2 movementInput;
    private bool isMovingWithInput;
    private int curHP = 0;
    private PlayerDashAbility dashAbility;
    private MovementControlCombo movementControl;
    private MovementControlCombo prevMovementControl;

    public GameObject HitCircle;

    private void Start()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
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
            Vector2 clampedDir = Vector2.zero;
            float eulerRotation = 0f;

            switch(movementControl)
            {
                case MovementControlCombo.SINGLE_W:
                    clampedDir = new Vector2(0,1);
                    break;
                case MovementControlCombo.SINGLE_A:
                    clampedDir = new Vector2(-1,0);
                    eulerRotation = 270;
                    break;
                case MovementControlCombo.SINGLE_S:
                    clampedDir = new Vector2(0,-1);
                    eulerRotation = 180;
                    break;
                case MovementControlCombo.SINGLE_D:
                    clampedDir = new Vector2(1,0);
                    eulerRotation = 90;
                    break;
                case MovementControlCombo.COMBO_WA:
                    clampedDir = new Vector2(-1,1);
                    eulerRotation = 330;
                    break;
                case MovementControlCombo.COMBO_AS:
                    clampedDir = new Vector2(-1,-1);
                    eulerRotation = 240;
                    break;
                case MovementControlCombo.COMBO_SD:
                    clampedDir = new Vector2(1,-1);
                    eulerRotation = 150;
                    break;
                case MovementControlCombo.COMBO_DW:
                    clampedDir = new Vector2(1,1);
                    eulerRotation = 60;
                    break;
            }

            Vector3 moveDirection = new Vector3(clampedDir.x, 0, clampedDir.y);
            moveDirection = transform.TransformDirection(moveDirection);
            agent.Move(moveDirection * agent.speed * Time.deltaTime);

            if (prevMovementControl != movementControl)
            {
                PlayerVisuals.localRotation = Quaternion.Euler(0, eulerRotation, 0);
                prevMovementControl = movementControl;
            }
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();

        if( movementInput.x == 0 )
        {
            if( movementInput.y > 0 )
            {
                movementControl = MovementControlCombo.SINGLE_W;
            } else
            {
                movementControl = MovementControlCombo.SINGLE_S;
            }
        } else if( movementInput.y == 0 )
        {
            if (movementInput.x > 0)
            {
                movementControl = MovementControlCombo.SINGLE_D;
            }
            else
            {
                movementControl = MovementControlCombo.SINGLE_A;
            }
        } else
        {
            if( movementInput.x > 0 )
            {
                if( movementInput.y > 0 )
                {
                    movementControl = MovementControlCombo.COMBO_DW;
                } else
                {
                    movementControl = MovementControlCombo.COMBO_SD;
                }
            } else
            {
                if (movementInput.y > 0)
                {
                    movementControl = MovementControlCombo.COMBO_WA;
                }
                else
                {
                    movementControl = MovementControlCombo.COMBO_AS;
                }
            }
        }

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
        curHP -= damageToTake;
        Debug.Log(curHP);
        if(curHP < 0)
        {
            SceneManager.Instance.ReloadCurrentScene();
        }
    }
}