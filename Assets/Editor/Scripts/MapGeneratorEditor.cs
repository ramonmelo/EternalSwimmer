using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    float MinLimit = 0.0f;
    float MaxLimit = 1.0f;

    SerializedProperty ForwardChanceProperty;
    SerializedProperty TurnChanceProperty;

    private void OnEnable()
    {
        ForwardChanceProperty = serializedObject.FindProperty("ForwardChance");
        TurnChanceProperty = serializedObject.FindProperty("TurnChance");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("LevelContainer"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("BlockPrefab"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MoverPrefab"));

        var forwardChance = ForwardChanceProperty.vector2Value;
        var turnChance = TurnChanceProperty.vector2Value;

        forwardChance.x = MinLimit;
        turnChance.x = forwardChance.y;
        turnChance.y = MaxLimit;

        EditorGUILayout.MinMaxSlider("Forward Chance", ref forwardChance.x, ref forwardChance.y, MinLimit, MaxLimit);
        EditorGUILayout.MinMaxSlider("Turn Chance", ref turnChance.x, ref turnChance.y, MinLimit, MaxLimit);

        EditorGUILayout.LabelField($"Forward Chance: {forwardChance.x * 100}% - {forwardChance.y * 100}%");
        EditorGUILayout.LabelField($"Turn Chance: {turnChance.x * 100}% - {turnChance.y * 100}%");

        ForwardChanceProperty.vector2Value = forwardChance;
        TurnChanceProperty.vector2Value = turnChance;

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Generate Map"))
        {
            (target as MapGenerator).Generate();
        }
    }
}
