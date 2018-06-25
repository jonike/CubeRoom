using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObject : MonoBehaviour {

	public GameObject yGrid;
	public GameObject zGrid;
	public GameObject xGrid;

	public Vector3i size;

	private bool[] spaceAvailable;
	// Use this for initialization
	void Start () {
		spaceAvailable = new bool[size.x * size.z + size.x * size.y + size.z * size.y];
	}
	
	// public void PlaceItem(Item item) {

	// }
}
