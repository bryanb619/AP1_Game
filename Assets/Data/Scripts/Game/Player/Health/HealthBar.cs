using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider, criticalSlider, shieldSlider;

    [SerializeField]
    private float criticalSliderVelocity;
    private float _time;
        
    //public Gradient _gradient;

    [FormerlySerializedAs("_fill")] public Image fill;

    private void Update()
    {
        SetCriticalHealth();
    }

    internal void SetMaxHealth(int empower, int shield)
    {

        healthSlider.maxValue += empower; 
        //criticalSlider.maxValue = health + shield;
        //shieldSlider.maxValue = health + shield;    
        //_fill.color = _gradient.Evaluate(1f);

    }
    internal void SetHealth(int health, int shield)
    {
        _time = 0f;
        healthSlider.value = health;
        //shieldSlider.value = health + shield;
        //_fill.color = _gradient.Evaluate(_slider.normalizedValue);
    }

    public void TakeDamageUi(int damage)
    {

    }

    public void TakeHealthUi(int heal)
    {

    }

    public void HealthEmpower(int powerUp)
    {
        healthSlider.maxValue += powerUp; 
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