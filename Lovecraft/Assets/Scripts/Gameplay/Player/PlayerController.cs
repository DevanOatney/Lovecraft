using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float clickRadius = 1.0f;
    [SerializeField] private float rotationSpeed = 100.0f;
    [SerializeField] private InputAction playerMovement;
    [SerializeField] private InputAction playerClick;
    [SerializeField] private InputAction playerDash;
    [SerializeField] private Transform PlayerVisuals;
    [SerializeField] private float dashDistance = 5;
    [SerializeField] private float dashCooldown = 3;
    [SerializeField] private Image dashCooldownIndicator;

    private NavMeshAgent agent;
    private PathfindingCostGrid pfindCostGrid;
    private Vector2 movementInput;
    private bool isMovingWithInput;
    private bool isRotating = true;
    private bool isDashing = false; 
    private float dashCooldownBucket = 3;

    private void Start()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
        pfindCostGrid = GameObject.FindObjectOfType<PathfindingCostGrid>();
    }

    private void OnEnable()
    {
        playerMovement.performed += OnMove;
        playerMovement.canceled += OnMoveCanceled;
        playerClick.performed += OnClick;
        playerDash.performed += PerformDash;
        playerMovement.Enable();
        playerClick.Enable();
        playerDash.Enable();
    }

    private void OnDisable()
    {
        playerMovement.performed -= OnMove;
        playerMovement.canceled -= OnMoveCanceled;
        playerClick.performed -= OnClick;
        playerMovement.Disable();
        playerClick.Disable();
        playerDash.Disable();
    }

    private void Update()
    {
        if( dashCooldownBucket < dashCooldown)
        {
            dashCooldownBucket += Time.deltaTime;
            dashCooldownIndicator.fillAmount = dashCooldownBucket / dashCooldown;
        } else
        {
            dashCooldownIndicator.enabled = false;
        }

        if( isDashing )
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        // Done
                        isDashing = false;
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

    private void PerformDash(InputAction.CallbackContext context)
    {
        if( dashCooldownBucket < dashCooldown) { return; }

        RaycastHit hit;
        Vector3 projectedPosition = transform.position;
        if (Physics.SphereCast(PlayerVisuals.position, 2, PlayerVisuals.forward, out hit, dashDistance))
        {
            Vector3 pos = hit.point;
            Vector3 adjustment = (PlayerVisuals.position - pos).normalized;
            pos += adjustment;

            projectedPosition = pos;
            Debug.Log("hit");
        } else
        {
            projectedPosition = PlayerVisuals.position + PlayerVisuals.forward * dashDistance;
            //transform.position = projectPos;

        }
       
        agent.SetDestination(projectedPosition);
        agent.speed = 100;       
        isDashing = true;
        dashCooldownBucket = 0;

        dashCooldownIndicator.enabled = true;
    }

    public void External_Dash()
    {
        PerformDash(new InputAction.CallbackContext());
    }
}