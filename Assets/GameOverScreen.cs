using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    public static GameOverScreen instance = null;
    void Awake() {
        if (instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
    }

    [SerializeField] private AudioSource gameOverSfx;
    [SerializeField] private AudioSource clickSfx;
    [SerializeField] private GameObject gameOverPanel;
    private bool actived = false;

    void Start()
    {
        this.gameObject.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveGameOverScreen(){
        
        actived = true;
        this.gameObject.SetActive(true);
        gameOverSfx.Play();
        StartCoroutine(WaitForThePianoSound());
    }

    private IEnumerator WaitForThePianoSound(){
        yield return new WaitForSeconds(7.0f);
        gameOverPanel.SetActive(true);
    }

    public void PlayClickAudio(){
        clickSfx.Play();
    }

    public void BackToHub(){
        //Load HUB Scene
    }

    public void RestartLevel(){
        //Load this scene again
    }
    public bool getActived(){
        return actived;
    }
}
