using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public static GameOverScreen instance = null;
    void Awake() {
        if (instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
    }

    [SerializeField] private AudioSource clickSfx;
    [SerializeField] private GameObject gameOverPanel;
    private bool actived = false;

    void Start()
    {
        this.gameObject.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Return))
        {
            Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

            if (button != null)
                button.onClick.Invoke();
        }
    }

    public void ActiveGameOverScreen(){        
        actived = true;
        this.gameObject.SetActive(true);
        Managers.Audio.PlayGameOver();
        StartCoroutine(WaitForThePianoSound());
    }

    private IEnumerator WaitForThePianoSound(){
        yield return new WaitForSeconds(7.0f);
        Time.timeScale = 0;
        gameOverPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("HubButton"));
    }

    public void PlayClickAudio(){
        clickSfx.Play();
    }

    public void BackToHub()
    {
        Time.timeScale = 1f;
        GlobalVariables.PlayerLives = GlobalVariables.PlayerLivesToReset;
        LevelLoader.instance.LoadLevel(GlobalVariables.HUB_SCENE);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        GlobalVariables.PlayerLives = GlobalVariables.PlayerLivesToReset;
        LevelLoader.instance.ReloadLevel();
    }
    public bool getActived(){
        return actived;
    }
}
