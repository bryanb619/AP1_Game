using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public class ObjectiveUI : MonoBehaviour
{
    [SerializeField]    private TextMeshProUGUI mainTextReference, newTextReference;
                        private Animator        _animator;
                        private int             lastAnimationIndex;
                        private DoorHandler     _doorHandler;
                        private bool            _secondTextAnimationTriggered = false, 
                                                _thirdTextAnimationTriggered = false, 
                                                _passedThroughCollider = false;
    [SerializeField]    private string[]        objectives;
    [SerializeField]    private LayerMask       aiLayer;
                        private int             _totalEnemyCount = 0, 
                                                _currentEnemyDefeated = 0;

    [Header("Crystal")]
    [SerializeField]    private GameObject[]    crystals;
                        private int             cleansedCrystalAmount = 0;

                        private bool            enemiesSpawned = false;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _doorHandler = FindObjectOfType<DoorHandler>();
        TextAnimation(0);
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            _animator.SetTrigger("Show Objective");
        }
        
        EnemiesAliveCheck();

/*
        if(_passedThroughCollider)
        {
            if (passedSecondObjective == false)
                KillEnemiesCheck();
            else if (passedSecondObjective == true)
                CleansedTheCrystals();
        }
  */
    }

    public void EnemiesAliveCheck()
    {
        AIHandler[] ai = FindObjectsOfType<AIHandler>();


        if (ai.Length > 0 && !enemiesSpawned)
        {
            TextAnimation(1);
        }

        if (ai.Length > 0)
        {
            mainTextReference.text = (objectives[1] + "(" + _currentEnemyDefeated + " / " + _totalEnemyCount + ")");
            enemiesSpawned = true;
        }
        else if (ai.Length == 0 && enemiesSpawned)
        {
            TextAnimation(2);
            enemiesSpawned = false;
        }
    }
    
    
   
    public void IncreaseEnemyDefeatedCount()
    {
        _currentEnemyDefeated += 1;
        //mainTextReference.text = (firstObjective + "(" + _currentEnemyDefeated + "/" + _totalEnemyCount + ")");
    }


    public void CleansedTheCrystals()
    { 
//        CrystalOutline[] ai = FindObjectsOfType<CrystalOutline>();
        
        cleansedCrystalAmount++;
        
        switch (cleansedCrystalAmount)
        {
            case 1:
                _doorHandler.DisableBarrier(0);
                break;
            case 2:
                _doorHandler.DisableBarrier(1);
                break;
            case 3:
                _doorHandler.DisableBarrier(2);
                break;
            case 4:
                _doorHandler.DisableBarrier(3);
                break;
            case 6:
                _doorHandler.DisableBarrier(4);
                
                break;
            case 8:
                _doorHandler.DisableBarrier(5);
                _doorHandler.state = DoorHandler.DoorState.Opening;
                TextAnimation(4);
                break;
        }

    }

    private void TextAnimation(int i)
    {
        switch (i)
        {
            /*
            case 0:
                newTextReference.text = objectives[0];
                _animator.SetTrigger("New Objective");
                break;
            */
            case 1:
                newTextReference.text = (objectives[1] + "(" + _currentEnemyDefeated + "/" + _totalEnemyCount + ")");
                _animator.SetTrigger("New Objective");
                break;
                
            default:
                newTextReference.text = objectives[i];
                _animator.SetTrigger("New Objective");
                break;
        }
        lastAnimationIndex = i;
    }

    private void TextAnimationEnd()
    {
        mainTextReference.text = newTextReference.text;
    }
    
    
    //Old Code (here in case we need it)
    private void KillEnemiesCheck()
    {
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
       // passedSecondObjective = true;
    }
    
    public void PassedThroughCollider()
    {
        if(_passedThroughCollider == true)
            TextAnimation(1);
        
        _passedThroughCollider = true;
    }

    public void RecieveEnemyCountInfo(int chaseCount, int rangedCount)
    {
        _totalEnemyCount = chaseCount + rangedCount;
    }

}
