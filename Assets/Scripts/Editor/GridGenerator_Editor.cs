using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridGenerator))]
public class GridGenerator_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var typeProp = serializedObject.FindProperty("gridType");
        EditorGUILayout.PropertyField(typeProp);
        EditorGUILayout.Space(6);

        var type = (GridType)typeProp.enumValueIndex;

        switch (type)
        {
            case GridType.Hex:
                DrawHexSection();
                break;

            case GridType.Square:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("squarePrefab"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("squareColumns"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("squareRows"));
                break;

            case GridType.Octagon:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("octagonPrefab"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("octagonFillPrefab"),
                    new GUIContent("Fill Prefab (optional)"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("octagonRadius"));
                break;
        }

        // Palette only relevant for Square / Octagon.
        if (type != GridType.Hex)
        {
            EditorGUILayout.Space(6);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("palette"), true);
        }

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(20);

        var grid = (GridGenerator)target;

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

        // Biome stats (read-only, shown after generation).
        if (grid.BiomeGroups.Count > 0)
        {
            EditorGUILayout.Space(6);
            int blocking  = 0;
            int passable  = 0;
            for (int i = 0; i < grid.BiomeGroups.Count; i++)
                foreach (var h in grid.BiomeGroups[i])
                    if (grid.IsBlocking(h)) blocking++; else passable++;

            EditorGUILayout.HelpBox(
                $"Biomes: {grid.BiomeGroups.Count}  |  Largest: {grid.BiomeGroups[0].Count} tiles\n" +
                $"Passable: {passable}   Blocking: {blocking}",
                MessageType.Info);
        }
    }

    private void DrawHexSection()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hexPrefab"),
            new GUIContent("Hex Prefab"));

        EditorGUILayout.Space(4);

        // Biome types array — each entry folds open to show its name + mesh list.
        var biomesProp = serializedObject.FindProperty("hexBiomeTypes");
        EditorGUILayout.PropertyField(biomesProp,
            new GUIContent("Biome Types", "Each biome owns a pool of mesh variants. " +
                           "A random mesh from the pool is picked for every tile in that biome."),
            includeChildren: true);

        EditorGUILayout.Space(4);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hexRadius"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tileScale"),
            new GUIContent("Tile Scale", "Uniform scale applied to every spawned tile. " +
                           "Spacing is recomputed automatically to match."));

        EditorGUILayout.Space(4);

        // Biome generation settings.
        var countProp = serializedObject.FindProperty("biomeCount");
        EditorGUILayout.PropertyField(countProp,
            new GUIContent("Biome Count",
                "Number of Voronoi seed regions. " +
                "0 = one region per biome type (recommended starting point). " +
                "Use multiples of the biome-type count for several blobs per type."));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("biomeSeedSeparation"),
            new GUIContent("Seed Separation",
                "Minimum hex-distance between two seeds. 0 = auto-computed."));

        // Show an info line so the effective count is always visible.
        var grid       = (GridGenerator)target;
        var biomeTypes = serializedObject.FindProperty("hexBiomeTypes");
        int effective  = countProp.intValue > 0 ? countProp.intValue : biomeTypes.arraySize;
        if (biomeTypes.arraySize > 0)
        {
            EditorGUILayout.HelpBox(
                $"Effective seed count: {effective}  ({biomeTypes.arraySize} biome type(s) defined)",
                MessageType.None);
        }

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Neutral Zone (Boss Arena)", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("neutralBiome"),
            new GUIContent("Neutral Biome", "Meshes used for the 7-hex boss arena."),
            includeChildren: true);

        var randomizeProp = serializedObject.FindProperty("randomizeNeutralPosition");
        EditorGUILayout.PropertyField(randomizeProp,
            new GUIContent("Random Position", "Pick a new random position each generation."));

        if (!randomizeProp.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Center (Q, R)",
                "Axial coordinates of the zone center. Must be within hexRadius - 1 from origin."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("neutralCenterQ"), GUIContent.none);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("neutralCenterR"), GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }

        // Show the actual center after generation.
        if (grid.BiomeGroups.Count > 0)
        {
            var c = grid.NeutralZoneCenter;
            EditorGUILayout.HelpBox($"Neutral zone center last placed at Q={c.Q}, R={c.R}",
                MessageType.None);
        }
    }
}
