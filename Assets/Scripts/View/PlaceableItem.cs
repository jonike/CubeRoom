using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableItem
{
    private Vector2Int size;
    private ItemObject[,] space;

    public PlaceableItem(Vector2Int size) {
        this.size = size;
        space = new ItemObject[size.x, size.y];
    }


}