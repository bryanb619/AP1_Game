using UnityEngine;

public class AstraProjectile : MonoBehaviour
{
    private float Bspeed = 20f;
    //private int Bdamage = 20;
    [SerializeField] private Rigidbody rb;

    // Start is called before the first frame update
    private void Start()
    {
        rb.velocity = transform.forward * Bspeed;
    }

    public void OnTriggerEnter(Collider hitInfo)
    {
        EnemyBehaviour enemy = hitInfo.GetComponent<EnemyBehaviour>();
        if (enemy != null)
        {
            enemy.TakeDamage(20);
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