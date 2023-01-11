using UnityEngine;

public class NormalProjectile : MonoBehaviour
{
    private float _speed = 20f;

    private int EnemyDamage = 2;
    public int enemyDamage => EnemyDamage;

    private int EnemyChaseDamage = 2; // before 80
    public int enemyChaseDamage => EnemyChaseDamage;



    [SerializeField] private Rigidbody rb;

    // Start is called before the first frame update
    private void Start()
    {
        rb.velocity = transform.forward * _speed;
    }



    private void OnTriggerEnter(Collider hitInfo)
    {
        EnemyBehaviour enemy = hitInfo.GetComponent<EnemyBehaviour>();
        EnemyChaseBehaviour ChaseEnemy = hitInfo.GetComponent<EnemyChaseBehaviour>();
        PlayerMovement player = hitInfo.GetComponent<PlayerMovement>();

        if (enemy != null)
        {
            enemy.TakeDamage(enemyDamage, WeaponType.Normal);
            DestroyBullet();
            //Debug.Log("HIT");

        }
        else if (ChaseEnemy != null)
        {
            ChaseEnemy.TakeDamage(enemyChaseDamage, WeaponType.Normal);
            DestroyBullet();
            Debug.Log("HIT");

        }
        else if (player != null)
        {
            DestroyBullet();
        }

        //Instantiate(impactEffect, transform.position, transform.rotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }


    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
