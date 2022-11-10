using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class GemManager : MonoBehaviour
{
    [SerializeField]
    private Gems gemType = new Gems();

    private GameObject player;

    [SerializeField]
    private float maxDistance = 3;

    [SerializeField]
    private int speed = 2;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsPlayer();
        CodedAnimations();
    }
        
    private void MoveTowardsPlayer()
    {
        var step = speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, player.transform.position) < maxDistance)
        {
            transform.position = Vector3.Lerp(transform.position, player.transform.position, Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            Destroy(gameObject);
    }

    #region Gem type and functions
    void GemType()
    {
        switch(gemType)
        { 
            case Gems.Damage:
                break;  

            case Gems.Shield:
                break;

            case Gems.Mana:
                break;
            
            default:
                break;
        }
    }
    
    void DamageGem()
    {

    }

    void ShieldGem()
    {

    }

    void ManaGem()
    {

    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

    void CodedAnimations()
    {

    }
}

public enum Gems
{
    Damage,
    Shield,
    Mana
}
