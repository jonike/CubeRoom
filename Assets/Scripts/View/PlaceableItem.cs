using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableItem
{
    private Vector2Int size;
    private ItemObject[,] space;
    public PlaceableItem(Vector2Int size)
    {
        this.size = size;
        space = new ItemObject[size.x, size.y];
    }

    // public void PlaceSpace(ItemObject item, int minX, int maxX, int minY, int maxY)
    // {
    //     for (int x = minX; x <= maxX; x++)
    //     {
    //         for (int y = minY; y <= maxY; y++)
    //         {
    //             Debug.Log(x + " " + y);
    //             space[x, y] = item;
    //         }
    //     }
    // }


}