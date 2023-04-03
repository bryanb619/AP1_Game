using System.Collections.Generic; 
using UnityEngine;

public class AIController : MonoBehaviour
{
    private int maxAttackingAgents = 2;

    private EnemyChaseBehaviour c_enemy; 

    private EnemyBehaviour r_enemy;

    //[SerializeField] privat
    // CHANGE TO DIFERENT SCRIPT
    //private List<Agents> AttakingAI = new List<Agents>(); 

    [SerializeField] private List<GameObject> _enemies = new List<GameObject>();

    private void Start()
    {
        c_enemy = GetComponentInChildren<EnemyChaseBehaviour>();
    }

    public void AgentStartedAttacking(GameObject enemy)
    {
        //print(AttakingAI); 

        if (_enemies.Count >= maxAttackingAgents)
        {
            // Stop the agent from attacking

            AgentStoppedAttacking(enemy);
            
            return; 
            //agent.StopAttacking();
            
            
        }
        else if (!_enemies.Contains(enemy))
        {
            // Add the agent to the list of attacking agents
            _enemies.Add(enemy);
            return; 

        }
    }

    public void AgentStoppedAttacking(GameObject enemy)
    {
        //print(AttakingAI);
        //c_enemy.StopAttack();
        _enemies.Remove(enemy);
        return; 
    }

    /*
    public void AgentStartedAttacking(Agents agent)
    {
        //print(AttakingAI); 

        if (AttakingAI.Count >= maxAttackingAgents)
        {
            // Stop the agent from attacking
            agent.StopAttacking();
        }
        else if(!AttakingAI.Contains(agent))
        {
            // Add the agent to the list of attacking agents
            AttakingAI.Add(agent);
        }
    }

    public void AgentStoppedAttacking(Agents agent)
    {
        //print(AttakingAI);

        AttakingAI.Remove(agent);
    }
    */
}

