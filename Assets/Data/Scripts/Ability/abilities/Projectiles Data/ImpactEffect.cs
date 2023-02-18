using UnityEngine;

public class ImpactEffect : MonoBehaviour
{
    [SerializeField] private ImpactData data;

    private float elapsed;
    private int maxTime;
    


    // Start is called before the first frame update
    private void Start()
    {
        maxTime = data.timeInScene;

    }

    // Update is called once per frame
    private void Update()
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
