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
    [SerializeField] private Transform PlayerVisuals;

    private NavMeshAgent agent;
    private PathfindingCostGrid pfindCostGrid;
    private Vector2 movementInput;
    private bool isMovingWithInput;
    private bool isRotating = true;
    private PlayerDashAbility dashAbility;

    private void Start()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
        pfindCostGrid = GameObject.FindObjectOfType<PathfindingCostGrid>();
        dashAbility = GetComponent<PlayerDashAbility>();
        transform.rotation = Quaternion.identity;
        transform.Rotate(new Vector3(0, 120, 0));
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

    public void TakeDamage(int damageToTake)
    {
        int x = 5;
    }
}