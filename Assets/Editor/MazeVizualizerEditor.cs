using Nox7atra.Mazes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(MazeVisualizer))]
public class MazeVizualizerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Refresh"))
        {
            ((MazeVisualizer)target).RefreshMaze();
        }
    }
}
