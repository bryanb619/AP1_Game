using System;
using System.Collections;
//using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

//using UnityEngine.UI;
//using UnityEngine.UIElements;

public class GemManager : MonoBehaviour
{
    [Header ("script references")]
    private PlayerHealth _playerScript;
    private ManaManager _manaManager;

    private Gems _gemType = new();

    private TextMeshProUGUI _gemText;
    
    [Header ("Variables")]
    [SerializeField] private float maxDistance = 3;
    [SerializeField] private int gemSpeed = 2;
    [SerializeField] private Material mana, health;
    [SerializeField] private int manaGemRecovery, healthGemRecovery;

    private GameObject _player;
    private int _gemNumber;

    [FormerlySerializedAs("CubeRenderer")] [SerializeField, HideInInspector]
    private MeshRenderer cubeRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _playerScript = FindObjectOfType<PlayerHealth>();
        _manaManager = FindObjectOfType<ManaManager>();
        _player = GameObject.Find("Player");
    }

    private void Awake()
    {
        _gemText = GetComponentInChildren<TextMeshProUGUI>();

        _gemNumber = UnityEngine.Random.Range(1, 3);
        GemNumber(_gemNumber);
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

        canvas.transform.LookAt(_player.transform.position);
    }

    private void MoveTowardsPlayer()
    {
        var step = gemSpeed * Time.deltaTime;
        if (Vector3.Distance(transform.position, _player.transform.position) < maxDistance)
        {
            transform.position = Vector3.LerpUnclamped(transform.position, _player.transform.position, Time.deltaTime);
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
            _gemType = Gems.Mana;
            GetComponentInChildren<MeshRenderer>().material = mana;
            _gemText.text = "Mana";
        }
        else if (i == 2)
        {
            _gemType = Gems.Health;
            GetComponentInChildren<MeshRenderer>().material = health;
            _gemText.text = "Health";
        }

        Debug.Log("Number: " + _gemNumber + " Type: " + _gemType);
    }
    
    public void GemCatch()
    {
        switch (_gemType)
        {
            case Gems.Mana:
                _manaManager.RecoverMana(manaGemRecovery);
                break;

            case Gems.Health:
                _playerScript.RegenerateHealth(healthGemRecovery);
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
