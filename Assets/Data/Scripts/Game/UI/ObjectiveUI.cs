using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class ObjectiveUi : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI mainTextReference, newTextReference;
    private Animator _animator;
    private DoorHandler _doorHandler;
    private bool _secondTextAnimationTriggered = false, _thirdTextAnimationTriggered = false, _passedThroughCollider = false;
    [HideInInspector]public bool passedSecondObjective;
    [SerializeField] private string firstObjective;
    [SerializeField] private string secondObjective;
    [SerializeField] private string thirdObjective;
    [SerializeField] private string fourthObjective;
    //[SerializeField] private float radius = 20f;
    [FormerlySerializedAs("AILayer")] [SerializeField] private LayerMask aiLayer;
    private int _totalEnemyCount = 0, _currentEnemyDefeated = 0;


    [FormerlySerializedAs("Crystals")]
    [Header("Crystal")]
    [SerializeField] private GameObject[] crystals;

    // Start is called before the first frame update
    void Start()
    {
        passedSecondObjective = false;
        mainTextReference.text = "No objective currently";
        _animator = GetComponent<Animator>();
        _doorHandler = FindObjectOfType<DoorHandler>();
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            _animator.SetTrigger("Show Objective");
        }
        


        if(_passedThroughCollider)
        {
            if (passedSecondObjective == false)
                KillEnemiesCheck();
            else if (passedSecondObjective == true)
                CleansedTheCrystals();
        }
    }

    public void PassedThroughCollider()
    {
        _passedThroughCollider = true;
        TextAnimation(1);
    }

    public void RecieveEnemyCountInfo(int chaseCount, int rangedCount)
    {
        _totalEnemyCount = chaseCount + rangedCount;
    }

    public void IncreaseEnemyDefeatedCount()
    {
        _currentEnemyDefeated += 1;
        mainTextReference.text = (firstObjective + "(" + _currentEnemyDefeated + "/" + _totalEnemyCount + ")");
    }

    private void KillEnemiesCheck()
    {
        //If needed for active and inactive checking through tags

        /*GameObject[] allEnemies = Resources.FindObjectsOfTypeAll<GameObject>()
            .Where(obj => obj.CompareTag("Enemy"))
            .ToArray();

        bool noEnemiesLeft = allEnemies.Length == 0;
        */
        //Debug.Log("Amount of enemies: " + currentEnemyDefeated);
        if (_totalEnemyCount == _currentEnemyDefeated)
        {
            TextAnimation(2);

            foreach (GameObject crystal in crystals)
            {
                crystal.GetComponent<Outline>().enabled = true;
            }
        }
    }

    public void Passed()
    {
        passedSecondObjective = true;
    }

    private void CleansedTheCrystals()
    {
        _doorHandler.state = DoorHandler.DoorState.Opening;
        //Reference the door opening script here
        TextAnimation(3);
    }

    private void TextAnimation(int i)
    {
        if (i == 1)
        {
            newTextReference.text = (firstObjective + "(" + _currentEnemyDefeated + "/" + _totalEnemyCount + ")");
            if(_passedThroughCollider == true)
            {
                _animator.SetTrigger("New Objective");
                _secondTextAnimationTriggered = true;
            }
        }
        else if (i == 2)
        {
            if (_secondTextAnimationTriggered == false)
            {
                newTextReference.text = secondObjective;
                _animator.SetTrigger("New Objective");
                _secondTextAnimationTriggered = true;
            }
        }
        else if(i == 3)
        {
            newTextReference.text = thirdObjective;
            if (_thirdTextAnimationTriggered == false)
            {
                _animator.SetTrigger("New Objective");
                _thirdTextAnimationTriggered = true;
            }
        }
    }

    private void TextAnimationEnd()
    {
        mainTextReference.text = newTextReference.text;
    }
}
