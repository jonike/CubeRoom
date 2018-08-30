using UnityEngine;
using UnityEditor;
using System.Collections;

public class CubemapRenderer : MonoBehaviour
{
    public Cubemap cubemap;

    public void RenderCubemap()
    {
        if (cubemap == null)
        {
            Debug.LogWarning("No Cubemap!");
            return;
        }

        GameObject go = new GameObject("CubemapCamera");

        go.AddComponent<Camera>();
        go.transform.position = transform.position;
        go.GetComponent<Camera>().RenderToCubemap(cubemap);


        DestroyImmediate(go);
    }

}