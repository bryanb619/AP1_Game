using System;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    [SerializeField][Range(1, 10)] float radius;

    public bool InArea(Vector3 pos)
    {
        return Mathf.Pow(pos.x - transform.position.x, 2) +
                Mathf.Pow(pos.y - transform.position.y, 2) <=
                Mathf.Pow(radius, 2);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,
        radius);
    }

    public Vector3 GetRndInside()
    {
        Vector3 rndPoint = UnityEngine.Random.insideUnitCircle * radius;

        //return rndPoint.z(transform.position.z) + transform.position.z(0);
        return transform.position; 
    }
}
