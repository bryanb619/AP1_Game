using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GemManager : MonoBehaviour
{
    [Header ("Script References")]
    public PlayerMovement playerScript;
    public EnemyBehaviour enemy;
    public EnemyChaseBehaviour enemyChase;

    private WaitForSeconds waitFor10Seconds = new(10f);

    [SerializeField]
    private Gems gemType = new();

    private TextMeshProUGUI gemText;

    [SerializeField]
    private float maxDistance = 3;

    [SerializeField]
    private int gemSpeed = 2;

    [SerializeField]
    private Material damage, shield, speed, health;

    private GameObject player;
    private int gemNumber;

    [SerializeField, HideInInspector]
    private MeshRenderer CubeRenderer;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = FindObjectOfType<PlayerMovement>();
        enemy = FindObjectOfType<EnemyBehaviour>();
        enemyChase = FindObjectOfType<EnemyChaseBehaviour>();
        player = GameObject.Find("Player");
    }

    private void Awake()
    {
        gemText = GetComponentInChildren<TextMeshProUGUI>();

        if (gemType == Gems.Damage) 
            GemNumber(1);
        else if (gemType == Gems.Shield)
            GemNumber(2);
        else if (gemType == Gems.Speed)
            GemNumber(3);
        else if (gemType == Gems.Health)
            GemNumber(4);
        else
        {
            gemNumber = UnityEngine.Random.Range(1, 4);
            GemNumber(gemNumber);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsPlayer();
        CodedAnimations();
        LookAtPlayer();
    }
   
    /*     
    private void ChangeTextToGemType(string gemTypeText)
    {
        

        gemText = gemTypeText;

        Debug.Log("Success on changing text");
    }
   */
    private void LookAtPlayer()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();

        canvas.transform.LookAt(player.transform.position);
    }

    private void MoveTowardsPlayer()
    {
        var step = gemSpeed * Time.deltaTime;
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
                gemText.text = "Damage";
                break;

            case 2:
                gemType = Gems.Shield;
                GetComponentInChildren<MeshRenderer>().material = shield;
                gemText.text = "Shield";
                break;

            case 3:
                gemType = Gems.Speed;
                GetComponentInChildren<MeshRenderer>().material = speed;
                gemText.text = "Speed";
                break;

            case 4:
                gemType = Gems.Health;
                GetComponentInChildren<MeshRenderer>().material = health;
                gemText.text = "Health";
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
                StopCoroutine(HealthGem());
                StopCoroutine(ShieldGem());
                StopCoroutine(SpeedGem());
                StartCoroutine(DamageGem());
                break;

            case Gems.Shield:
                StopCoroutine(DamageGem());
                StopCoroutine(HealthGem());
                StopCoroutine(SpeedGem());
                StartCoroutine(ShieldGem());
                break;

            case Gems.Speed:
                StopCoroutine(DamageGem());
                StopCoroutine(ShieldGem());
                StopCoroutine(HealthGem());
                StartCoroutine(SpeedGem());
                break;

            case Gems.Health:
                StopCoroutine(DamageGem());
                StopCoroutine(ShieldGem());
                StopCoroutine(SpeedGem());
                StartCoroutine(HealthGem());
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
        playerScript.shield = 10;

        yield return waitFor10Seconds;

        playerScript.shield = 0;
    }

    IEnumerator SpeedGem()
    {
        playerScript.walkSpeed += 10;
        playerScript.dashSpeed += 10;
        playerScript.maxSpeed += 10;
        
        yield return waitFor10Seconds;

        playerScript.walkSpeed -= 10;
        playerScript.dashSpeed -= 10;
        playerScript.maxSpeed -= 10;
    }

    IEnumerator HealthGem()
    {
        playerScript.GiveHealth(10);

        yield return null;
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
    Health,
    Shield,
    Speed
}
