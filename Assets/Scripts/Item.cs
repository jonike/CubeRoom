using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	float minX = -3.0f;
	float maxX = 2.0f;
	float minZ = -3.0f;
	float maxZ = 2.0f;

	public float yAxis = 0.5f;

	void OnMouseDrag() {
		
		Plane plane = new Plane(Vector3.down, yAxis);
    	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    	float distance;
    	if(plane.Raycast(ray, out distance)) {
			Vector3 mousePosition = ray.GetPoint(distance);
			float x = Mathf.Clamp(Mathf.Round(mousePosition.x - 0.5f), minX, maxX) + 0.5f;
			float z = Mathf.Clamp(Mathf.Round(mousePosition.z - 0.5f), minZ, maxZ) + 0.5f;
			Vector3 objPosition = new Vector3(x, yAxis, z);
       		transform.position = objPosition;
    	}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
