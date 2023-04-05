using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; 
using LibGameAI.FSMs;

[RequireComponent(typeof(NavMeshAgent))]
public class BossBehaviour : MonoBehaviour
{


    // References //
    [SerializeField] private AIBossData _data; 

    private NavMeshAgent _agent;

    // Reference to the state machine
    private StateMachine stateMachine;

    // Reference to the Outline component
    [SerializeField] private Outline _outlineDeactivation;

    private WarningSystemAI _warn;

    private GemManager _gemManager;




    // FOV //

    [Header("FOV")]
    [Range(10, 150)]
    [SerializeField]
    private float radius = 10F;
    public float Radius => radius;

    [Range(50, 360)]
    [SerializeField]
    private float angle = 20F;
    public float Angle => angle;

    private LayerMask targetMask;
    private LayerMask obstructionMask;
    [SerializeField] private Transform FOV;
    public Transform EEFOV => FOV;

    private bool canSeePlayer;
    public bool canSee => canSeePlayer;



    // hide code //
    [Header("Hide config")]
    private Collider[] Colliders = new Collider[10];

    //[Range(-1, 1)]
    private float HideSensitivity = 0;

    //[Range(0.01f, 1f)]
    //[SerializeField] 
    private float UpdateFrequency = 0.65f;

    [SerializeField] private LayerMask HidableLayers;

  
    private float MinObstacleHeight = 0.3f;

    public SceneChecker LineOfSightChecker;

    private Coroutine MovementCoroutine;

    private float fleeDistance;

    // UI //
    [SerializeField] private Slider _healthSlider;

    [SerializeField] private Slider _abilitySlider;


    // Start is called before the first frame update
    void Start()
    {
        GetProfile();
        GetComponents();
    }


    private void GetProfile()
    {

    }

    private void GetComponents()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
