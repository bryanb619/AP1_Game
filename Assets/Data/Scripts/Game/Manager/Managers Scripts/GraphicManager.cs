using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphicManager : MonoBehaviour
{
    #region Variables
    
    [SerializeField]    private TMP_Dropdown        resDropdown;
    private Resolution[]        _resolutions;

    [SerializeField]    private GameObject          warningBox; 
                        
    [SerializeField]    private TMP_Dropdown        qualityDropdown;
    
    #region Resoltion
    
    public void ResolutionSetup()
    {
        // get resolution
        _resolutions = Screen.resolutions; 

        // clear
        resDropdown.ClearOptions(); 

        // create list of resolution
        List<string> resOptions = new List<string>();  

        int currentResolutionIndex = 0; 

        for (int x = 0; x < _resolutions.Length; x++)
        {
            string option = _resolutions[x].width + " x " + _resolutions[x].height;

            resOptions.Add(option);

            if (_resolutions[x].width == Screen.currentResolution.width && _resolutions[x].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = x;
            }
        }
        resDropdown.AddOptions(resOptions);
        resDropdown.value = currentResolutionIndex;
        resDropdown.RefreshShownValue();
    }
    
    
    public void ChangeResolution()
    {
        
        int resIndex = resDropdown.value;
        Resolution resolution = _resolutions[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        
        // save data
        PlayerPrefs.SetInt("Resolution", resIndex);

#if UNITY_EDITOR

        Debug.Log("Resolution: " + resolution.width + " x " + resolution.height);
        
#endif
    }

    #endregion

    #region Quality

    public void SetQualityLevel(int qualityLevel)
    {
        QualitySettings.SetQualityLevel(qualityLevel);
        qualityDropdown.RefreshShownValue();
        
#if UNITY_EDITOR
        
        Debug.Log("Current Quality: " + QualitySettings.GetQualityLevel());
#endif
    }
    public void Quality(int qualityIndex)
    {
        // Set Game graphical quality 
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("Quality", qualityIndex);
        
          
#if UNITY_EDITOR
        Debug.Log("Current Quality: " + QualitySettings.GetQualityLevel());
#endif
        
    }
    
    #endregion 
    
   public void OpenWarningRes()
    {
        warningBox.SetActive(true);
        
        
    #if UNITY_EDITOR
        print("run");
    #endif
    }


    public void SetNewRes(bool change)
    {
        if (change)
        {
            //warningBox.SetActive(true);
            
            int resIndex = resDropdown.value;
            Resolution resolution = _resolutions[resIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            warningBox.SetActive(false);
            
#if UNITY_EDITOR
            Debug.Log("Resolution: " + resolution.width + " x " + resolution.height);
#endif
        }
        
        else
        {
            warningBox.SetActive(false);
        }
        
    }
    
   

    #endregion
    
}
