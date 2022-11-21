using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CompanionMovement : MonoBehaviour
{

    // Player
    [SerializeField] private Player_ _Player;
    
    // agent
    [SerializeField] private NavMeshAgent _Agent;
    // script
    [SerializeField] private Companion _Companion;


    [SerializeField] private Transform _Target;

    private bool IsIdle; 


    private Vector3 _Position;


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
        // Idle call 
        if(IsIdle)
        {
            PosUpdate();
        }

        // move update


        // Fazer companion seguir no FU , 

        // ver distancia ate Follow Point

        if (_Agent.remainingDistance <= 0.5f)
        {
            //_Agent.SetDestination(_PlayerFollow.position);
        }


        // Se distancia for superior a Y  (porque torna movimento mais natural e dinamico)
        // usar navmesh (Set Destinatin)


        // no idle manter distancia 

        // No ataque tem que ir para Pos final 

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
                //_Agent.enabled = false;
                //
                Idle();
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

     private IEnumerator Idle()
     {
        IsIdle = true;

        Debug.Log("Companion animation");
        _Agent.SetDestination(transform.position);


        //_Agent.speed = 0F;
        /*
        if(_Agent.remainingDistance > 0.5f)
        {
            _Agent.speed = 3f;
            _Agent.SetDestination(_PlayerFollow.position);
        }
        /*
        WaitForFixedUpdate Wait = new WaitForFixedUpdate();
        while (true)
        {
            transform.RotateAround(_Player.transform.position, Vector3.up, _RotationSpeed);
            yield return Wait;
        }
        */
        yield return null;
    }


    private IEnumerator FollowPlayer()
    {

       

        _Agent.SetDestination(_Target.position);
        _Agent.speed = 8f;


        // Fazer companion seguir no FU , 

        // ver distancia ate Follow Point


        // Se distancia for superior a Y  (porque torna movimento mais natural e dinamico)
        // usar navmesh (Set Destinatin)


        // no idle manter distancia 

        // No ataque tem que ir para Pos final 


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
        yield return null; // wait for agent's destination

    }

    private void PosUpdate()
    {
        _Agent.speed = 1F;

        if (_Agent.remainingDistance <= 2f)
        {
            _Agent.SetDestination(_Target.position);
        }
            
    }



}
