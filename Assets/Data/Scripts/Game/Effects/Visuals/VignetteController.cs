using UnityEngine;
using UnityEngine.UI;

public class VignetteController : MonoBehaviour
{
    private Image _vignette;

    private float _rColor, _gColor, _bColor, _aColor; 

    private void Awake()
    {
        GetComponents();
    }
    
    private void GetComponents()
    {
        
        // get image & colors
        _vignette   = GetComponent<Image>();
        _rColor     = _vignette.color.r;
        _gColor     = _vignette.color.g;
        _bColor     = _vignette.color.b;
        _aColor     = _vignette.color.a;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SetIntensity(0.5f);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            ResetIntensity();
        }
        
    }

    public void SetIntensity(float intensity)
    {
        
        _vignette.color = new Color(_rColor, _gColor , _bColor , intensity);
    }
    
    private void ResetIntensity()
    {
        _vignette.color = new Color(_rColor, _gColor , _bColor , _aColor);
    }
    
}
