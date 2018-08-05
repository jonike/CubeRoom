using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{

    private GameObject grid;


    private Vector2Int size;
    private Direction dir;
    public PlaceableItem PlaceableItem;
    private List<ItemObject> items;

    void Start()
    {
        grid = transform.Find("grid").gameObject;
    }

    public void Init(Vector2Int size)
    {
        this.size = size;
        PlaceableItem = new PlaceableItem(size);
        this.items = new List<ItemObject>();
    }

    public void ShowGrid(bool show)
    {
        grid.SetActive(show);
    }

    public void PlaceItem(ItemObject item)
    {
        // items.Add(item);

        // Vector3Int rotateSize = item.Item.RotateSize;
		// Vector2Int size = 

        // if (item.IsOccupid)
        // {
        //     for (int z = item.RoomPosition.z - rotateSize.z; z < item.RoomPosition.z + rotateSize.z; z++)
        //     {
        //         for (int x = item.RoomPosition.x - rotateSize.x; x < item.RoomPosition.x + rotateSize.x; x++)
        //         {
        //             groundSpace[z, x] = item;
        //         }
        //     }

        // }
    }
}
