using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneSO))]
public class SceneSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        bool hasSceneReference = CheckSceneReference();

        // Iterate through all the serialized properties, except the one named "m_IsValid"
        SerializedProperty property = serializedObject.GetIterator();
        bool enterChildren = true;
        while (property.NextVisible(enterChildren))
        {
            enterChildren = false;
            if (property.propertyType == SerializedPropertyType.ObjectReference && property.name == "m_Script")
            {
                DrawDisabledProperty(property);
                continue;
            }

            if (property.name == "scene")
            {
                EditorGUILayout.PropertyField(property, true);
                continue;
            }

            if (!hasSceneReference) continue;
            
            if (property.name is "sceneName" or "sceneIndex" or "scenePath")
            {
                DrawDisabledProperty(property);
                continue;
            }
            EditorGUILayout.PropertyField(property, true);
        }
        serializedObject.ApplyModifiedProperties();
    }

    void DrawDisabledProperty(SerializedProperty property)
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(property, true);
        EditorGUI.EndDisabledGroup();
    }

    bool CheckSceneReference()
    {
        var scriptableObject = (SceneSO)target;
        return scriptableObject.Scene != null;
    }
}
