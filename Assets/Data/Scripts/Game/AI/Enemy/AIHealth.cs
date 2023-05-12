using UnityEngine;
using System.Collections;

public class AiHealth : MonoBehaviour
{
    [Header("Gem Spawn")]
    [SerializeField] private bool gemSpawnOnDeath = true;
    [SerializeField] private GameObject gemPrefab;

    [SerializeField] private float health;

    private Color _originalColor;
    public int damage = 20;
    public int damageBoost = 0;

    private Transform _player; 

    

    private void Start()
    {
        _player = FindObjectOfType<Transform>();
    }

    #region AI Health 
    public void TakeDamage(int damage)
    {
        print(health);


        if (health <= 0)
        {
            Die();
        }
        if (health > 0)
        {
            //_underAttack= true;
            transform.LookAt(new Vector3(0, _player.position.y, 0));
            StartCoroutine(HitFlash());
        }
        health -= damage + damageBoost;
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
        _originalColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GetComponent<Renderer>().material.color = Color.magenta;
    }
}
