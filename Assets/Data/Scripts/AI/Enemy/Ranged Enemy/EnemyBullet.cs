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

        if(startTime >= 10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider hitInfo)
    {
        PlayerMovement _player = hitInfo.GetComponent<PlayerMovement>();
        
        if(_player != null)
        {
            _player.TakeDamage(damage);
        }
        
        Destroy(gameObject);

    }


}
