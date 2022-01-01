using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Map"))
        {
            (target as MapGenerator).GenerateLevels(1, 3);
        }

        if (GUILayout.Button("Clean Map"))
        {
            (target as MapGenerator).CleanLevels();
        }
    }
}
