public class Item {
    public Vector3i Size { get; set; }

    public Direction Dir { get; set; }

    public Vector3i Position { get; set; }

    // 是否算占据空间
    public bool IsOccupid { get; set; }

    public Item (Vector3i size) {
        this.Size = size;
    }

}