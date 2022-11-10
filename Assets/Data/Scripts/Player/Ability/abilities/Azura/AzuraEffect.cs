using UnityEngine;

public class AzuraEffect : MonoBehaviour
{
    public Transform EffectPoint;
    public GameObject effect;

    [SerializeField] private AudioSource azuraSound;

    public void EffectSpawn()
    {
        Instantiate(effect, EffectPoint.position, EffectPoint.rotation);

        if(!azuraSound.isPlaying)
        {
            azuraSound.Play();
        }
        
    }
}
