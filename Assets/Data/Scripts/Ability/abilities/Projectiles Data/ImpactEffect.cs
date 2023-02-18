using UnityEngine;

public class ImpactEffect : MonoBehaviour
{
    private float elapsed;

    private int maxTime; 

    [SerializeField] private ImpactData data; 

    // Start is called before the first frame update
    void Start()
    {
        maxTime = data.timeInScene;
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;

        if(elapsed >= maxTime)
        {
            DestroyEffect();
        } 
    }

    private void DestroyEffect()
    {
        Destroy(gameObject);
    }
}
