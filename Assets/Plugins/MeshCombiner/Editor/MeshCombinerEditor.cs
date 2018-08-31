using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshCombiner))]
public class MeshCombinerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MeshCombiner mc = target as MeshCombiner;

        if (GUILayout.Button("Combine Meshes"))
        {
            mc.CombineMeshes();
        }

        if (GUILayout.Button("Export Mesh To OBJ"))
        {
            string path = EditorUtility.SaveFilePanel("Export Mesh To OBJ", "Assets", this.name + ".obj", "obj");
            if (!string.IsNullOrEmpty(path))
            {
                mc.ExportMeshToOBJ(path);
            }
        }
    }
}