using System.Collections;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    public void CombineMeshes()
    {
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();

        Mesh finalMesh = new Mesh();

        CombineInstance[] combiners = new CombineInstance[filters.Length];

        Debug.Log(filters.Length + " meshs");

        for (int i = 0; i < filters.Length; i++)
        {
            if (filters[i].sharedMesh == null)
                continue;
            if (filters[i].GetComponent<Renderer>().enabled == false)
                continue;

            combiners[i].subMeshIndex = 0;
            combiners[i].mesh = filters[i].sharedMesh;
            combiners[i].transform = filters[i].transform.localToWorldMatrix;
        }

        finalMesh.CombineMeshes(combiners);

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (!meshRenderer)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (!meshFilter)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        meshFilter.sharedMesh = finalMesh;
    }


    public void ExportMeshToOBJ(string path)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        OBJFactory.MeshToFile(meshFilter, path);
    }
}