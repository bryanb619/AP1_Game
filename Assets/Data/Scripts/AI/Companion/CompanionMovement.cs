using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompanionMovement : MonoBehaviour
{

    // Player
    [SerializeField] private Player_ _Player;
    
    // agent
    [SerializeField] private NavMeshAgent _Agent;
    // script
    [SerializeField] private Companion _Companion;


    [SerializeField] private Transform _PlayerFollow;

    private Vector3 _Position;

    public RaycastHit hit;

    // editable variables
    [Header("configuration for iddle")]
    [Range(0f, 5)] private float _RotationSpeed = 1f;

    [Header("Configuration for Follow")]
    private float _FollowRadius = 2f;

    private Coroutine _MoveCoroutine;
    // change of state
    private Coroutine _StateCoroutine;

    private void Awake()
    {
        _Agent = GetComponent<NavMeshAgent>();

        // calls change of state
        _Player.OnStateChange += HandleStateChange;
    }

    private void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out hit))
        {
            transform.LookAt(hit.point, Vector3.up);
        }
        
    }

    private void HandleStateChange(PlayerState OldState, PlayerState NewState)
    {
        if(_StateCoroutine != null)
        {
            StopCoroutine(_StateCoroutine);
        }

        switch (NewState)
        {
            case PlayerState.Idle:
                _StateCoroutine = StartCoroutine(HandleIdlePlayer());
                break;

            case PlayerState.Moving:
                HandleMovingPlayer();
                break;  

        }

    }
    private IEnumerator HandleIdlePlayer()
    {
        switch (_Companion.State)
        {
            case CompanionState.Follow:
                yield return null; // 2 frames skipped in Follow Player, so need to be replicated here
                yield return null;
                yield return new WaitUntil(() => _Companion.State == CompanionState.Idle);
                goto case CompanionState.Idle;
            case CompanionState.Idle:
                if (_MoveCoroutine != null)
                {
                    StopCoroutine(_MoveCoroutine);
                }
                _Agent.enabled = false;
                RotateAroundPlayer();
                break;
        }
    }

     private void HandleMovingPlayer()
     {
        _Companion.ChangeState(CompanionState.Follow);
        if (_MoveCoroutine != null)
        {
            StopCoroutine(_MoveCoroutine);
        }

        if (!_Agent.enabled)
        {
            _Agent.enabled = true;
            _Agent.Warp(transform.position);
            }
        //_MoveCoroutine = StartCoroutine(FollowPlayer());
        FollowPlayer();
     }

     private void RotateAroundPlayer()
     {
        Debug.Log("Companion animation");

        _Agent.speed = 0F;
        /*
        WaitForFixedUpdate Wait = new WaitForFixedUpdate();
        while (true)
        {
            transform.RotateAround(_Player.transform.position, Vector3.up, _RotationSpeed);
            yield return Wait;
        }
        */
     }
    private void FollowPlayer()
    {

        _Agent.SetDestination(_PlayerFollow.position);
        _Agent.speed = 8f;
        /*
        
        
        //Vector3 playerDestination = playerAgent.destination;
          _Position = _PlayerFollow.position;
        Vector3 positionOffset = _FollowRadius * new Vector3(
            Mathf.Cos(2 * Mathf.PI * Random.value),
            0,
            Mathf.Sin(2 * Mathf.PI * Random.value)
        ).normalized;

        _Agent.SetDestination(_Position + positionOffset);
        */
        //yield return null; // Wait for player's destination to be updated!
        /*
        NavMeshAgent playerAgent = _Player.GetComponentInChildren<NavMeshAgent>();
        Vector3 playerDestination = playerAgent.destination;
        Vector3 positionOffset = _FollowRadius * new Vector3(
            Mathf.Cos(2 * Mathf.PI * Random.value),
            0,
            Mathf.Sin(2 * Mathf.PI * Random.value)
        ).normalized;

        _Agent.SetDestination(playerDestination + positionOffset);

        yield return null; // wait for agent's destination
        yield return new WaitUntil(() => _Agent.remainingDistance <= _Agent.stoppingDistance);
        */
        //_Companion.ChangeState(CompanionState.Idle);
    }


}
