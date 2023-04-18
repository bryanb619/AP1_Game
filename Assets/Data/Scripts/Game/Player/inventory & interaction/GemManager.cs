using System;
using System.Collections;
//using System.Collections.Generic;
using TMPro;
using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.UIElements;

public class GemManager : MonoBehaviour
{
    [Header ("script references")]
    private PlayerHealth playerScript;
    private ManaManager manaManager;

    private Gems gemType = new();

    private TextMeshProUGUI gemText;
    
    [Header ("Variables")]
    [SerializeField] private float maxDistance = 3;
    [SerializeField] private int gemSpeed = 2;
    [SerializeField] private Material mana, health;
    [SerializeField] private int manaGemRecovery, healthGemRecovery;

    private GameObject player;
    private int gemNumber;

    [SerializeField, HideInInspector]
    private MeshRenderer CubeRenderer;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = FindObjectOfType<PlayerHealth>();
        manaManager = FindObjectOfType<ManaManager>();
        player = GameObject.Find("Player");
    }

    private void Awake()
    {
        gemText = GetComponentInChildren<TextMeshProUGUI>();

        gemNumber = UnityEngine.Random.Range(1, 3);
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
        if (i == 1)
        { 
            gemType = Gems.Mana;
            GetComponentInChildren<MeshRenderer>().material = mana;
            gemText.text = "Mana";
        }
        else if (i == 2)
        {
            gemType = Gems.Health;
            GetComponentInChildren<MeshRenderer>().material = health;
            gemText.text = "Health";
        }

        Debug.Log("Number: " + gemNumber + " Type: " + gemType);
    }
    
    public void GemCatch()
    {
        switch (gemType)
        {
            case Gems.Mana:
                manaManager.RecoverMana(manaGemRecovery);
                break;

            case Gems.Health:
                playerScript.RegenerateHealth(healthGemRecovery);
                break;

            default:
                break;
        }
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
