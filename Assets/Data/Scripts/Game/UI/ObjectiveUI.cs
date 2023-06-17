using UnityEngine;
using TMPro;
using System.Collections;

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
                        [SerializeField]private bool            isAnimationPlaying;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _doorHandler = FindObjectOfType<DoorHandler>();
        isAnimationPlaying = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
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
            _totalEnemyCount = ai.Length;
            TextAnimation(1);
            enemiesSpawned = true;
        }

        if (ai.Length > 0 && enemiesSpawned)
        {
            //The commented part is not working as of right now due to not detecting the enemies in time, so it just puts it at 1 constantly
            mainTextReference.text = (objectives[1] /* + "(" + _currentEnemyDefeated + " / " + _totalEnemyCount + ")" */);
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
//        CrystalSelect[] ai = FindObjectsOfType<CrystalOutline>();
        
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
                if(isAnimationPlaying)
                    _animator.SetTrigger("Force Stop");
                
                newTextReference.text = (objectives[1]/* + "(" + _currentEnemyDefeated + "/" + _totalEnemyCount + ")"*/);
                isAnimationPlaying = true;
                _animator.SetTrigger("New Objective");
                break;
                
            default:
                if(isAnimationPlaying)
                    _animator.SetTrigger("Force Stop");
                
                newTextReference.text = objectives[i];
                isAnimationPlaying = true;
                _animator.SetTrigger("New Objective");
                break;
        }
        //lastAnimationIndex = i;
    }

    private void TextAnimationEnd()
    {
        mainTextReference.text = newTextReference.text;
        isAnimationPlaying = false;
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
