using UnityEngine;
using UnityEngine.Serialization;

public class MiniMap : MonoBehaviour
{
    // Player camaera
    [FormerlySerializedAs("Target")] [SerializeField] private Transform target;

    private void LateUpdate()
    {
        Vector3 newPos = target.position;
        newPos.y = transform.position.y;

        transform.position = newPos;

        transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
    }
}
