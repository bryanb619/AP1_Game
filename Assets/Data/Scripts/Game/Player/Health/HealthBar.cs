using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider, criticalSlider, shieldSlider;

    [SerializeField]
    private float criticalSliderVelocity;
    private float _time;
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI healthText;
        
    //public Gradient _gradient;

    [FormerlySerializedAs("_fill")] public Image fill;

    private void Start()
    {
        healthText.text = healthSlider.value + "/" + healthSlider.maxValue;
    }

    private void Update()
    {
        SetCriticalHealth();
    }

    internal void SetMaxHealth( int max, int shield)
    {
      
        healthSlider.maxValue +=  max; 
        
        
        //criticalSlider.maxValue = health + shield;
        //shieldSlider.maxValue = health + shield;    
        //_fill.color = _gradient.Evaluate(1f);

    }
    internal void SetHealth(int health, int shield)
    {
        _time = 0f;
        healthSlider.value = health;
        healthText.text = healthSlider.value + "/" + healthSlider.maxValue;
        //shieldSlider.value = health + shield;
        //_fill.color = _gradient.Evaluate(_slider.normalizedValue);
    }

    public void TakeDamageUi(int damage)
    {

    }

    public void TakeHealthUi(int heal)
    {

    }

    public void HealthEmpower(int currentHealth,int powerUp)
    {
        healthSlider.maxValue += powerUp; 
        healthSlider.value = healthSlider.maxValue;    
        
        print(currentHealth +"/"+ healthSlider.maxValue);
        
        //healthText.text = healthSlider.maxValue.ToString(currentHealth +"/" + healthSlider.maxValue);
        healthText.text = currentHealth+ "/" + healthSlider.maxValue;
    }



    private void  SetCriticalHealth()
    {
        _time += Time.deltaTime * criticalSliderVelocity;

        if (criticalSlider.value < healthSlider.value)
            criticalSlider.value = healthSlider.value;
        else
            criticalSlider.value = Mathf.Lerp(criticalSlider.value, healthSlider.value, _time);
    }
}