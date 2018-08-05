using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{

    public Material correctMaterial;
    public Material errorMaterial;

    private Renderer renderer;

    private bool type;
    // Use this for initialization
    public void Init()
    {
        renderer = GetComponent<Renderer>();
        renderer.material.mainTextureScale = new Vector2(1, 1);
        type = true;
    }


    public void SetType(bool type)
    {
        if (this.type == type) return;
        this.type = type;
        renderer.sharedMaterial = type ? correctMaterial : errorMaterial;
    }
}
