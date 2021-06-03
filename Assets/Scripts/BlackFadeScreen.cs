using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackFadeScreen : MonoBehaviour
{
    public static BlackFadeScreen instance = null;

    void Awake() {
        if (instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
    }
    public Image blackScreen;
    // Start is called before the first frame update
    void Start()
    {
        blackScreen = this.gameObject.GetComponent<Image>();
        blackScreen.canvasRenderer.SetAlpha(0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startFade(){
        StartCoroutine(WaitTimer());
    }

    private IEnumerator WaitTimer(){
        blackScreen.CrossFadeAlpha(1.0f, 2.0f, true);
        yield return new WaitForSeconds(3.0f);
        blackScreen.CrossFadeAlpha(0.0f, 1.0f, true);
    }
}