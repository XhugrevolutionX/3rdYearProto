using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexGrid))]
public class HexGrid_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        HexGrid grid = (HexGrid)target;
        DrawDefaultInspector();

        EditorGUILayout.Space(20);

        if (GUILayout.Button("Generate"))
        {
            grid.GenerateGrid();
            EditorUtility.SetDirty(grid);
        }

        if (GUILayout.Button("Clear"))
        {
            grid.ClearGrid();
            EditorUtility.SetDirty(grid);
        }
    }
}
