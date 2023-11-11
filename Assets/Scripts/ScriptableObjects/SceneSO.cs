using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSceneSO", menuName = "Scriptable Object/Scene")]
public class SceneSO : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] string sceneName;
    public string SceneName => sceneName;

    [SerializeField] int sceneIndex;
    public int SceneIndex => sceneIndex;

    [SerializeField] string scenePath;
    public string ScenePath => scenePath;

    [SerializeField] LoadingImageUI loadingImage;
    public LoadingImageUI LoadingImage => loadingImage;

    [SerializeField] GameState gameStateOnLoading;
    public GameState GameStateOnLoading => gameStateOnLoading;

    [SerializeField] SceneSO defaultNextScene;
    public SceneSO DefaultNextScene => defaultNextScene;

#if UNITY_EDITOR
    [SerializeField] SceneAsset scene;
    public SceneAsset Scene => scene;
#endif
    
    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        sceneName = scene != null ? scene.name : null;
        scenePath = scene != null ? AssetDatabase.GetAssetPath(scene) : null;
        sceneIndex = -2;

        EditorBuildSettingsScene[] buildSettingsScenes = EditorBuildSettings.scenes;
        if (buildSettingsScenes.Length > 0)
        {
            for (int i = 0; i < buildSettingsScenes.Length; i++)
            {
                if (buildSettingsScenes[i].path == scenePath)
                {
                    sceneIndex = i;
                    break;
                }
            }
        }
#endif
    }

    public void OnAfterDeserialize() { }
}
