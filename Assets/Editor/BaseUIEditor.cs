using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseUI), true)]
public class BaseUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var baseUI = (BaseUI) target;
        
        SerializedProperty property = serializedObject.GetIterator();
        bool enterChildren = true;
        while (property.NextVisible(enterChildren))
        {
            enterChildren = false;
            if (property.name is "toggleAction")
            {
                if (baseUI.IsPermanent)
                    continue;
            }
            EditorGUILayout.PropertyField(property, true);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
