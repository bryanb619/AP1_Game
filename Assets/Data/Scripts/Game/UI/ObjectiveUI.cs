using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    private TextMeshProUGUI textReference;
    private DoorHandler doorHandler;
    [HideInInspector]public bool passedSecondObjective;
    [SerializeField] private string firstObjective;
    [SerializeField] private string secondObjective;
    [SerializeField] private string thirdObjective;
    [SerializeField] private string fourthObjective;
    [SerializeField] private float radius = 20f;
    [SerializeField] private LayerMask AILayer;

    [Header("Crystal")]
    [SerializeField] private GameObject[] Crystals;

    // Start is called before the first frame update
    void Start()
    {
        passedSecondObjective = false;
        textReference = GetComponent<TextMeshProUGUI>();
        textReference.text = firstObjective;

        doorHandler = FindObjectOfType<DoorHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (passedSecondObjective == false)
            KillEnemiesCheck();
        else if (passedSecondObjective == true)
            CleansedTheCrystals();
    }

    private void KillEnemiesCheck()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length <= 0)
        {
            textReference.text = secondObjective;
            foreach(GameObject crystal in Crystals)
            {
                crystal.GetComponent<Outline>().enabled = true;
            }
        }
    }

    private void CleansedTheCrystals()
    {
        //Reference the door opening script here
        textReference.text = thirdObjective;
        doorHandler.state = DoorHandler.DoorState.Opening;
    }
}
