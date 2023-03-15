using System.Collections.Generic; 
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] private AIControllerData _data;

    //[HideInInspector]
    //public int currentAttackers; 

    [HideInInspector]
    public int maxAttackers = 3;
    
    public bool canAttack;

    private bool _underAttack;
    public bool UnderAttack => _underAttack;

    private List<GameObject> currentAttackers = new List<GameObject>(); 



    private bool _canAttack = true; 

    // Start is called before the first frame update
    void Start()
    {
        maxAttackers = _data.maxAttackers;
    }

    // Update is called once per frame
    void Update()
    {
        switch(IsPlayerUnderAttack())
        {
            case true:
                {
                    if(_canAttack)
                    {
                        if(!currentAttackers.Contains(gameObject))
                        {
                            if(currentAttackers.Count>= maxAttackers)
                            {
                                currentAttackers.Add(gameObject); 

                            }
                       

                        }
                       
                    }
                    else 
                    {
                        if(currentAttackers.Contains(gameObject))
                        {
                            currentAttackers.Remove(gameObject); 
                        }

                    }
                    break; 
                }
                        

            case false:
                {

                    //currentAttackers = 0; 
                    break; 

                }
        }
        

    }

/// <summary>
/// Public functions takes bool from enemey behaviours and will update 
/// </summary>
/// <param name="isAttacking"></param>
    public void PlayerAttackStatus(bool isAttacking)
    {
        switch (isAttacking)
        {
            case true:
                {
                   _underAttack = true;           
                    break; 
                }
            case false:
                {
                    _underAttack = false; 
                    break; 
                }
        }

    }

    private bool IsPlayerUnderAttack()
    {
        switch (_underAttack)
        {
        case true:
                {
                   return true; 
                }
            case false:
                {
                    return false;                     
                }
        }  
    }
}
