using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Pillar : MonoBehaviour
{

    private MeshRenderer meshRenderer;
    // private BoxCollider boxCollider;

    public void Init()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        // boxCollider = GetComponent<BoxCollider>();
    }

    public void Hide(bool hide)
    {
        meshRenderer.shadowCastingMode = hide ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On;
        // boxCollider.enabled = !hide;
    }
}
