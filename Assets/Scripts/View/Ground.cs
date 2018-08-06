using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{

    private GameObject grid;


    private Vector2Int size;
    private Direction dir;
    public PlaceableItem PlaceableItem;

    void Start()
    {
        grid = transform.Find("grid").gameObject;
    }

    public void Init(Vector2Int size)
    {
        this.size = size;
        PlaceableItem = new PlaceableItem(size);
    }

    public void ShowGrid(bool show)
    {
        grid.SetActive(show);
    }

}
