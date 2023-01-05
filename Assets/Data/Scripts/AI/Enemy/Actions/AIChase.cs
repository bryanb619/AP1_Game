using UnityEngine;
using UnityEngine.AI;


public class AIChase : EnemyBrain
{
    [SerializeField] private NavMeshAgent agent;
    private EnemyBrain brain;
    [SerializeField]private Transform player;
    

    [Header("AI Attack Settings")]
    //[Range(0, 10)] 
    [SerializeField] private float minDist = 7f;

    [Range(0, 15)][SerializeField] private float minDistInCover = 12f;

    [SerializeField] private float minAttackDist = 3f;

    // Attack rate
    [Range(1, 5)] private float AttackRate = 2f;
    private float nextAttack = 0f;

    bool _inAttackRange;


    private void Start()
    {
        //agent= GetComponent<NavMeshAgent>();
        brain=GetComponent<EnemyBrain>();
        
       

    }

    private void Update()
    {
        MinimalCheck();
    }


    private void MinimalCheck()
    {
        if ((transform.position - transform.position).magnitude < minDist)
        {
            transform.LookAt(new Vector3(0, player.position.y, 0));
            _inAttackRange = true;
        }
        else
        {
            _inAttackRange = false;
        }
    }

    public void Action()
    {
        ChasePlayer();
    }

    private void ChasePlayer()
    {

        transform.LookAt(new Vector3(0, player.position.y, 0));

        agent.speed = 5f;
        agent.acceleration = 11f;

        agent.SetDestination(player.position);

        Attack();

        
        if (brain._canSeePlayer == false)
        {
            brain._inSearch = true;
        }
        

        //print("ATTACK");
    }
    private void Attack()
    {
        if (agent.remainingDistance <= 3)
        {
            agent.stoppingDistance = 2.7f;

            if (_inAttackRange == true)
            {

                if (Time.time > nextAttack)
                {
                    //transform.LookAt(playerTarget);

                    nextAttack = Time.time + AttackRate;
                    //_Player.TakeDamage(damage);
                }
                else
                {
                    //IsAttacking = false;
                }

            }
        }



    }


}
