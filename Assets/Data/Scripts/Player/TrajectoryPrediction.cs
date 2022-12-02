using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryPrediction : MonoBehaviour
{
    [Header("Line renderer veriables")]
    public LineRenderer line;
    [Range(2, 100)]
    public int resolution;

    [Header("Formula variables")]
    public Vector3 velocity;
    public float yLimit;
    private float g;

    [Header("Linecast variables")]
    [Range(2, 30)]
    public int linecastResolution;
    public LayerMask canHit;

    public Transform shootPoint;

    private void Start()
    {
        g = Mathf.Abs(Physics.gravity.y);
    }

    private void Update()
    {
        RenderArc();
    }

    private void RenderArc()
    {
        var pt = CalculateLineArray();
        line.positionCount = pt.Length;
        line.SetPositions(pt);
    }

    private Vector3[] CalculateLineArray()
    {
        List<Vector3> lineArray = new List<Vector3>();

        float incT = 5.0f / (float)(resolution + 1);

        Vector3 pos = shootPoint.position;
        Vector3 velocity = 20 * shootPoint.forward;
        float t = 0;
        for (int i = 0; i < resolution + 1; i++)
        {
            lineArray.Add(pos);
            Vector3 oldPos = pos;
            pos = pos + velocity * incT;
            velocity = velocity + Physics.gravity * incT;

            RaycastHit hit;
            if (Physics.Linecast(oldPos, pos, out hit, canHit))
            {
                lineArray.Add(hit.point);
                break;
            }
        }
        return lineArray.ToArray();
    }

    private Vector3 HitPosition()
    {
        var lowestTimeValue = MaxTimeY() / linecastResolution;

        for (int i = 0; i < linecastResolution + 1; i++)
        {
            RaycastHit rayHit;

            var t = lowestTimeValue * i;
            var tt = lowestTimeValue * (i + 1);

            if (Physics.Linecast(CalculateLinePoint(t), CalculateLinePoint(tt), out rayHit, canHit))
                return rayHit.point;
        }

        return CalculateLinePoint(MaxTimeY());
    }

    private Vector3 CalculateLinePoint(float t)
    {
        float x = velocity.x * t;
        float z = velocity.z * t;
        float y = (velocity.y * t) - (g * Mathf.Pow(t, 2) / 2);
        return new Vector3(x + transform.position.x, y + transform.position.y, z + transform.position.z);
    }

    private float MaxTimeY()
    {
        var v = velocity.y;
        var vv = v * v;

        var t = (v + Mathf.Sqrt(vv + 2 * g * (transform.position.y - yLimit))) / g;
        return t;
    }

    private float MaxTimeX()
    {
        if (IsValueAlmostZero(velocity.x))
            SetValueToAlmostZero(ref velocity.x);

        var x = velocity.x;

        var t = (HitPosition().x - transform.position.x) / x;
        return t;
    }

    private float MaxTimeZ()
    {
        if (IsValueAlmostZero(velocity.z))
            SetValueToAlmostZero(ref velocity.z);

        var z = velocity.z;

        var t = (HitPosition().z - transform.position.z) / z;
        return t;
    }

    private bool IsValueAlmostZero(float value)
    {
        return value < 0.0001f && value > -0.0001f;
    }

    private void SetValueToAlmostZero(ref float value)
    {
        value = 0.0001f;
    }

    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }
}