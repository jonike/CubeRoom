using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    // temp
    private Vector3i roomSize = new Vector3i(6, 6, 6);

	private float minX = -3f;
	private float maxX = 3f;
	private float minZ = -3f;
	private float maxZ = 3f;

    // end temp

    private Room room;
    private RoomObject roomObject;

    private ItemObject currentItemObject;
    private void Awake(){ 
        room = new Room(roomSize);
        GameObject roomGO = Instantiate(Resources.Load("Prefabs/Room")) as GameObject;
        roomObject = roomGO.GetComponent<RoomObject>();
    }

#if UNITY_EDITOR
	void OnGUI() {
		if (GUI.Button(new Rect(0, 150, 50, 20), "+ Item")) {  
            AddItem();
        }

        if (GUI.Button(new Rect(0, 180, 50, 20), "Ok Item")) {  
            PlaceItem();
        }
	}
#endif

    private void AddItem() {
        Vector3i size = new Vector3i(3, 1, 2); // TODO
        Item item = new Item(size);

        GameObject itemGO = Instantiate(Resources.Load("Prefabs/Item")) as GameObject;
        currentItemObject = itemGO.GetComponent<ItemObject>();
        currentItemObject.OnDrag = DragItem;
        currentItemObject.OnDragBefore = BeforeDragItem;
        currentItemObject.OnDragAfter = AfterDragItem;
        currentItemObject.Size = size;

        currentItemObject.SetActive();
    }

    private void PlaceItem() {
        if (!currentItemObject) return;
        currentItemObject.SetInactive();
        currentItemObject = null;
    }

    private void BeforeDragItem() {
        float z = currentItemObject.transform.position.z;
        Plane plane = new Plane(Vector3.back, z);

    	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    	float distance;
    	if(plane.Raycast(ray, out distance)) {
			Vector3 mousePosition = ray.GetPoint(distance);
            currentItemObject.dragY = mousePosition.y;
        }
    }

    private void DragItem() {
        Vector3i size = currentItemObject.Size;
		Plane plane = new Plane(Vector3.down, currentItemObject.dragY);

    	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    	float distance;
    	if(plane.Raycast(ray, out distance)) {
			Vector3 mousePosition = ray.GetPoint(distance);
			float x = Mathf.Clamp(Mathf.Round(mousePosition.x - 0.5f * size.x % 2) + 0.5f * size.x % 2, minX + 0.5f * size.x, maxX - 0.5f * size.x);
			float z = Mathf.Clamp(Mathf.Round(mousePosition.z - 0.5f * size.z % 2) + 0.5f * size.z % 2, minZ + 0.5f * size.z, maxZ - 0.5f * size.z);
			Vector3 objPosition = new Vector3(x, 0, z);
       		currentItemObject.transform.position = objPosition;
    	}
	
    }

     private void AfterDragItem() {
         currentItemObject.dragY = 0.0f;
     }

}
