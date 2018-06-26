using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour {

	public delegate void VoidAction();
	public VoidAction OnDrag;
	public VoidAction OnDragBefore;
	public VoidAction OnDragAfter;

	[HideInInspector]
	public float dragY = 0.0f;

	public Vector3Int Size { get; set; } 
	public Vector3Int RotateSize { get; set; } 

    public Direction Dir { get; set; }

    // 相对于房间坐标的
    public Vector3Int RoomPosition { get; set; }
	

    // 是否算占据空间
    public bool IsOccupid { get; set; }
	private GridObject grids;
	

	private bool isActive;
	public void Init(Vector3Int size) {
        this.Size = size;
		this.RotateSize = size;
        this.Dir = Direction.A;
        this.IsOccupid = true; // TODO
    }
	void OnMouseDrag() {
		if (OnDrag != null) OnDrag();
	}

	void OnMouseDown() {
		SetActive();
		if (OnDragBefore != null) OnDragBefore();
	}

	void OnMouseUp() {
		if (OnDragAfter != null) OnDragAfter();
	}

	public void SetActive() {
		if (isActive) return;
		isActive = true;
	
	}
	
	public void SetInactive() {
		if (!isActive) return;
		isActive = false;
	}

	// public void SetGridType(bool[,] gridTypes) {
	// 	for (int i = 0; i < Size.z; i++)
	// 	{
	// 		for (int j = 0; j < Size.x; j++)
	// 		{
	// 			grids[i,j].SetType(gridTypes[i,j]);
	// 		}
	// 	}
	// }

}
