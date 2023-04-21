using System.Collections;
using UnityEngine;
using UnityEngine.AI; 

public class _PLAYERTESTMOVEMENT : MonoBehaviour
{
    private bool _isMoving = false;
    public bool IsMoving => _isMoving;

    private NavMeshAgent agent;

    [SerializeField] private float _playerSpeed;

    float speed;
    private Vector3 lastPosition; 

    [SerializeField] private float _turnSpeed;

    [SerializeField] private float _walkTimer;

    [SerializeField] private LayerMask seeThroughLayer;

    [SerializeField] private GameObject effect; 

    //private Vector3 targetEulerAngles;

    private Vector3 targetPosition;
    Vector3 direction; 


    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        agent = GetComponent<NavMeshAgent>();

        //targetEulerAngles = new Vector3(_turnSpeed, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

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
                    //direction = hit.point - transform.position;
                    //Instantiate(effect, hit.point, Quaternion.identity);

                    //Quaternion targetRotation = Quaternion.LookRotation(direction);
                    //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _turnSpeed * 2f);

                    //targetPosition = hit.point;
                    //isMoving = true;
                    float height = 0f;
                    height = hit.point.y + 0.3f;


                    //(effect, hit.point, Quaternion.identity);

                    GameObject spawnedObject = Instantiate(effect, hit.point, Quaternion.identity);

                    spawnedObject.transform.position =
                        new Vector3(spawnedObject.transform.position.x, height, spawnedObject.transform.position.z);


                    /*
                    if (_isMoving)
                    {
                        targetPosition = hit.point;

                    }
                    else
                    {
                        targetPosition = hit.point;
                        _isMoving = true;

                    }

                    */
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
}
