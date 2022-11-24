using UnityEngine;

public class AstraProjectile : MonoBehaviour
{
    private float bSpeed = 20f;
    public int bDamage = 20;
    [SerializeField] private Rigidbody rb;

    // Start is called before the first frame update
    private void Start()
    {
        rb.velocity = transform.forward * bSpeed;
    }

    public void OnTriggerEnter(Collider hitInfo)
    {
        EnemyBehaviour enemy = hitInfo.GetComponent<EnemyBehaviour>();
        if (enemy != null)
        {
            enemy.TakeDamage(bDamage);
            DestroyBullet();
            Debug.Log("HIT");

        }
        EnemyChaseBehaviour Chaseenemy = hitInfo.GetComponent<EnemyChaseBehaviour>();
        if (enemy != null)
        {
            enemy.TakeDamage(bDamage);
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