using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;
/*
[RequireComponent(typeof(SphereCollider))]
public class MeleeAttack : MonoBehaviour
{
    private PlayerMovement player;
    private EnemyChaseBehaviour AI_Head; 

    private SphereCollider collider;
    private float radius = 5f;

    public int damage = 10;

    [Range(1, 5)] private float AttackRate = 2f;
    private float nextAttack = 0f;

    private Collider[] hitColliders;


    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<SphereCollider>();

        AI_Head = GetComponentInParent<EnemyChaseBehaviour>();

    }
    public void Attack()
    {
        AttackPlayer();
    }

    private void AttackPlayer()
    {
        // Debug.Log 1st stage of attack 


        hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (var hitCollider in hitColliders)
        {

            Debug.Log("2nd Stage of attack");
            if (Time.time > nextAttack)
            {
                //transform.LookAt(playerTarget);

                nextAttack = Time.time + AttackRate;

                if (hitCollider.gameObject.CompareTag("Player"))
                {

                    Debug.Log("Attack SHOULD be succesfull ");
                    player.TakeDamage(damage);
                }
                else
                {
                    AI_Head.IsAttacking = false;
                }


            }
            
           
        }
    }

}
*/