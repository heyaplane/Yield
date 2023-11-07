using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FlexUIDataSourceSO))]
public class UpdateFlexUIDataSource : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var propertyUpdater = (FlexUIDataSourceSO)target;

        if (GUILayout.Button("Update Shared Property"))
        {
            propertyUpdater.UpdateSourceDictionaries();
            
            string folderPath = Path.Combine(Application.dataPath, propertyUpdater.Path);
            Debug.Log(propertyUpdater.Path);
            
            if (Directory.Exists(folderPath))
            {
                FlexUIEventSO dataEventSO = null;
                string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets/" + propertyUpdater.Path });
                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                    if (AssetDatabase.LoadAssetAtPath(assetPath, typeof(FlexUIDataSO)) is FlexUIDataSO scriptableObject)
                    {
                        scriptableObject.UpdateDataSource(propertyUpdater);
                        EditorUtility.SetDirty(scriptableObject);
                        continue;
                    }
                    
                    if (AssetDatabase.LoadAssetAtPath(assetPath, typeof(FlexUIEventSO)) is FlexUIEventSO dataSO)
                    {
                        dataEventSO = dataSO;
                    }
                }
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                if (dataEventSO != null)
                {
                    dataEventSO.DataSourceUpdatedEvent?.Invoke();
                }
            }
            else
            {
                Debug.LogError("ScriptableObjects folder not found. Please make sure the folder path is correct.");
            }
        }
    }
}