using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [FormerlySerializedAs("_slider")] [SerializeField] private Slider slider;



    public void SetMana(float mana)
    {
        slider.value = mana;
  
    }

    internal void SetNewMax(int add)
    {
        slider.maxValue += add; 
    }
}