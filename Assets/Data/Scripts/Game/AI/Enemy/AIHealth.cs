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

    private void Awake()
    {
        _healthBar = GetComponentInChildren<Slider>();  
    }
    
    #region AI Health

    public void HealthValueSet(float maxHealth)
    {
        _healthBar.maxValue = maxHealth;
        _healthBar.value = _healthBar.maxValue;
    }
    public void HandleBar(int damage)
    {
        _healthBar.value -= damage;
        //fill.color = _healthBar.value <= 25 ? Color.red : Color.green;

        _healthPercentage = _healthBar.value / _healthBar.maxValue * 100f;

#if UNITY_EDITOR
        //Debug.Log("Health Percentage: "+_healthPercentage);
#endif
        
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
    
    #endregion
}
