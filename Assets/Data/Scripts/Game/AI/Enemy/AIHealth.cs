using UnityEngine;
using System.Collections;

public class AIHealth : MonoBehaviour
{
    [Header("Gem Spawn")]
    [SerializeField] private bool gemSpawnOnDeath = true;
    [SerializeField] private GameObject gemPrefab;

    [SerializeField] private float health;

    private Color originalColor;
    public int damage = 20;
    public int damageBoost = 0;

    private Transform player; 

    

    private void Start()
    {
        player = FindObjectOfType<Transform>();
    }

    #region AI Health 
    public void TakeDamage(int _damage)
    {
        print(health);


        if (health <= 0)
        {
            Die();
        }
        if (health > 0)
        {
            //_underAttack= true;
            transform.LookAt(new Vector3(0, player.position.y, 0));
            StartCoroutine(HitFlash());
        }
        health -= _damage + damageBoost;
        //Debug.Log("enemy shot" + _Health);
    }



    private void Die()
    {

        if (gemSpawnOnDeath)
            Instantiate(gemPrefab, transform.position, Quaternion.identity);


        //Instantiate(transform.position, Quaternion.identity);
        Destroy(gameObject);

        // call for AI event
        //DieEvent.Invoke();

        // Debug.Log("Enemy died");
    }
    #endregion

    IEnumerator HitFlash()
    {
        originalColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GetComponent<Renderer>().material.color = Color.magenta;
    }
}
