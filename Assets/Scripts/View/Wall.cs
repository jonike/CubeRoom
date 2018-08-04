using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Wall : MonoBehaviour
{

    private GameObject mesh;
    private GameObject grid;
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;

    private Vector2Int size;
    private Direction dir;
    public PlaceableItem PlaceableItem;
    void Start()
    {
        mesh = transform.Find("mesh").gameObject;
        grid = transform.Find("grid").gameObject;
        meshRenderer = mesh.GetComponent<MeshRenderer>();
        boxCollider = mesh.GetComponent<BoxCollider>();
    }

    public void Init(Vector2Int size, Direction dir)
    {
        this.size = size;
        this.dir = dir;
        PlaceableItem = new PlaceableItem(size);
    }

    public void Hide(bool hide)
    {
        meshRenderer.shadowCastingMode = hide ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On;
        boxCollider.enabled = !hide;
    }

    public void ShowGrid(bool show)
    {
        grid.SetActive(show);
    }
}
