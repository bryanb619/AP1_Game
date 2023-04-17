using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dashing : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navmeshAgent;
    [SerializeField] private Transform orientation;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask SeeThroughLayer;
                     private Rigidbody rb;
                     private PlayerMovement pm;
                     private NavMeshAgent playerNavMesh;

    [Header("Dashing"), SerializeField]
                     private float dashForce;
    [SerializeField] private float dashUpwardForce, dashDuration;

    [Header("Cooldown"), SerializeField]
                     internal float dashCd;
                     private float dashCdTimer;

    [Header("Settings"), SerializeField]
                     private bool allowAllDirections = true; 
                     private bool shieldUpgrade = false;
    [SerializeField] private bool disableGravity = true, resetVel = true;
    [SerializeField] private bool dashUpgraded;
    [SerializeField] private int shieldAmount = 50;


    private void Start()
    {
        navmeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        playerNavMesh = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        DashInput();
    }

    private void DashInput()
    {
        if (Input.GetButtonDown("Dash") && dashCdTimer <= 0)
        {
            RaycastHit hit;

            

            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100, ~SeeThroughLayer))
            {
                transform.LookAt(new Vector3(hit.point.x, 0, hit.point.z));
                playerNavMesh.ResetPath();
                StartCoroutine(DeactivateNavMesh(0.4f));
                Dash(hit);
                
                if(dashUpgraded)
                {
                    pm.GiveShield(shieldAmount);
                }
            }

            
        }

        if (dashCdTimer > 0)
            dashCdTimer -= Time.deltaTime;
    }

    private void Dash(RaycastHit hit)
    {
        if (dashCdTimer > 0) return;
        else
        {
            dashCdTimer = dashCd;
        }
        pm.dashing = true;

        Transform forwardT = hit.transform;
         
        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = gameObject.transform.forward * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
            rb.useGravity = false;

        delayedForceToApply = forceToApply;
        
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;
    private void DelayedDashForce()
    {
        if (resetVel)
            rb.velocity = Vector3.zero;

        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        pm.dashing = false;

        if (disableGravity)
            rb.useGravity = true;
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if (allowAllDirections)
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        else
            direction = forwardT.forward;

        if (verticalInput == 0 && horizontalInput == 0)
            direction = forwardT.forward;

        return direction.normalized;
    }

    IEnumerator DeactivateNavMesh(float time)
    {
        navmeshAgent.enabled = false;
        //Debug.Log("Deactivated NavMeshAgent");
        yield return new WaitForSeconds(time);
        navmeshAgent.enabled = true;
        //Debug.Log("Activated NavMeshAgent");
    }

    internal void EnableUpgrade()
    {
        dashUpgraded = true;
    }

}
