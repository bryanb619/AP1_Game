using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Playertestmovement : MonoBehaviour
{
    private bool _isMoving = false;
    public bool IsMoving => _isMoving;

    private NavMeshAgent _agent;

    [FormerlySerializedAs("_playerSpeed")] [SerializeField] private float playerSpeed;

    float _speed;
    private Vector3 _lastPosition; 

    [FormerlySerializedAs("_turnSpeed")] [SerializeField] private float turnSpeed;

    [FormerlySerializedAs("_walkTimer")] [SerializeField] private float walkTimer;

    [SerializeField] private LayerMask seeThroughLayer;

    [SerializeField] private GameObject effect; 

    //private Vector3 targetEulerAngles;

    private Vector3 _targetPosition;
    Vector3 _direction; 


    private Camera _mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = FindObjectOfType<Camera>();
        _agent = GetComponent<NavMeshAgent>();

        //targetEulerAngles = new Vector3(_turnSpeed, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
       // Movement();


    }
    /*
    private void Movement()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // agent.isStopped = false;
            RaycastHit hit;

            // Something other than the world was hit!
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100, ~seeThroughLayer))
            //if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100, NavMesh.GetAreaFromName("Walkable")))
            {

                if (hit.transform.CompareTag("Walk"))
                {
                    agent.enabled = false;

                    float height = 0f;
                    height = hit.point.y + 0.3f;


                    GameObject spawnedObject = Instantiate(effect, hit.point, Quaternion.identity);

                    spawnedObject.transform.position =
                        new Vector3(spawnedObject.transform.position.x, height, spawnedObject.transform.position.z);

                }
            }
        }
        if (_isMoving)
        {

            direction = targetPosition - transform.position;

            direction.y = 0;

            // Rotate player towards the target position
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _turnSpeed);

            StartCoroutine(StartToWalkTimer());

            // Move player towards the target position
            Vector3 movement = direction.normalized * _playerSpeed * Time.deltaTime;
            transform.Translate(movement, Space.World);



            // Stop moving if the player is close enough to the target position
            if (Vector3.Distance(transform.position, targetPosition) < 0.3f) //&& speed <= 0.3f)
            {
                _isMoving = false;
                return;
            }
        }

    }
    

    private IEnumerator StartToWalkTimer()
    {
        yield return new WaitForSeconds(_walkTimer * 2f);
        
    }

    */
}
