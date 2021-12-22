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
            (target as MapGenerator).Generate();
        }

        if (GUILayout.Button("Clean Map"))
        {
            (target as MapGenerator).CleanLevels();
        }
    }
}
