using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    [SerializeField]
    private Enum gemType = new Enum();

    private Enum typeOfGem;

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

    void GemType()
    {
        if (gemType == Enum.Damage)
        {
        }
        if (gemType == Enum.Shield)
        {
        }
        if (gemType == Enum.Mana)
        {
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}

enum Enum
{
    Damage,
    Shield,
    Mana
};
