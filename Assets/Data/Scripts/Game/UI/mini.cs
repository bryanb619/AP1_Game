using UnityEngine;
using UnityEngine.UI;

public class Mini : MonoBehaviour
{
    private Image _borderImage;

    private void Start()
    {
        _borderImage = GetComponent<Image>();
        _borderImage.color = Color.yellow;
    }
    public void SetCollorRed()
    {    
        _borderImage.color = Color.red;  
    }

    public void SetCollorDefault()
    {
        _borderImage.color = Color.yellow;
    }
}
