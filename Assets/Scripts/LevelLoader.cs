using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;

    public Animator transition;
    public float transitionTime = 1;

    public Slider slider;

    void Awake()
    {
        instance = this;
        GlobalVariables.GameOver = GlobalVariables.Win = false;
    }

    public void ReloadLevel()
    {
        StartCoroutine(LoadLevelAsync(SceneManager.GetActiveScene().buildIndex));
    }

    public void LoadLevel(int x)
    {
        StartCoroutine(LoadLevelAsync(x));
    }

    IEnumerator LoadLevelAsync(int levelIndex)
    {
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
