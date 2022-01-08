using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Level))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Map"))
        {
            (target as Level).GenerateLevels(1, 3);
        }

        if (GUILayout.Button("Clean Map"))
        {
            (target as Level).CleanLevels();
        }
    }
}
