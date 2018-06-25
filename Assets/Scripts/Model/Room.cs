using System.Collections.Generic;

public class Room {
    public Vector3i Size { get; set; }

    private List<Item> items;

    private Item[,,] occupiedSpace;

    public Room(Vector3i size) {
        this.Size = size;
        this.items = new List<Item>();
        this.occupiedSpace = new Item[size.x, size.z, size.y];
    }
}
