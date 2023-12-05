using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class SceneLoadManager : Singleton<SceneLoadManager>
{
    enum eLoadingState
    {
        NONE = 0,
        ING,
    }

    [SerializeField]
    private Canvas ManagerCanvas;
    [SerializeField]
    private Image loadingBar;
    [SerializeField]
    private TextMeshProUGUI loadingText;

    string nextScene;
    private eLoadingState eCurrnetState;


    private void Awake()
    {
        eCurrnetState = eLoadingState.NONE;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        SceneLoadder("GameField");
    }

    private void Update()
    {
        switch (eCurrnetState)
        {
            case eLoadingState.NONE:
                ManagerCanvas.gameObject.SetActive(false);
                loadingBar.gameObject.SetActive(false);
                loadingText.gameObject.SetActive(false);
                ManagerCanvas.sortingLayerName = "";
                break;
            case eLoadingState.ING:
                ManagerCanvas.gameObject.SetActive(true);
                loadingBar.gameObject.SetActive(true);
                loadingText.gameObject.SetActive(true);
                ManagerCanvas.sortingLayerName = "UI";
                break;
        }
    }

    #region 싱글로딩
    //싱글 로딩, 로딩바 표시
    public void SceneLoadder(string sceneName)
    {
        nextScene = sceneName;
        eCurrnetState = eLoadingState.ING;
        StartCoroutine(LoadSceneManager());
    }
    //싱글 로딩, 로딩바 표시X
    public void SceneLoadderForNotLoadProcessShow(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        ChangeSceneManagerCall();
    }

    IEnumerator LoadSceneManager()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(nextScene);
        async.allowSceneActivation = false;
        float timer = 0f;
        while (!async.isDone)
        {
            yield return null;

            if (async.progress < 0.9f)
            {

                loadingBar.fillAmount = async.progress;

                int percent = Mathf.RoundToInt(async.progress * 100);
                loadingText.text = $"{percent}%";
            }
            else
            {
                
                timer += Time.unscaledDeltaTime;

                float percent = Mathf.RoundToInt(Mathf.Lerp(0.9f, 1.0f, timer) * 100);
                loadingText.text = $"{percent}%";

                loadingBar.fillAmount = Mathf.Lerp(0.9f, 1.0f, timer);
                if (loadingBar.fillAmount >= 1f)
                {
                    async.allowSceneActivation = true;
                }
            }
        }
        ChangeSceneManagerCall();
        eCurrnetState = eLoadingState.NONE;
    }

    #endregion

    #region 멀티로딩
    //멀티 로딩, 로딩바 표시O
    public void SceneLoadder_Additive(string sceneName)
    {
        nextScene = sceneName;
        eCurrnetState = eLoadingState.ING;
        StartCoroutine(LoadSceneManager_Additive());
    }

    //멀티 로딩, 로딩바 표시X
    public void SceneLoadder_AdditiveForNotLoadProcessShow(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        ChangeSceneManagerCall();
    }

    IEnumerator LoadSceneManager_Additive()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
        async.allowSceneActivation = false;
        float timer = 0f;
        while (!async.isDone)
        {
            yield return null;

            if (async.progress < 0.9f)
            {
                loadingBar.fillAmount = async.progress;

                int percent = Mathf.RoundToInt(async.progress * 100);
                loadingText.text = $"{percent}%";
            }
            else
            {

                timer += Time.unscaledDeltaTime;

                float percent = Mathf.RoundToInt(Mathf.Lerp(0.9f, 1.0f, timer) * 100);
                loadingText.text = $"{percent}%";

                loadingBar.fillAmount = Mathf.Lerp(0.9f, 1.0f, timer);
                if (loadingBar.fillAmount >= 1f)
                {
                    async.allowSceneActivation = true;
                }
            }
        }
        ChangeSceneManagerCall();
        eCurrnetState = eLoadingState.NONE;
    }

    #endregion

    #region 기타함수

    void ChangeSceneManagerCall()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeSceneManaging();
        }
    }

    #endregion


}
