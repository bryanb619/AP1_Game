using System.Collections;
using UnityEngine;

public class FOV : MonoBehaviour
{
    private bool _canSeePlayer;
    public bool CanSee => _canSeePlayer;

    [Header("Target for debug only")]
    [SerializeField]private Transform target; 
    public Transform Target => target;

    [Range(10, 150)]
    public float radius;
    //public float Radius => radius;
    [Range(50, 360)]
    public float angle;
    //public float Angle => angle;


    public LayerMask targetMask;
    public LayerMask obstructionMask;
    [SerializeField] private Transform fov;
    public Transform pfovPoS => fov; // Enemy Editor FOV

    private bool canSeePlayer;
    public bool canSee => canSeePlayer;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    #region Field of view Routine
    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(fov.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - fov.position).normalized;

            if (Vector3.Angle(fov.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(fov.position, target.position);

                if (!Physics.Raycast(fov.position, directionToTarget, distanceToTarget, obstructionMask))
                    _canSeePlayer = true;
                else
                    _canSeePlayer = false;
            }
            else
                _canSeePlayer = false;
        }
        else if (_canSeePlayer)
            _canSeePlayer = false;
    }

    #endregion
}
