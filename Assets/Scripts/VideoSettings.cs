using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettings : MonoBehaviour
{
    Resolution[] resolutions;
    public Dropdown resolutionDropdown;
    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> resolutionToString = new List<string>();
        int currentResolutionIndex = 0;
        
        for (int i = 0; i < resolutions.Length; i++){
            string res = resolutions[i].width + " x " + resolutions[i].height;
            resolutionToString.Add(res);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                    currentResolutionIndex = i;
        }
        
        resolutionDropdown.AddOptions(resolutionToString);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    void Update()
    {
        
    }

    public void SetResolution(int resIndex){
        Resolution resolutionToSet = resolutions[resIndex];
        Screen.SetResolution(resolutionToSet.width, resolutionToSet.height, Screen.fullScreen);
    }

    public void SetQuality(int qualityIndex){
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
