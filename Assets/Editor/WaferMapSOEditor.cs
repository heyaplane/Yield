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

            var waferLayout = new WaferLayout(waferMap.ChunkDimSize, waferMap.SectionDimSize);
            var waferSections = waferLayout.DivideWaferIntoSections(.4967f);
            var sectionData = waferMap.WaferFeatures.Select(x => new SectionData {
                Feature = new DataFeature
            {
                FeatureName = x.FeatureName,
                Units = x.Units
            }}).ToList();
            waferMap.SetSectionAssignments(waferSections.Select(x => new SectionAssignment
            {
                rowNum = x.SectionIndices.x,
                colNum = x.SectionIndices.y,
                sectionData = sectionData.ToList(), 
                chunkLimits = $"Row Limits: {x.MinChunkRow}, {x.MaxChunkRow}\nCol Limits: {x.MinChunkCol}, {x.MaxChunkCol}"
            }).ToList());

            serializedObject.ApplyModifiedProperties();
        }
        
    }
}
