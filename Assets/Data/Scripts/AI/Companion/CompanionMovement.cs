using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompanionMovement : MonoBehaviour
{

    // Player
    [SerializeField] private Player _Player;
    
    // agent
    [SerializeField] private NavMeshAgent _Agent;
    // script
    [SerializeField] private Companion _Companion;


    // editable variables
    [Header("configuration for iddle")]
    [Range(0f, 5)] private float _RotationSpeed = 1f;

    [Header("Configuration for Follow")]
    private float _FollowRadius = 2f;

    private Coroutine _MoveCoroutine;
    private Coroutine _StateCoroutine;

    private void Awake()
    {
       // Player.
    }






}
