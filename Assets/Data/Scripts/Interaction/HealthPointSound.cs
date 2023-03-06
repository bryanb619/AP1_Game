using UnityEngine;

public class HealthPointSound : MonoBehaviour
{
    private float elapsed;

    private void Start()
    {
        elapsed = 0f;
    }
    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        print(elapsed); 

        if (elapsed >= 3f)
        {
            Destroy(gameObject);    
        }
    }
}
