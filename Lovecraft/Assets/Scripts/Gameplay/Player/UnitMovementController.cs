using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovementController : MonoBehaviour
{
    [SerializeField] private float clickRadius = 1.0f;
    [SerializeField] private float rotationSpeed = 100.0f;
    [SerializeField] private InputAction playerMovement;
    [SerializeField] private InputAction playerClick;
    [SerializeField] private InputAction rightMouseButton;
    [SerializeField] private InputAction mouseDelta;

    private NavMeshAgent agent;
    private PathfindingCostGrid pfindCostGrid;
    private Vector2 movementInput;
    private bool isMovingWithInput;
    private bool isRotating;

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
        rightMouseButton.performed += OnRightMouseButtonDown;
        rightMouseButton.canceled += OnRightMouseButtonUp;
        playerMovement.Enable();
        playerClick.Enable();
        rightMouseButton.Enable();
        mouseDelta.Enable();
    }

    private void OnDisable()
    {
        playerMovement.performed -= OnMove;
        playerMovement.canceled -= OnMoveCanceled;
        playerClick.performed -= OnClick;
        rightMouseButton.performed -= OnRightMouseButtonDown;
        rightMouseButton.canceled -= OnRightMouseButtonUp;
        playerMovement.Disable();
        playerClick.Disable();
        rightMouseButton.Disable();
        mouseDelta.Disable();
    }

    private void Update()
    {
        if (isMovingWithInput)
        {
            Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);
            moveDirection = transform.TransformDirection(moveDirection);
            agent.Move(moveDirection * agent.speed * Time.deltaTime);
        }

        if (isRotating)
        {
            Vector2 delta = mouseDelta.ReadValue<Vector2>();
            float rotation = delta.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, rotation, 0);
        }

        // Debug line to show forward direction
        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
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
        if (isMovingWithInput || Input.GetKeyDown(KeyCode.LeftControl) || MouseInputManager.Instance.IsPointerOverUIElement())
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(hit.point, out navHit, clickRadius, NavMesh.AllAreas))
            {
                if (pfindCostGrid)
                {
                    List<Vector3> path = pfindCostGrid.FindPath(agent, agent.transform.position, navHit.position);
                    if (path.Count > 0)
                    {
                        StopAllCoroutines();
                        StartCoroutine(FollowPath(path));
                    }
                }
                else
                {
                    NavMeshPath path = new NavMeshPath();
                    if (agent.CalculatePath(navHit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                    {
                        agent.SetDestination(navHit.position);
                    }
                }
            }
        }
    }

    private IEnumerator FollowPath(List<Vector3> path)
    {
        foreach (Vector3 point in path)
        {
            agent.SetDestination(point);
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }
        }
    }


    private void OnRightMouseButtonDown(InputAction.CallbackContext context)
    {
        isRotating = true;
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Hide the cursor
    }

    private void OnRightMouseButtonUp(InputAction.CallbackContext context)
    {
        isRotating = false;
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Show the cursor
    }
}