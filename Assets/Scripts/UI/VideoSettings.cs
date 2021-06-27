using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 *  Class: Video Settings
 *  
 *  Description:
 *  This script manages the Video Settings.
 *  
 *  Author: Andrea De Seta
*/
public class VideoSettings : MonoBehaviour
{
    Resolution[] resolutions;
    public Dropdown resolutionDropdown;
    void Start()
    {   
        //resolutions will be filled up with all the resolutions of our PC.
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        //I have to do a "conversion" because the Resolution type is strange.
        List<string> resolutionToString = new List<string>();
        int currentResolutionIndex = 0;
        
        for (int i = 0; i < resolutions.Length; i++){
            string res = resolutions[i].width + " x " + resolutions[i].height;
            resolutionToString.Add(res);

            //The current resolution of our PC will be setter for the game.
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                    currentResolutionIndex = i;
        }
        
        //The dropdown will be filled up with the resolutions strings
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
