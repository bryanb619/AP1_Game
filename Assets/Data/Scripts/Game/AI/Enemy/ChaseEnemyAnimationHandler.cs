using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseEnemyAnimationHandler : MonoBehaviour
{
    private EnemyChaseBehaviour enemyChaseScript;
    private PlayerHealth player;
    
    private void Awake()
    {
        enemyChaseScript = GetComponentInParent<EnemyChaseBehaviour>();
        player = GetComponent<PlayerHealth>();
    }

    private void AttackPlayerAnimationEvent()
    {
        //enemyChaseScript.DealDamage(player);
    }
}
