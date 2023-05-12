using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimReceive : MonoBehaviour
{
    EnemyChaseBehaviour _chase; 

    private void Start()
    {
        _chase = GetComponentInParent<EnemyChaseBehaviour>();
        
    }

    internal void ReceiveAttackComand()
    {
        
    }
}
