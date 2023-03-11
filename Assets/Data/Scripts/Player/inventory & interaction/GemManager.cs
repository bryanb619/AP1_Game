using System;
using System.Collections;
//using System.Collections.Generic;
using TMPro;
using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.UIElements;

public class GemManager : MonoBehaviour
{
    [Header ("Script References")]
    public PlayerMovement playerScript;

    private WaitForSeconds waitFor10Seconds = new(10f);

    private Gems gemType = new();

    private TextMeshProUGUI gemText;

    [SerializeField]
    private float maxDistance = 3;

    [SerializeField]
    private int gemSpeed = 2;

    [SerializeField]
    private Material mana, health;

    private GameObject player;
    private int gemNumber;

    [SerializeField, HideInInspector]
    private MeshRenderer CubeRenderer;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = FindObjectOfType<PlayerMovement>();
        player = GameObject.Find("Player");
    }

    private void Awake()
    {
        gemText = GetComponentInChildren<TextMeshProUGUI>();

        gemNumber = UnityEngine.Random.Range(1, 2);
        GemNumber(gemNumber);
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsPlayer();
        CodedAnimations();
        LookAtPlayer();
    }
   
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
        {
            GemCatch();
            Destroy(gameObject);
        }
            
    }

    #region Gem type and functions
    public void GemNumber(int i)
    {
        switch (i)
        {
            case 1:
                gemType = Gems.Mana;
                GetComponentInChildren<MeshRenderer>().material = mana;
                gemText.text = "Mana";
                break;

            case 2:
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
            case Gems.Mana:
                StopCoroutine(HealthGem());
                StartCoroutine(ManaGem());
                break;

            case Gems.Health:
                StopCoroutine(ManaGem());
                StartCoroutine(HealthGem());
                break;

            default:
                break;
        }
    }

    IEnumerator HealthGem()
    {
        playerScript.GiveHealth(10);

        yield return null;
    }

    IEnumerator ManaGem()
    {
        //Awaiting mana implementation
        Debug.Log("Mana gem caught");
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
    Health,
    Mana
}
