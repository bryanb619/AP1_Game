using UnityEngine;

public class AzuraHandler : MonoBehaviour
{
    private float time;
    // Start is called before the first frame update
    private void Start()
    {
        time = 0;
    }

    private void Update()
    {
        CheckForDestroy(); 
    }

    private void CheckForDestroy()
    {
        time += Time.deltaTime;

        if (time >= 5)
        {
            Destroy(gameObject);
        }
    }


}
