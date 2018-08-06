using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType {
	Horizontal,
	Vertical
}

public abstract class Item {
	public Vector3Int Size { get; set; } 

	public Vector3Int FlippedSize { get; set; } 
	public Vector3Int RotateSize { get; set; }  
	public Direction Dir { get; set; }

    // 相对于房间坐标的
    public Vector3Int RoomPosition { get; set; }

	public PlaceableItem PlaceableItem;
	public bool IsOccupid { get; set; }
	public abstract void SetEdited(bool edited);

	public abstract Vector3 PositionOffset();
	public abstract Vector2 GetDragOffset(Vector3 position);
	public abstract Plane GetOffsetPlane(Vector3 position);
}
