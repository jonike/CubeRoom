using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ColliderTool))]
public class ColliderToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ColliderTool ct = target as ColliderTool;

        if (GUILayout.Button("Add Box Collider"))
        {
            ct.AddBoxCollider();
        }
    }
}