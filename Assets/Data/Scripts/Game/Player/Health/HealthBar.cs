using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider, criticalSlider, shieldSlider;

    [SerializeField]
    private float criticalSliderVelocity;
    private float time;
        
    //public Gradient _gradient;

    public Image _fill;

    private void Update()
    {
        SetCriticalHealth();
    }

    internal void SetMaxHealth(int health, int shield)
    {
        healthSlider.maxValue = health + shield;
        criticalSlider.maxValue = health + shield;
        shieldSlider.value = shield;
        //_fill.color = _gradient.Evaluate(1f);
    }

    internal void SetHealth(int health)
    {
        time = 0f;
        healthSlider.value = health;
        //_fill.color = _gradient.Evaluate(_slider.normalizedValue);
    }

    public void TakeDamageUI(int damage)
    {

    }

    public void TakeHealthUI(int heal)
    {

    }

    public void HealthEmpower(int powerUp)
    {
        //_slider.maxValue = 
    }



    private void  SetCriticalHealth()
    {
        time += Time.deltaTime * criticalSliderVelocity;

        if (criticalSlider.value < healthSlider.value)
            criticalSlider.value = healthSlider.value;
        else
            criticalSlider.value = Mathf.Lerp(criticalSlider.value, healthSlider.value, time);
    }
}