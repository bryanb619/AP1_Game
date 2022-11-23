using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyBullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 10;
    public Rigidbody rb;


    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        startTime = 0f;
        rb.velocity = transform.forward * speed;


    }

    private void Update()
    {
        startTime += Time.deltaTime;

        if(startTime >= 15f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider hitInfo)
    {
        Player_test _player = hitInfo.GetComponent<Player_test>();
        
        if(_player != null)
        {
            _player.TakeDamage(damage);
        }
        
        Destroy(gameObject);

    }


}
