using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RoomCamera : MonoBehaviour {

	// Use this for initialization
    private float defaultRotationY = -135f;
    private float defaultPositionX = 9f;
    private float defaultPositionZ = 9f;
    private float defaultPositionY = 10.3f;
	private float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
    private float orthoZoomSpeed = 0.05f;        // The rate of change of the orthographic size in orthographic mode.

    private float MoveSpeed = 0.1f;
    private float RotateSpeed = 1f;
	private float orthographicMin = 2.0f;
	private float orthographicMax = 5.0f;

	private Camera camera;

    public delegate void FloatDelegate(float value);
    public FloatDelegate OnCameraRotate;

    // record
    private float rotateDegree = 0;
    private float moveVertical = 0;
    private float moveHorizontal = 0;

#if UNITY_EDITOR
	void OnGUI() {
		if (GUI.RepeatButton(new Rect(0, 0, 50, 20), "放大")) {  
            Zoom(-1f);
        }

		if (GUI.RepeatButton(new Rect(60, 0, 50, 20), "缩小")) {  
            Zoom(1f);
        }

        if (GUI.RepeatButton(new Rect(0, 30, 50, 20), "左转")) {  
            Rotate(-1f);
        }

		if (GUI.RepeatButton(new Rect(60, 30, 50, 20), "右转")) {  
            Rotate(1f);
        }

        if (GUI.RepeatButton(new Rect(30, 60, 50, 20), "↑")) {  
            Move(new Vector2(0, 1));
        }

        if (GUI.RepeatButton(new Rect(30, 120, 50, 20), "↓")) {  
            Move(new Vector2(0, -1));
        }

        if (GUI.RepeatButton(new Rect(0, 90, 50, 20), "←")) {  
            Move(new Vector2(-1, 0));
        }

        if (GUI.RepeatButton(new Rect(60, 90, 50, 20), "→")) {  
            Move(new Vector2(1, 0));
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

			Zoom(deltaMagnitudeDiff);
        }
    }

	private void Zoom(float deltaMagnitudeDiff) {
		if (camera.orthographic) {
            camera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, orthographicMin, orthographicMax);
        } else {
            camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
            camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 0.1f, 179.9f);
        }
	}

    private void Rotate(float deltaDegree) {

        rotateDegree += deltaDegree * RotateSpeed;

        SetCameraTransform();
    }

    private void Move(Vector2 deltaVec) {
        moveHorizontal += deltaVec.x * MoveSpeed;
        moveVertical += deltaVec.y * MoveSpeed;
        
        SetCameraTransform();
    }
    
    private void SetCameraTransform() {
        Vector3 cameraPosition =  camera.transform.position;
        Vector3 cameraRotation = camera.transform.eulerAngles;

        cameraRotation.y = defaultRotationY - rotateDegree;

        float rotateRad = rotateDegree * Mathf.Deg2Rad;
        float moveDegree = (defaultRotationY + rotateDegree + 180);
        float moveRad = moveDegree * Mathf.Deg2Rad;
        
        cameraPosition.x = defaultPositionX * Mathf.Cos(rotateRad) - defaultPositionZ * Mathf.Sin(rotateRad);
        cameraPosition.z = defaultPositionX * Mathf.Sin(rotateRad) + defaultPositionZ * Mathf.Cos(rotateRad);
        cameraPosition.x += - Mathf.Sin(moveRad) * moveHorizontal;
        cameraPosition.z += + Mathf.Cos(moveRad) * moveHorizontal;

        cameraPosition.y = defaultPositionY + moveVertical;

        camera.transform.position = cameraPosition;
        camera.transform.eulerAngles = cameraRotation;

        if (OnCameraRotate != null) {
            OnCameraRotate(moveDegree);
        }
    }

}
