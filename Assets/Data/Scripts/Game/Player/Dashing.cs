using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Dashing : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navmeshAgent;
    [SerializeField] private Transform orientation;
    [SerializeField] private Camera mainCamera;
    
    [SerializeField] private LayerMask seeThroughLayer;
                     private Rigidbody _rb;
                     private PlayerHealth _playerHealth;
                     private PlayerMovement _playerMovement;
                     private NavMeshAgent _playerNavMesh;
                     private PlayerAnimationHandler _playerAnimationHandler;

    [Header("Dashing")] 
    [SerializeField] private float dashForce;
    [SerializeField] private float dashUpwardForce, dashDuration;

    [Header("Cooldown")] 
    [SerializeField] internal float dashCd;
                     private float _dashCdTimer;

    [Header("Settings")] 
    [SerializeField] private bool allowAllDirections = true; 
    [SerializeField] private bool disableGravity = true, resetVel = true;
    [SerializeField] private bool dashUpgraded;
    [SerializeField] public int shieldAmount = 50;
    [SerializeField] private KeyCode eKey = KeyCode.E;
    private Animator _animator;
    private bool cantUseE = false;


    private void Start()
    {
        navmeshAgent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _playerHealth = GetComponent<PlayerHealth>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerNavMesh = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _playerAnimationHandler = GetComponentInChildren<PlayerAnimationHandler>();
    }

    private void Update()
    {
        cantUseE = _playerAnimationHandler.cantUseOtherAbilities;
        
        if(Input.GetKeyUp(eKey))
            DashInput();
        if (_dashCdTimer > 0)
            _dashCdTimer -= Time.deltaTime;
    }

    private void DashInput()
    {
        if (_dashCdTimer <= 0 && !cantUseE)
        {
            RaycastHit hit;

            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100, ~seeThroughLayer))
            {
                transform.LookAt(new Vector3(hit.point.x, gameObject.transform.position.y, hit.point.z));
                _playerNavMesh.ResetPath();
                StartCoroutine(DeactivateNavMesh(0.4f));
                Dash(hit);
                
                if(dashUpgraded)
                {
                    _playerHealth.GiveShield(shieldAmount);
                }
                _playerAnimationHandler.cantUseOtherAbilities = true;
                _animator.SetTrigger("Dash");
            }

            
        }
    }

    private void Dash(RaycastHit hit)
    {
        if (_dashCdTimer > 0) return;
        else
        {
            _dashCdTimer = dashCd;
        }
        _playerMovement.dashing = true;

        Transform forwardT = hit.transform;
         
        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = gameObject.transform.forward * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
            _rb.useGravity = false;

        _delayedForceToApply = forceToApply;
        
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 _delayedForceToApply;
    private void DelayedDashForce()
    {
        if (resetVel)
            _rb.velocity = Vector3.zero;

        _rb.AddForce(_delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        _playerMovement.dashing = false;

        if (disableGravity)
            _rb.useGravity = true;
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
    
    public void KeyChanger(int option)
    {
        if (option == 1)
        {
            eKey = KeyCode.E;
        }
        else if (option == 2)
        {
            eKey = KeyCode.Alpha3;
        }
    }

}
