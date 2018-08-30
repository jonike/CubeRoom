using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CubemapRenderer))]
public class CubemapRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CubemapRenderer cr = target as CubemapRenderer;

        if (GUILayout.Button("Render Cubemap"))
        {
            cr.RenderCubemap();
        }
    }
}