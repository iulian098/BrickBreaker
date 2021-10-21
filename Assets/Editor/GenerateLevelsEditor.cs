using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateLevels))]
public class GenerateLevelsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GenerateLevels generator = (GenerateLevels)target;

        if (GUILayout.Button("Generate"))
        {
            generator.Generate();
        }
    }
}
