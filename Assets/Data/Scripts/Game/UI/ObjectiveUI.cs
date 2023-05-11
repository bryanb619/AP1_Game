using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI mainTextReference, newTextReference;
    private Animator animator;
    private DoorHandler doorHandler;
    private bool secondTextAnimationTriggered = false, thirdTextAnimationTriggered = false, passedThroughCollider = false;
    [HideInInspector]public bool passedSecondObjective;
    [SerializeField] private string firstObjective;
    [SerializeField] private string secondObjective;
    [SerializeField] private string thirdObjective;
    [SerializeField] private string fourthObjective;
    //[SerializeField] private float radius = 20f;
    [SerializeField] private LayerMask AILayer;
    private int totalEnemyCount = 0, currentEnemyDefeated = 0;


    [Header("Crystal")]
    [SerializeField] private GameObject[] Crystals;

    // Start is called before the first frame update
    void Start()
    {
        passedSecondObjective = false;
        mainTextReference.text = "No objective currently";
        animator = GetComponent<Animator>();
        doorHandler = FindObjectOfType<DoorHandler>();
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            animator.SetTrigger("Show Objective");
        }
        


        if(passedThroughCollider)
        {
            if (passedSecondObjective == false)
                KillEnemiesCheck();
            else if (passedSecondObjective == true)
                CleansedTheCrystals();
        }
    }

    public void PassedThroughCollider()
    {
        passedThroughCollider = true;
        newTextReference.text = firstObjective;
        animator.SetTrigger("New Objective");
    }

    public void RecieveEnemyCountInfo(int ChaseCount, int RangedCount)
    {
        totalEnemyCount = ChaseCount + RangedCount;
    }

    public void IncreaseEnemyDefeatedCount()
    {
        totalEnemyCount += 1;
        mainTextReference.text = (firstObjective + "(" + currentEnemyDefeated + "/" + totalEnemyCount + ")");
    }

    private void KillEnemiesCheck()
    {
        //If needed for active and inactive checking through tags

        /*GameObject[] allEnemies = Resources.FindObjectsOfTypeAll<GameObject>()
            .Where(obj => obj.CompareTag("Enemy"))
            .ToArray();

        bool noEnemiesLeft = allEnemies.Length == 0;
        */

        if (totalEnemyCount == currentEnemyDefeated)
        {
            TextAnimation(2);
            foreach (GameObject crystal in Crystals)
            {
                crystal.GetComponent<Outline>().enabled = true;
            }
        }
    }

    private void CleansedTheCrystals()
    {
        //Reference the door opening script here
        TextAnimation(2);
        doorHandler.state = DoorHandler.DoorState.Opening;
    }

    private void TextAnimation(int i)
    {
        if (i == 1)
        {
            newTextReference.text = (firstObjective + "(" + currentEnemyDefeated + "/" + totalEnemyCount + ")");
            if(secondTextAnimationTriggered == false)
                animator.SetTrigger("New Objective");
            secondTextAnimationTriggered = true;
        }
        else if (i == 2)
        {
            newTextReference.text = thirdObjective;
            animator.SetTrigger("New Objective");
            thirdTextAnimationTriggered = true;
        }
    }

    private void TextAnimationEnd()
    {
        mainTextReference.text = newTextReference.text;
    }
}
