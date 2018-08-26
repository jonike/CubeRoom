using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditedItem : MonoBehaviour {

	public Item Item;
	public Action OnDrag;
	public Action OnDragBefore;
	public Action OnDragAfter;
	
	/* 
	 * VerticalItem 计算 y 和 x
	 * HorizontalItem 只计算 y
	 */
	public Vector2 DragOffset;

	public bool CanOutside = true;

	public bool CanPlaced = false;

	public void Init() {
		Item = GetComponent<ItemObject>().Item;
	}
	void OnMouseDown() {
		if (OnDragBefore != null) OnDragBefore();
	}

	void OnMouseDrag() {
		if (OnDrag != null) OnDrag();
	}

	void OnMouseUp() {
		if (OnDragAfter != null) OnDragAfter();
	}

	public void SetDragOffset(Vector3 position) {
		// Debug.Log("relative pos: " + position + transform.position);
		position = position - Item.Position;
		// Debug.Log("relative pos: " + position);
		DragOffset = Item.GetDragOffset(position);
		// Debug.Log("dragOffset: " + DragOffset);
	}
}
