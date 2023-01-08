using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AI : MonoBehaviour
{
    [SerializeField] private AIBrain brain;

    [SerializeField] private ScriptableObject[] ScriptableObjectscriptableObject;

    private NavMeshAgent agent; 

    // public Transform Target;

    Dictionary<String, Transform> SceneValues = new Dictionary<String, Transform>()
    {
       // {"Target", Target},
    };
}
 