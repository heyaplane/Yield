using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(0)] //Before GameManager
public class SceneController : SingletonMonobehaviour<SceneController>
{
    [SerializeField] Canvas canvas;
    [SerializeField] Camera globalUICamera;
    
    [SerializeField] SceneSO startingScene;
    public SceneSO StartingScene => startingScene;

    [SerializeField] bool useStartingScene;

    [SerializeField] SceneSO mainMenu;
    public SceneSO MainMenu => mainMenu;

    [SerializeField] SceneSO profileCreate;
    public SceneSO ProfileCreate => profileCreate;

    [SerializeField] SceneSO newGameScene;
    public SceneSO NewGameScene => newGameScene;

    [SerializeField] SceneSO[] availableScenes;
    public SceneSO[] AvailableScenes => availableScenes;

    Dictionary<int, SceneSO> sceneDictionary;
    
    SceneSO nextScene;
    public SceneSO NextScene => nextScene;

    public bool IsNextSceneMain => nextScene == MainMenu;

    LoadingImageUI loadingImage;

    public int GetBuildIndex() => SceneManager.GetActiveScene().buildIndex;
    public SceneSO GetActiveSceneSO() => sceneDictionary[GetBuildIndex()];
    public SceneSO GetDefaultNextSceneSO() => GetActiveSceneSO().DefaultNextScene;
    public SceneSO GetSceneFromBuildIndex(int index) => sceneDictionary[index];

    public void SetProfileSelectStartingScene() => nextScene = profileCreate;
    public void SetStartingScene() => nextScene = useStartingScene ? startingScene : MainMenu;
    
    protected override void Awake()
    {
        base.Awake();
        sceneDictionary = availableScenes.ToDictionary(scene => scene.SceneIndex, scene => scene);
    }

    public void SwitchToNewScene()
    {
        if (loadingImage != null && loadingImage.IsTransitioning) return;
        if (nextScene == null)
        {
            Debug.LogError("Next scene is null.");
            return;
        }

        CheckForSceneLocks();
        
        StartCoroutine(SwitchScenes());
    }

    public void SetNextScene(SceneSO scene)
    {
        if (scene == null)
            nextScene = GetDefaultNextSceneSO();
        else
            nextScene = scene;
    }

    void CheckForSceneLocks()
    {
        
    }

   IEnumerator SwitchScenes()
   {
        loadingImage = Instantiate(nextScene.LoadingImage, canvas.transform);
        loadingImage.SetUp();
        
        print("About to start fading out");
        yield return AlertThenFadeOutAndTearDownCurrentScene();
        
        print("About to start unloading");
        yield return AlertThenUnloadCurrentScene();
        
        EventManager.CallInBetweenScenes();

        print("About to start loading");
        yield return LoadNextSceneThenAlert(nextScene);

        print("About to start fading in");
        yield return SetUpNextSceneAndFadeThenAlert();
        
        loadingImage.TearDown();
        nextScene = null;
   }

    IEnumerator AlertThenFadeOutAndTearDownCurrentScene()
    {
        EventManager.CallBeforeSceneFadeOut();
        print("Alertthenfade");
        yield return loadingImage.Transition(1);
    }

    IEnumerator AlertThenUnloadCurrentScene()
    {
        EventManager.CallBeforeSceneUnload();

        //globalUICamera.gameObject.SetActive(true);
        var asyncUnload = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        yield return loadingImage.UpdateProgressUI(asyncUnload, 0f);
    }

    IEnumerator LoadNextSceneThenAlert(SceneSO scene)
    {
        yield return LoadSceneAndSetActive(scene);
        //globalUICamera.gameObject.SetActive(false);
        EventManager.CallAfterSceneLoad(scene);
    }

    IEnumerator SetUpNextSceneAndFadeThenAlert()
    {
        yield return loadingImage.Transition(0);
        EventManager.CallAfterSceneFadeIn();
    }

    IEnumerator LoadSceneAndSetActive(SceneSO scene)
    {
        var asyncLoad = SceneManager.LoadSceneAsync(scene.SceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = true;

        yield return loadingImage.UpdateProgressUI(asyncLoad, 0.5f);

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(scene.SceneIndex));
    } 

}
