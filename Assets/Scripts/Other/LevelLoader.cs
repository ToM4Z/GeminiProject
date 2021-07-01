using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 *  Class: LevelLoader
 *  
 *  Description:
 *  This script handle change scene and show loading bad
 *  
 *  Author: Thomas Voce
*/

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;

    // this variable is used by player because when I'm changing scene
    // player can receive damage
    public bool isChangingScene = true;

    public Animator transition;
    public float transitionTime = 1;

    public Slider slider;

    void Awake()
    {
        instance = this;        
        Cursor.visible = false;
        GlobalVariables.isRespawning = GlobalVariables.GameOver = GlobalVariables.Win = false;
    }

    private void Start()
    {
        StartCoroutine(ChangedScene());
    }

    // with this method, the player should not open pause during the entry animation
    private IEnumerator ChangedScene()
    {
        yield return new WaitForSeconds(2f);
        isChangingScene = false;
    }

    // this method is called by 'Retry' button of gameover and victory screen
    public void ReloadLevel()
    {
        StartCoroutine(LoadLevelAsync(SceneManager.GetActiveScene().buildIndex));
    }

    public void LoadLevel(int x)
    {
        StartCoroutine(LoadLevelAsync(x));
    }

    // this method start to load the new scene and update loading bar
    IEnumerator LoadLevelAsync(int levelIndex)
    {
        isChangingScene = true;
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);

        slider.gameObject.SetActive(true);
        PlayerSaveSystem.Save();
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = progress;

            yield return null;
        }
    }
}
