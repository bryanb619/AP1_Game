using UnityEngine;

public class ImpactEffect : MonoBehaviour
{
    private float           elapsed; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;

        if(elapsed >=3f)
        {
            DestroyEffect();
        }
        
    }

    private void DestroyEffect()
    {
        
    }
}
