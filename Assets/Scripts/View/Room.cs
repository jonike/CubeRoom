using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sorumi.Util;
public class Room : MonoBehaviour
{

    public Vector3Int Size { get; set; }

    private Ground ground;

    private Wall[] walls;

    private Pillar[] pillars;

    private string[] wallNames = { "a", "b", "c", "d" };

    private List<ItemBehaviour> items;

    // private ItemBehaviour[,,] occupiedSpace;
    // private ItemBehaviour[,] groundSpace;
    // private ItemBehaviour[,] wallASpace;
    // private ItemBehaviour[,] wallBSpace;
    // private ItemBehaviour[,] wallCSpace;
    // private ItemBehaviour[,] wallDSpace;

    public void Init(Vector3Int size)
    {
        this.Size = size;
        this.items = new List<ItemBehaviour>();
        // this.groundSpace = new ItemBehaviour[size.z * 2, size.x * 2];
        // this.wallASpace = new ItemBehaviour[size.y * 2, size.x * 2];
        // this.wallBSpace = new ItemBehaviour[size.y * 2, size.z * 2];
        // this.wallCSpace = new ItemBehaviour[size.y * 2, size.x * 2];
        // this.wallDSpace = new ItemBehaviour[size.y * 2, size.z * 2];

        //
        ground = transform.Find("ground").GetComponent<Ground>();
        walls = new Wall[4];
        pillars = new Pillar[4];
        for (int i = 0; i < 4; i++)
        {
            Wall wall = transform.Find("wall_" + wallNames[i]).GetComponent<Wall>();
            walls[i] = wall;
            Pillar pillar = transform.Find("v_" + wallNames[i]).GetComponent<Pillar>();
            pillars[i] = pillar;
        }

    }

    public void RefreshByAngle(float angle)
    {
        walls[0].Hide(angle > 270 || angle < 90);
        walls[1].Hide(angle > 0 && angle < 180);
        walls[2].Hide(angle > 90 && angle < 270);
        walls[3].Hide(angle > 180 && angle < 360);

        pillars[0].Hide(angle > 270 || angle < 180);
        pillars[1].Hide(angle > 0 && angle < 270);
        pillars[2].Hide(angle > 90 && angle < 360);
        pillars[3].Hide(angle > 180 || angle < 90);
    }

    public void RefreshGrids(bool isEdited, ItemType itemType)
    {
        if (itemType == ItemType.Horizontal)
        {
            ground.ShowGrid(isEdited);
        }
        else if (itemType == ItemType.Vertical)
        {
            for (int i = 0; i < 4; i++)
            {
                walls[i].ShowGrid(isEdited);
            }
        }
    }


    // ------------
    // public void AddItem(ItemBehaviour item) {
    //     items.Add(item);

    //     bool isFlipped = item.Dir.IsFlipped();
    //     Vector3Int size = item.Size;
    //     Vector3Int rotateSize = isFlipped ? new Vector3Int(size.z, size.y, size.x) : size;

    //     if (item.IsOccupid) {
    //         for (int z = item.RoomPosition.z - rotateSize.z; z < item.RoomPosition.z + rotateSize.z; z++)
    //         {
    //             for (int x = item.RoomPosition.x - rotateSize.x; x < item.RoomPosition.x + rotateSize.x; x++)
    //             {
    //                 groundSpace[z, x] = item;
    //             }
    //         }

    //     }
    // }

    // public List<Vector2Int> ConflictSpace(ItemBehaviour item){
    // 	List<Vector2Int> space = new List<Vector2Int>();

    // 	bool isFlipped = item.Dir.IsFlipped();
    //     Vector3Int size = item.Size;
    //     Vector3Int rotateSize = isFlipped ? new Vector3Int(size.z, size.y, size.x) : size;

    // 	for (int z = item.RoomPosition.z - rotateSize.z; z < item.RoomPosition.z + rotateSize.z; z++)
    //         {
    //             for (int x = item.RoomPosition.x - rotateSize.x; x < item.RoomPosition.x + rotateSize.x; x++)
    //             {
    //                 if (groundSpace[z, x] != null) {
    // 					space.Add(new Vector2Int(x, z));
    // 				}
    //             }
    //         }

    // 	return space;
    // }

}
