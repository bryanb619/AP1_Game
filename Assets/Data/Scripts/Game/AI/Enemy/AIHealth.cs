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

    private EnemyChaseBehaviour _chase; 
    private EnemyRangedBehaviour _ranged;
    
    private RangedBossBehaviour _rangedBoss;
    
    private enum AiType { Chase, Ranged, Boss}
    private AiType _aiType;
    

    private void Awake()
    {
        _healthBar = GetComponentInChildren<Slider>();
        _aiAnimator = GetComponentInParent<Animator>();
        

        if (TryGetComponent(out EnemyChaseBehaviour chaseBehaviour))
        {
            _chase = chaseBehaviour;
            _aiType = AiType.Chase;
        }

        if (TryGetComponent(out EnemyRangedBehaviour rangedBehaviour))
        {
            _ranged = rangedBehaviour;
            _aiType = AiType.Ranged;
        }

        if (TryGetComponent(out RangedBossBehaviour rangedBossBehaviour))
        {
            _rangedBoss = rangedBossBehaviour;
            _aiType = AiType.Boss;
        }
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
    
    public void IncrementHealth(float health)
    {
        _healthBar.value += health;
        
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
                _chase.Die(); 
                break;
            }

            case AiType.Ranged:
            {
               _ranged.Die();
                break;
            }
                
            case AiType.Boss:
            {
                _rangedBoss.Die();
                break;
            }
                
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion
}
