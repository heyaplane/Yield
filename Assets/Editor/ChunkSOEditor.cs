using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CustomEditor(typeof(ChunkSO))]
public class ChunkSOEditor : Editor
{
    private SerializedProperty spriteReferencesProperty;

    private void OnEnable()
    {
        // Cache the SerializedProperty
        spriteReferencesProperty = serializedObject.FindProperty("spriteRefs");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // Load the actual serialized data

        // Draw the default inspector
        DrawDefaultInspector();

        // Drag and drop functionality
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag Sprite Assets Here");

        if (dropArea.Contains(evt.mousePosition))
        {
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            // Only accept Texture2D or Sprite objects
                            if (draggedObject is Texture2D || draggedObject is Sprite)
                            {
                                // Get the path to the asset
                                string assetPath = AssetDatabase.GetAssetPath(draggedObject);

                                // Add a new element to the array
                                spriteReferencesProperty.arraySize++;
                                SerializedProperty element = spriteReferencesProperty.GetArrayElementAtIndex(spriteReferencesProperty.arraySize - 1);
                                
                                // Create a new AssetReference
                                AssetReference assetReference = new AssetReference(AssetDatabase.AssetPathToGUID(assetPath));
                                
                                // Assign the AssetReference to the serialized property
                                element.FindPropertyRelative("m_AssetGUID").stringValue = assetReference.AssetGUID;
                            }
                        }

                        serializedObject.ApplyModifiedProperties(); // Save the changes
                    }
                    break;
            }
        }

        serializedObject.ApplyModifiedProperties(); // Apply changes to the serializedProperty
    }
}
