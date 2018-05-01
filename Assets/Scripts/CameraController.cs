using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// Use this for initialization

	private float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
    private float orthoZoomSpeed = 0.05f;        // The rate of change of the orthographic size in orthographic mode.

    private float moveSpeed = 0.1f;
	private float orthographicMin = 2.0f;
	private float orthographicMax = 5.0f;

	private Camera camera;

#if UNITY_EDITOR
	void OnGUI() {
		if (GUI.RepeatButton(new Rect(0, 0, 50, 20), "放大")) {  
            zoom(-1f);
        }

		if (GUI.RepeatButton(new Rect(60, 0, 50, 20), "缩小")) {  
            zoom(1f);
        }

        if (GUI.RepeatButton(new Rect(30, 30, 50, 20), "↑")) {  
            move(new Vector2(0, 1));
        }

        if (GUI.RepeatButton(new Rect(30, 90, 50, 20), "↓")) {  
            move(new Vector2(0, -1));
        }

        if (GUI.RepeatButton(new Rect(0, 60, 50, 20), "←")) {  
            move(new Vector2(-1, 0));
        }

        if (GUI.RepeatButton(new Rect(60, 60, 50, 20), "→")) {  
            move(new Vector2(1, 0));
        }
    
	}
#endif

	void Start () {
		camera = GetComponent<Camera>();
	}

    void Update() {
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // If the camera is orthographic...

			zoom(deltaMagnitudeDiff);
        }
    }

	private void zoom(float deltaMagnitudeDiff) {
		if (camera.orthographic) {
            camera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, orthographicMin, orthographicMax);
        } else {
            camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
            camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 0.1f, 179.9f);
        }
	}

    private void move(Vector2 deltaVec) {
        float rotationY = camera.transform.rotation.y;
        Vector3 cameraPosition =  camera.transform.position;
        cameraPosition.y += deltaVec.y * moveSpeed;
        cameraPosition.x += -Mathf.Cos(rotationY) * deltaVec.x * moveSpeed;
        cameraPosition.z += -Mathf.Sin(rotationY) * deltaVec.x * moveSpeed;

        camera.transform.position = cameraPosition;
    }
    

}
