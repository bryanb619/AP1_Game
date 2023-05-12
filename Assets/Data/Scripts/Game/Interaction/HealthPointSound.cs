using UnityEngine;

public class HealthPointSound : MonoBehaviour
{
    private float _elapsed;

    private void Start()
    {
        _elapsed = 0f;
    }
    // Update is called once per frame
    void Update()
    {
        _elapsed += Time.deltaTime;
        //print(elapsed); 

        if (_elapsed >= 3f)
        {
            Destroy(gameObject);    
        }
    }
}
