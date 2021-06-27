using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 *  Class: BlackFadeScreen
 *  
 *  Description:
 *  When the player dies, a fading black screen will appear.
 *  
 *  Author: Andrea De Seta
*/

public class BlackFadeScreen : MonoBehaviour
{
    public Image blackScreen;
    void Start()
    {
        blackScreen = this.gameObject.GetComponent<Image>();
        //Set the alpha of the image to 0 in order to make it transparent
        blackScreen.canvasRenderer.SetAlpha(0.0f);
    }


    public void startFade(){
        StartCoroutine(WaitTimer());
    }

    private IEnumerator WaitTimer()
    {
        //There is the fade transition, 2 sec to black and 1 to return transparent
        yield return new WaitForSeconds(.7f);
        blackScreen.CrossFadeAlpha(1.0f, 2.0f, true);
        yield return new WaitForSeconds(3.0f);
        blackScreen.CrossFadeAlpha(0.0f, 1.0f, true);
    }
}
