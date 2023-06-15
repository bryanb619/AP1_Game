using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaBar : MonoBehaviour
{
     [SerializeField] private Slider slider;

    [SerializeField] private TextMeshProUGUI manaText;

    public void SetMana(float mana)
    {
        slider.value = mana;
        manaText.text = slider.value + "/" + slider.maxValue;
    }

    internal void SetNewMax(int add)
    {
        slider.maxValue += add; 
        manaText.text = slider.value + "/" + slider.maxValue;
    }
}