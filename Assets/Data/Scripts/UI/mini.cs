using UnityEngine;
using UnityEngine.UI;

public class mini : MonoBehaviour
{
    private Image borderImage;

    private void Start()
    {
        borderImage = GetComponent<Image>();
        borderImage.color = Color.yellow;
    }
    public void SetCollorRed()
    {    
        borderImage.color = Color.red;  
    }

    public void SetCollorDefault()
    {
        borderImage.color = Color.yellow;
    }
}
