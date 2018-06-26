using System.Collections.Generic;
using UnityEngine;

public class Room {
    // public Vector3Int Size { get; set; }

    // private List<Item> items;

    // // private Item[,,] occupiedSpace;
    // private Item[,] groundSpace;
    // private Item[,] wallASpace;
    // private Item[,] wallBSpace;
    // private Item[,] wallCSpace;
    // private Item[,] wallDSpace;

    // public Room(Vector3Int size) {
    //     this.Size = size;
    //     this.items = new List<Item>();
    //     this.groundSpace = new Item[size.z * 2, size.x * 2];
    //     this.wallASpace = new Item[size.y * 2, size.x * 2];
    //     this.wallBSpace = new Item[size.y * 2, size.z * 2];
    //     this.wallCSpace = new Item[size.y * 2, size.x * 2];
    //     this.wallDSpace = new Item[size.y * 2, size.z * 2];
    // }

    // public void AddItem(Item item) {
    //     items.Add(item);

    //     bool isFlipped = item.Dir.IsFlipped();
    //     Vector3Int size = item.Size;
    //     Vector3Int rotateSize = isFlipped ? new Vector3Int(size.z, size.y, size.x) : size;
       

    //     if (item.IsOccupid) {
    //         for (int z = item.Position.z - rotateSize.z; z < rotateSize.z + rotateSize.z; z++)
    //         {
    //             for (int x = item.Position.x - rotateSize.x; x < rotateSize.x + rotateSize.x; x++)
    //             {
    //                 Debug.Log(x + ", " + z);
    //                 groundSpace[z, x] = item;
    //             }
    //         }
        
    //     }
    // }

    

}
