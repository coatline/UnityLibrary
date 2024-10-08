using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : Singleton<SceneFader>
{
    [SerializeField] Image fadePrefab;
    Image thisSceneImage;

    bool calledLoaded;

    protected override void Awake()
    {
        base.Awake();

        SceneManager.activeSceneChanged += OnSceneLoaded;

        OnSceneLoaded(SceneManager.GetActiveScene(), SceneManager.GetActiveScene());

        fadeTime = 0.25f;
    }

    void OnSceneLoaded(Scene scene1, Scene scene2)
    {
        if (calledLoaded == true || instance != this) return;
        calledLoaded = true;

        //if (thisSceneImage != null)
        //    DestroyImmediate(thisSceneImage);

        Image i = C.FindObjectOfNameFromArray("FadeOut(Clone)", FindObjectsOfType<Image>()) as Image;

        if (i != null)
        {
            thisSceneImage = i;
            return;
        }

        Canvas c = C.FindObjectOfNameFromArray("Canvas", FindObjectsOfType<Canvas>()) as Canvas;
        thisSceneImage = Instantiate(fadePrefab, c.transform);
        alpha = 1;

        sceneLoaded = true;
    }

    public void LoadNewScene(string sceneName, float fadeTime)
    {
        this.fadeTime = fadeTime;
        sceneToLoad = sceneName;
        loadingScene = true;
        Time.timeScale = 1;

        this.fadeTime = fadeTime;
    }

    public void LoadNewScene(string sceneName)
    {
        LoadNewScene(sceneName, 0.25f);
    }

    string sceneToLoad;
    bool loadingScene;
    bool sceneLoaded;
    float fadeTime;
    float alpha;

    private void Update()
    {
        thisSceneImage.color = new Color(0, 0, 0, alpha);

        if (loadingScene)
        {
            if (alpha < 1)
                alpha += Time.unscaledDeltaTime / fadeTime;
            else
            {
                sceneLoaded = false;
                calledLoaded = false;
                loadingScene = false;
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        else
        {
            if (sceneLoaded == true)
            {
                if (alpha > 0)
                    alpha -= Time.unscaledDeltaTime / fadeTime;
                else if (alpha < 0)
                {
                    sceneLoaded = false;
                    alpha = 0;
                }
            }
        }
    }
}
