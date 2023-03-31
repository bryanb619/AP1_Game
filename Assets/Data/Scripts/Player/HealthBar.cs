//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider _slider, criticalSlider;

    [SerializeField]
    private float criticalSliderVelocity;
    private float time;
        
    //public Gradient _gradient;

    public Image _fill;

    private void Update()
    {
        SetCriticalHealth();
    }

    public void SetMaxHealth(int health)
    {
        _slider.maxValue = health;
        _slider.value = health;

        //_fill.color = _gradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        time = 0f;
        _slider.value = health;
        //_fill.color = _gradient.Evaluate(_slider.normalizedValue);
    }

    private void  SetCriticalHealth()
    {
        time += Time.deltaTime * criticalSliderVelocity;

        criticalSlider.value = Mathf.Lerp(criticalSlider.value, _slider.value, time);
    }
}