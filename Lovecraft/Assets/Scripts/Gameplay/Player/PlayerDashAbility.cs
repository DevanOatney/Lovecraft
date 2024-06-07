using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerDashAbility : MonoBehaviour
{
    [SerializeField] private Transform PlayerVisuals;
    [SerializeField] private InputAction playerDash;
    [SerializeField] private float dashDistance = 5;
    [SerializeField] private float dashCooldown = 3;
    [SerializeField] private Image dashCooldownIndicator;

    private NavMeshAgent agent;
    private bool isDashing = false; 
    private float dashCooldownBucket = 3;

    private void OnEnable()
    {
        playerDash.performed += PerformDash;
        playerDash.Enable();
    }

    private void OnDisable()
    {
        playerDash.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if( dashCooldownBucket < dashCooldown)
        {
            dashCooldownBucket += Time.deltaTime;
            dashCooldownIndicator.fillAmount = dashCooldownBucket / dashCooldown;
        } else
        {
            dashCooldownIndicator.enabled = false;
        }


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

    public bool IsDashing()
    {
        return isDashing;
    }

    public void SetDashing(bool _dashing)
    {
        isDashing = _dashing;
    }
}
