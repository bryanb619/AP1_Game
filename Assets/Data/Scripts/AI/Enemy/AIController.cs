using System.Collections.Generic; 
using UnityEngine;

public class AIController : MonoBehaviour
{
    public int maxAttackingAgents = 3;

    // CHANGE TO DIFERENT SCRIPT
    public List<Agents> AttakingAI = new List<Agents>(); 


    public void AgentStartedAttacking(Agents agent)
    {
        print(AttakingAI); 

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
        print(AttakingAI);

        AttakingAI.Remove(agent);
    }
}

