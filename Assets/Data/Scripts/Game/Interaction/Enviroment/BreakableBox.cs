using UnityEngine;

public class BreakableBox : MonoBehaviour
{
    private float _health = 100f;
    
    float tempDamage = 50f;
    
    // Start is called before the first frame update
    private void Start()
    {
        
    }
    
    
    private void TakeDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
