using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float clickRadius = 1.0f;
    [SerializeField] private float rotationSpeed = 100.0f;
    [SerializeField] private InputAction playerMovement;
    [SerializeField] private InputAction playerClick;
    [SerializeField] private InputAction rightMouseButton;
    [SerializeField] private InputAction mouseDelta;

    private NavMeshAgent agent;
    private Vector2 movementInput;
    private bool isMovingWithInput;
    private bool isRotating;

    private void Start()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
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
        if (isMovingWithInput)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(hit.point, out navHit, clickRadius, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(navHit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    agent.SetDestination(navHit.position);
                }
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