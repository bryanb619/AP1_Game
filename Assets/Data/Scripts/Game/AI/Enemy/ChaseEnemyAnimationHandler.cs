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
        player = FindObjectOfType<PlayerHealth>();
    }

    public void RecievePlayerCollision(PlayerHealth playerHealth)
    {
        player = playerHealth;
    }

    public void AttackPlayerAnimationEvent()
    {
        enemyChaseScript.ActualAttack();
    }
}
