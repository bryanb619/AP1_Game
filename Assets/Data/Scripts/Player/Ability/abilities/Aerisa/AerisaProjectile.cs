using UnityEngine;

public class AerisaProjectile : MonoBehaviour
{
    [SerializeField] private int bDamage = 40;
    public int Bdamage => bDamage;

    public void OnTriggerEnter(Collider hitInfo)
    {
        AIBehaviour enemy = hitInfo.GetComponent<AIBehaviour>();
        if (enemy != null)
        {
            //enemy.TakeDamage(Bdamage);
            Debug.Log("HIT");

            Destroy(gameObject);
        }

        //Instantiate(impactEffect, transform.position, transform.rotation);
    }
}