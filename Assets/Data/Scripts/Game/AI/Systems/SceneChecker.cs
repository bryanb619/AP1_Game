using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(SphereCollider))]
public class SceneChecker : MonoBehaviour
{

    [FormerlySerializedAs("Collider")] public SphereCollider collider;


    [FormerlySerializedAs("FieldOfView")] public float fieldOfView = 360f;
    [FormerlySerializedAs("LineOfSightLayers")] public LayerMask lineOfSightLayers;

    public delegate void GainSightEvent(Transform target);
    public GainSightEvent OnGainSight;
    public delegate void LoseSightEvent(Transform target);
    public LoseSightEvent OnLoseSight;

    private Coroutine _checkForLineOfSightCoroutine;


    private void Awake()
    {
        collider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //enemyChase = FindObjectsOfType<EnemyChaseBehaviour>();

        if (!CheckLineOfSight(other.transform))
        {
            _checkForLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(other.transform));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        OnLoseSight?.Invoke(other.transform);
        if (_checkForLineOfSightCoroutine != null)
        {
            StopCoroutine(_checkForLineOfSightCoroutine);
        }
    }

    private bool CheckLineOfSight(Transform target)
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, direction);
        if (dotProduct >= Mathf.Cos(fieldOfView))
        {
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, collider.radius, lineOfSightLayers))
            {
                OnGainSight?.Invoke(target);
                return true;
            }
        }

        return false;
    }

    private IEnumerator CheckForLineOfSight(Transform target)
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);

        while (!CheckLineOfSight(target))
        {
            yield return wait;
        }
    }



}

