using UnityEngine;

public class MiniMap : MonoBehaviour
{
    // Player camaera
    [SerializeField] private Transform Target;

    private void LateUpdate()
    {
        Vector3 newPos = Target.position;
        newPos.y = transform.position.y;

        transform.position = newPos;

        transform.rotation = Quaternion.Euler(90f, Target.eulerAngles.y, 0f);
    }
}
