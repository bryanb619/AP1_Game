using UnityEngine;
using UnityEngine.UI;

public class VignetteController : MonoBehaviour
{
   //[SerializeField] 

   private Image _vignette;
   private Color _color; 
   
    
    private void Awake()
    {
        //_vignette = GetComponent<Image>(); 
        
        //
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SetIntensity(0.6f);
        }
        
    }

    public void SetIntensity(float intensity)
    {
        /*
        _color = _vignette.color; 
        
        _color.a = intensity;
        _vignette.color = _color;
    */
        
        var tempColor = _vignette.color;
        tempColor.a = intensity;
        _vignette.color = tempColor;

    }
    
}
