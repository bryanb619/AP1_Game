using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GemManager : MonoBehaviour
{
    [Header ("Script References")]
    public Player_test playerScript;
    private EnemyBehaviour enemy;
    public EnemyChaseBehaviour enemyChase;

    private WaitForSeconds waitFor10Seconds = new(10f);

    [SerializeField]
    private Gems gemType = new();

    [SerializeField]
    private float maxDistance = 3;

    [SerializeField]
    private int speed = 2;

    [SerializeField]
    private Material damage, shield, mana;

    private GameObject player;
    private int gemNumber;

    [SerializeField, HideInInspector]
    private MeshRenderer CubeRenderer;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = FindObjectOfType<Player_test>();
        enemy = FindObjectOfType<EnemyBehaviour>();
        enemyChase = FindObjectOfType<EnemyChaseBehaviour>();
        player = GameObject.Find("Player");
    }

    private void Awake()
    {
        gemNumber = UnityEngine.Random.Range(1, 4);
        GemNumber(gemNumber);
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
            transform.position = Vector3.LerpUnclamped(transform.position, player.transform.position, Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            GemCatch();
            Destroy(gameObject);
    }

    #region Gem type and functions
    public void GemNumber(int i)
    {
        switch (i)
        {
            case 1:
                gemType = Gems.Damage;
                GetComponentInChildren<MeshRenderer>().material = damage;
                break;

            case 2:
                gemType = Gems.Shield;
                GetComponentInChildren<MeshRenderer>().material = shield;
                break;

            case 3:
                gemType = Gems.Mana;
                GetComponentInChildren<MeshRenderer>().material = mana;
                break;

            default:
                print("Error Selecting the Gem Type");
                break;
        }

        Debug.Log("Number: " + gemNumber + " Type: " + gemType);
    }
    
    public void GemCatch()
    {
        switch (gemType)
        {
            case Gems.Damage:
                StopCoroutine(ShieldGem());
                StopCoroutine(ManaGem());
                StartCoroutine(DamageGem());
                break;

            case Gems.Shield:
                StopCoroutine(ManaGem());
                StopCoroutine(DamageGem());
                StartCoroutine(ShieldGem());
                break;

            case Gems.Mana:
                StopCoroutine(DamageGem());
                StopCoroutine(ShieldGem());
                StartCoroutine(ManaGem());
                break;

            default:
                break;
        }
    }


    IEnumerator DamageGem()
    {
        enemy.damageBoost = 30;
        enemyChase.damageBoost = 30;

        yield return waitFor10Seconds;

        enemy.damageBoost = 0;
        enemyChase.damageBoost = 0;
    }

    IEnumerator ShieldGem()
    {
        playerScript.shield = 19;

        yield return waitFor10Seconds;

        playerScript.shield = 0;
    }

    IEnumerator ManaGem()
    {
        Debug.Log("You do nothing for now");
        yield return waitFor10Seconds;
        Debug.Log("You still do nothing");
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

    void CodedAnimations()
    {
        gameObject.transform.Rotate(0, 1, 0);

    }
}

public enum Gems
{
    Damage,
    Shield,
    Mana
}
