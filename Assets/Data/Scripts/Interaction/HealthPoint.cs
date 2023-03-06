using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class HealthPoint : MonoBehaviour
{
    [SerializeField] private HealthPointData data;
    private BoxCollider m_BoxCollider;

    private int m_Health;
    public int Health => m_Health;

    [SerializeField] private GameObject m_prefab; 

    private void Start()
    {
        m_BoxCollider = GetComponent<BoxCollider>();
       
        m_Health = data.Health;
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            if (player._playerHealthState == PlayerMovement._PlayerHealth.NotMax)
            {
                m_BoxCollider.enabled = false;
                
                player.Takehealth(Health);

                Instantiate(m_prefab, transform.position, Quaternion.identity);
                //Destroy(m_BoxCollider);
                Destroy(gameObject);
            }
            //player
        }
    }

}
