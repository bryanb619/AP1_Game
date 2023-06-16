using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AiHealth : MonoBehaviour
{
   
    
    [Header("Health Bar")]
    [SerializeField]    private Image           fill; 
                        private Slider          _healthBar;
                        
                        private float           _healthPercentage;
         
    [Header("Color Values")]
    [SerializeField] private float              yellowValue, redValue, greenValue; 
    
    private Animator _aiAnimator;

    [SerializeField] private EnemyChaseBehaviour chase; 
    [SerializeField] private EnemyRangedBehaviour ranged;
    
    [SerializeField] private RangedBossBehaviour rangedBoss;
    
    private enum AiType { Chase, Ranged, Boss}
    private AiType _aiType;
    

    private void Awake()
    {
        _healthBar = GetComponentInChildren<Slider>();
        _aiAnimator = GetComponentInParent<Animator>();


        if (chase != null)
        {
            _aiType = AiType.Chase;
        }

        if (ranged!= null)
        {
            _aiType = AiType.Ranged;
        }
        
        if (rangedBoss!= null)
        {
            _aiType = AiType.Boss;
        }
        
/*
        if (TryGetComponent(out EnemyChaseBehaviour chaseBehaviour))
        {
            
            _chase = GetComponentInParent<EnemyChaseBehaviour>();
            _aiType = AiType.Chase;
        }

        if (TryGetComponent(out EnemyRangedBehaviour rangedBehaviour))
        {
            _ranged = GetComponentInParent<EnemyRangedBehaviour>();
            _aiType = AiType.Ranged;
        }

        if (TryGetComponent(out RangedBossBehaviour rangedBossBehaviour))
        {
            _rangedBoss = GetComponentInParent<RangedBossBehaviour>(); 
            _aiType = AiType.Boss;
        }
        */
    }
    
    #region AI Health

    public void HealthValueSet(float maxHealth)
    {

        if (maxHealth >= 9999)
        {
            _healthBar.maxValue = 9999;
            _healthBar.value = _healthBar.maxValue;   
        }
        else
        {
            _healthBar.maxValue = maxHealth;
            _healthBar.value = _healthBar.maxValue;
        }
        
        
    }
    public void HandleBar(int damage)
    {
        _healthBar.value -= damage;
        //fill.color = _healthBar.value <= 25 ? Color.red : Color.green;

        _healthPercentage = _healthBar.value / _healthBar.maxValue * 100f;

#if UNITY_EDITOR
        //Debug.Log("Health Percentage: "+_healthPercentage);
#endif
        if ( _healthPercentage <= 0)
        {
           KillAi();
        }
        
        if(_healthPercentage <= redValue)
        {
            fill.color = Color.red;
        }
        else if(_healthPercentage <= yellowValue)
        {
            fill.color = Color.yellow;
        }
    }
    
    public void IncrementHealth(float value)
    {
        _healthBar.value = value;
        _healthPercentage = _healthBar.value / _healthBar.maxValue * 100f;
        
        if(_healthPercentage >= greenValue)
        {
            fill.color = Color.green;
        }
    }

    private void KillAi()
    {
        //_aiAnimator.enabled = false;

        switch (_aiType)
        {
            case AiType.Chase:
            {
                chase.Die(); 
                break;
            }

            case AiType.Ranged:
            {
               ranged.Die();
                break;
            }
                
            case AiType.Boss:
            {
                rangedBoss.Die();
                break;
            }
                
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion
}
