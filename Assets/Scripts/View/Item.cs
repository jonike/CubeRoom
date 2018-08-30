using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Horizontal,
    Vertical
}

public enum PlaceType
{
    None,
    Wall,
    Floor,
    Item,
}

public abstract class Item
{
    public Vector3Int Size { get; set; }

    public Vector3Int FlippedSize { get; set; }
    public Vector3Int RotateSize { get; set; }
    public Direction Dir { get; set; }

    // 中心位置
    public Vector3 Position { get; set; }

    // 相对于房间坐标的
    public Vector3Int RoomPosition { get; set; }

    public PlaceType PlaceType;
    public bool IsOccupid { get; set; }
    public abstract void SetEdited(bool edited);

	public abstract bool CanPlaceOfType();

    public abstract Vector3 CenterPositionOffset();
    public abstract Vector2 GetDragOffset(Vector3 position);
    public abstract Plane GetOffsetPlane();
}
