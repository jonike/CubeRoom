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
	private GameObject grids;

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
		
		grids = new GameObject();
		grids.transform.localScale = Vector3.one;
		grids.transform.parent = transform;
		Vector3 gridsPos = transform.position;
		gridsPos.y = 0.002f;
		grids.transform.position = gridsPos;
		for (int i = 0; i < Size.z; i++)
		{
			for (int j = 0; j < Size.x; j++)
			{
				GameObject grid = Instantiate(Resources.Load("Prefabs/ItemGrid")) as GameObject;
					grids.transform.localScale = new Vector3(1, 1, 1);
					grid.transform.parent = grids.transform;
					float x = gridsPos.x + j - 0.5f * Size.x + 0.5f;
					float z = gridsPos.z + i - 0.5f * Size.z + 0.5f;
					grid.transform.position = new Vector3(x, gridsPos.y, z);

					grid.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1, 1);
			}
		}
	}
	
	public void SetInactive() {
		if (!isActive) return;
		isActive = false;
		Destroy(grids);
	}

	public Vector3i[] GetItemPlace() {
		Vector3i[] place = new Vector3i[Size.x * Size.y * Size.z];

		return place;
	}
}
