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
    
    public void StopAi()
    {
        //enemyChaseScript.StopAi();
        enemyChaseScript.SetAiAfterAttack(false);
    }
    public void ResumeAi()
    {
        //enemyChaseScript.StartAi();
        enemyChaseScript.SetAiAfterAttack(true);
    }

    public void AttackPlayerAnimationEvent()
    {
        enemyChaseScript.ActualAttack();
    }
}
