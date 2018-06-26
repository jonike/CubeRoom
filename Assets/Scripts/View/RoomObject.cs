using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObject : MonoBehaviour {

    public Vector3Int Size { get; set; }

    private List<ItemObject> items;

    // private Item[,,] occupiedSpace;
    private ItemObject[,] groundSpace;
    private ItemObject[,] wallASpace;
    private ItemObject[,] wallBSpace;
    private ItemObject[,] wallCSpace;
    private ItemObject[,] wallDSpace;

    public void Init(Vector3Int size) {
        this.Size = size;
        this.items = new List<ItemObject>();
        this.groundSpace = new ItemObject[size.z * 2, size.x * 2];
        this.wallASpace = new ItemObject[size.y * 2, size.x * 2];
        this.wallBSpace = new ItemObject[size.y * 2, size.z * 2];
        this.wallCSpace = new ItemObject[size.y * 2, size.x * 2];
        this.wallDSpace = new ItemObject[size.y * 2, size.z * 2];
    }



    public bool AddItem(ItemObject item) {
        items.Add(item);

        bool isFlipped = item.Dir.IsFlipped();
        Vector3Int size = item.Size;
        Vector3Int rotateSize = isFlipped ? new Vector3Int(size.z, size.y, size.x) : size;

        if (item.IsOccupid) {
            for (int z = item.RoomPosition.z - rotateSize.z; z < item.RoomPosition.z + rotateSize.z; z++)
            {
                for (int x = item.RoomPosition.x - rotateSize.x; x < item.RoomPosition.x + rotateSize.x; x++)
                {
					if (groundSpace[z, x] != null)
						return false; 
                    groundSpace[z, x] = item;
                }
            }
        
        }

		return true;
    }

	public List<Vector2Int> ConflictSpace(ItemObject item){
		List<Vector2Int> space = new List<Vector2Int>();

		bool isFlipped = item.Dir.IsFlipped();
        Vector3Int size = item.Size;
        Vector3Int rotateSize = isFlipped ? new Vector3Int(size.z, size.y, size.x) : size;

		for (int z = item.RoomPosition.z - rotateSize.z; z < item.RoomPosition.z + rotateSize.z; z++)
            {
                for (int x = item.RoomPosition.x - rotateSize.x; x < item.RoomPosition.x + rotateSize.x; x++)
                {
                    if (groundSpace[z, x] != null) {
						space.Add(new Vector2Int(x, z));
					}
                }
            }

		return space;
	}

}
