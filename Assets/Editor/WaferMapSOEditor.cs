using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaferMapSO))]
public class WaferMapSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Update Section Assignments"))
        {
            serializedObject.Update();
            var waferMap = (WaferMapSO) target;

            var waferLayout = new WaferLayout(waferMap.ChunkDimSize);
            var waferSections = waferLayout.DivideSampleIntoSections(waferMap.SectionDimSize, .4967f);
            waferMap.SetSectionAssignments(waferSections.Select(x => new SectionAssignment
            {
                rowNum = x.SectionIndices.x,
                colNum = x.SectionIndices.y,
                chunkLimits = $"Row Limits: {x.MinChunkRow}, {x.MaxChunkRow}\nCol Limits: {x.MinChunkCol}, {x.MaxChunkCol}"
            }).ToList());

            serializedObject.ApplyModifiedProperties();
        }
        
    }
}
