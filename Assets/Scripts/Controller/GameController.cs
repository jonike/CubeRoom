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

    private Item currentItem;
    private ItemObject currentItemObject;

    private void Awake(){ 
        room = new Room(roomSize);
        GameObject roomGO = Instantiate(Resources.Load("Prefabs/Room")) as GameObject;
        roomObject = roomGO.GetComponent<RoomObject>();
    }

#if UNITY_EDITOR
	void OnGUI() {
		if (GUI.Button(new Rect(0, 150, 50, 20), "+ Item")) { 
            if (currentItem != null) return; 
            AddItem();
        }

        if (GUI.Button(new Rect(0, 180, 50, 20), "Rotate Item")) { 
            if (currentItem == null) return; 
            RotateItem();
        }

        if (GUI.Button(new Rect(0, 210, 50, 20), "Ok Item")) {
            if (currentItem == null) return; 
            PlaceItem();
        }
	}
#endif

    private void AddItem() {
        Vector3i size = new Vector3i(3, 1, 2); // TODO
        currentItem = new Item(size);

        GameObject itemGO = Instantiate(Resources.Load("Prefabs/Item")) as GameObject;
        currentItemObject = itemGO.GetComponent<ItemObject>();

        currentItemObject.OnDrag = DragItem;
        currentItemObject.OnDragBefore = BeforeDragItem;
        currentItemObject.OnDragAfter = AfterDragItem;

        currentItemObject.Size = currentItem.Size;
        currentItemObject.RotateSize = currentItem.Size;

        currentItemObject.transform.position = realPos(Vector3.zero, size.x, size.z);

        currentItemObject.SetActive();
    }

    private void PlaceItem() {
        if (!currentItemObject) return;
        currentItemObject.SetInactive();
        // TODO current item
        currentItem = null;
        currentItemObject = null;
    }

    private void RotateItem() {
        currentItem.Dir.Next();
        Vector3 eulerAngles = currentItemObject.transform.eulerAngles;
        eulerAngles.y = currentItem.Dir.Rotation();
        currentItemObject.transform.eulerAngles = eulerAngles;

        bool isFlipped = currentItem.Dir.IsFlipped();
        Vector3i size = currentItem.Size;
        Vector3i rotateSize = isFlipped ? new Vector3i(size.z, size.y, size.x) : size;
        currentItemObject.RotateSize = rotateSize;
        Vector3 pos = currentItemObject.transform.position;
        
        currentItemObject.transform.position = realPos(pos, rotateSize.x, rotateSize.z);
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
        Vector3i size = currentItemObject.RotateSize;
    
		Plane plane = new Plane(Vector3.down, currentItemObject.dragY);

    	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    	float distance;
    	if(plane.Raycast(ray, out distance)) {
			Vector3 mousePosition = ray.GetPoint(distance);
            Vector3 objPosition = realPos(mousePosition, size.x, size.z);
            objPosition.y = 0;
       		currentItemObject.transform.position = objPosition;
    	}
	
    }

     private void AfterDragItem() {
         currentItemObject.dragY = 0.0f;
     }

     // util
     private Vector3 realPos(Vector3 pos, int sx, int sz) {
        float x = Mathf.Clamp(Mathf.Round(pos.x - 0.5f * sx % 2) + 0.5f * sx % 2, minX + 0.5f * sx, maxX - 0.5f * sx);
		float z = Mathf.Clamp(Mathf.Round(pos.z - 0.5f * sz % 2) + 0.5f * sz % 2, minZ + 0.5f * sz, maxZ - 0.5f * sz);
		
        return new Vector3(x, pos.y, z);
     }

}
