using System.Collections.Generic; 
using UnityEngine;

public class AIController : MonoBehaviour
{
    private int maxAttackingAgents = 3;


    //[SerializeField] privat
    // CHANGE TO DIFERENT SCRIPT
    private List<Agents> AttakingAI = new List<Agents>(); 


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
}

