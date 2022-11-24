using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private Transform _Player;

    private void Start()
    {
        
    }

    private void LateUpdate()
    {
        Vector3 newPos = _Player.position;
        newPos.y = transform.position.y;

        transform.position = newPos;

        transform.rotation = Quaternion.Euler(90f, _Player.eulerAngles.y, 0f);
    }
}
