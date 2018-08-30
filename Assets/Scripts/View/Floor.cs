using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{

    private GameObject grid;

    private Vector2Int size;
    private Direction dir;
    public PlaceableItem PlaceableItem;

    public void Init(Vector2Int size)
    {
        grid = transform.Find("grid").gameObject;

        this.size = size;
        PlaceableItem = new PlaceableItem(size);
    }

    public void ShowGrid(bool show)
    {
        grid.SetActive(show);
    }

}
