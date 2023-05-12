using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.Serialization;

public class AiController : MonoBehaviour
{
    private int _maxAttackingAgents = 2;

    private EnemyChaseBehaviour _cEnemy; 

    private EnemyBehaviour _rEnemy;

    //[SerializeField] privat
    // CHANGE TO DIFERENT SCRIPT
    //private List<Agents> AttakingAI = new List<Agents>(); 

    [FormerlySerializedAs("_enemies")] [SerializeField] private List<GameObject> enemies = new List<GameObject>();


    private void Start()
    {
        //c_enemy = GetComponentInChildren<EnemyChaseBehaviour>();
    }

    public void AgentStartedAttacking(GameObject enemy)
    {
        //print(AttakingAI); 

        if (enemies.Count >= _maxAttackingAgents)
        {
            // Stop the agent from attacking

            AgentStoppedAttacking(enemy);
            
            return; 
            //agent.StopAttacking();
            
            
        }
        else if (!enemies.Contains(enemy))
        {
            // Add the agent to the list of attacking agents
            enemies.Add(enemy);
            return; 

        }
    }

    public void AgentStoppedAttacking(GameObject enemy)
    {
        //print(AttakingAI);
        //c_enemy.StopAttack();
        enemies.Remove(enemy);
        return; 
    }

   
}

