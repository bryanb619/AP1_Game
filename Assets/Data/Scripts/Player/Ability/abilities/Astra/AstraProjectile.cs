using UnityEngine;

public class AstraProjectile : MonoBehaviour
{
    private float _speed = 20f;

    private int EnemyDamage = 20;
    public int enemyDamage => EnemyDamage;

    private int EnemyChaseDamage = 20; // before 80
    public int enemyChaseDamage => EnemyChaseDamage;

    [SerializeField] private Rigidbody rb;

    // Start is called before the first frame update
    private void Start()
    {
        rb.velocity = transform.forward * _speed;
    }

    public void OnTriggerEnter(Collider hitInfo)
    {
        EnemyBehaviour enemy = hitInfo.GetComponent<EnemyBehaviour>();
        EnemyChaseBehaviour ChaseEnemy = hitInfo.GetComponent<EnemyChaseBehaviour>();

        if (enemy != null)
        {
            enemy.TakeDamage(enemyDamage);
            DestroyBullet();
            //Debug.Log("HIT");

        }
        else if (ChaseEnemy != null)
        {
            ChaseEnemy.TakeDamage(enemyChaseDamage);
            DestroyBullet();
            Debug.Log("HIT");

        }

        //Instantiate(impactEffect, transform.position, transform.rotation);
    }


    private void DestroyBullet()
    {
        Destroy(gameObject);
    }

}