using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour {

	float minX = -3f;
	float maxX = 3f;
	float minZ = -3f;
	float maxZ = 3f;

	public delegate void VoidAction();
	public VoidAction OnDrag;
	public VoidAction OnDragBefore;
	public VoidAction OnDragAfter;

	[HideInInspector]
	public float dragY = 0.0f;

	public Vector3i Size { get; set; } 
	private GameObject grid;

	private bool isActive;

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
		
		grid = Instantiate(Resources.Load("Prefabs/ItemGrid")) as GameObject;
		grid.transform.localScale = new Vector3(Size.x * 0.1f, 1, Size.z * 0.1f);
		grid.transform.parent = transform;
		Vector3 gridPos = transform.position;
		gridPos.y = 0.002f;
		grid.transform.position = gridPos;

		grid.GetComponent<Renderer>().material.mainTextureScale = new Vector2(Size.x, Size.z);
	}
	
	public void SetInactive() {
		if (!isActive) return;
		isActive = false;
		Destroy(grid);
	}

	public Vector3i[] GetItemPlace() {
		Vector3i[] place = new Vector3i[Size.x * Size.y * Size.z];

		return place;
	}
}
