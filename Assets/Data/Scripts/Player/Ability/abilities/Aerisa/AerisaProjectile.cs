using UnityEngine;

public class AerisaProjectile : MonoBehaviour
{
    [SerializeField] private int bDamage = 40;
    public int Bdamage => bDamage;

    public void OnTriggerEnter(Collider hitInfo)
    {
        EnemyBehaviour enemy = hitInfo.GetComponent<EnemyBehaviour>();
        if (enemy != null)
        {
            //enemy.TakeDamage(Bdamage);
            Debug.Log("HIT");

            Destroy(gameObject);
        }

        //Instantiate(impactEffect, transform.position, transform.rotation);
    }
}