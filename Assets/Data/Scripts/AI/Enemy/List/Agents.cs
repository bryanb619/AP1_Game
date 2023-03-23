using UnityEngine;

public class Agents : MonoBehaviour
{
    private AIController _controller;

    private EnemyBehaviour _enemyBehaviour;
    
    private EnemyChaseBehaviour _enemyChaseBehaviour; 
    

    private void Start()
    {
        if(_enemyBehaviour != null)
        {
            _enemyBehaviour = GetComponentInParent<EnemyBehaviour>();   
        }
        if(_enemyChaseBehaviour != null) 
        {
            _enemyChaseBehaviour = GetComponentInParent<EnemyChaseBehaviour>();
        }

        _controller = FindObjectOfType<AIController>();
    }


    public void StartAttacking()
    {
        _controller.AgentStartedAttacking(this);

    }

    public void StopAttacking()
    {
        _controller.AgentStoppedAttacking(this);

        if(_enemyBehaviour != null)
        {
            //_enemyBehaviour.StopAttacking(); 
        }

        if( _enemyChaseBehaviour != null) 
        {

        }

    }
}
