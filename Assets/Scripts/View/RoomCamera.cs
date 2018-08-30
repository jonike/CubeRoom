using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sorumi.Util;
public class RoomCamera : MonoBehaviour
{
    private float defaultRotationY = -135f;

    private float defaultRotationX = 30f;
    private float defaultPositionX = 32f;
    private float defaultPositionZ = 32f;

    private float defaultPositionXZ = Mathf.Sqrt(2) * 32f;
    private float defaultPositionY = 6f;

    private float defaultOrthographic = 10.0f;
    private float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
    private float orthoZoomSpeed = 0.1f;        // The rate of change of the orthographic size in orthographic mode.

    private float moveSpeed = 0.1f;
    private float rotateSpeed = 1f;
    private float orthographicMin = 4.0f;
    private float orthographicMax = 12.0f;

    private Camera camera;

    public delegate void FloatDelegate(float value);
    public FloatDelegate OnCameraRotate;

    // record
    private float rotateHorizontal = 0;
    private float rotateVertical = 0;
    private float moveVertical = 0;
    private float moveHorizontal = 0;

    // animation
    private bool isAnimated = false;
    private float count = 60;

    private float iRotateHorizontal = 0;
    private float iRotateVertical = 0;
    private float iMoveVertical = 0;
    private float iMoveHorizontal = 0;

    private float iOrthographic = 0;

#if UNITY_EDITOR && CR_DEBUG_CAMERA
        void OnGUI()
        {
            if (GUI.RepeatButton(new Rect(0, 0, 50, 20), "放大"))
            {
                Zoom(1f);
            }

            if (GUI.RepeatButton(new Rect(60, 0, 50, 20), "缩小"))
            {
                Zoom(-1f);
            }
            if (GUI.RepeatButton(new Rect(30, 30, 50, 20), "上转"))
            {
                Rotate(new Vector2(0, 1));
            }

            if (GUI.RepeatButton(new Rect(30, 90, 50, 20), "下转"))
            {
                Rotate(new Vector2(0, -1));
            }

            if (GUI.RepeatButton(new Rect(0, 60, 50, 20), "左转"))
            {
                Rotate(new Vector2(-1, 0));
            }

            if (GUI.RepeatButton(new Rect(60, 60, 50, 20), "右转"))
            {
                Rotate(new Vector2(1, 0));
            }

            if (GUI.RepeatButton(new Rect(30, 120, 50, 20), "↑"))
            {
                Move(new Vector2(0, 1));
            }

            if (GUI.RepeatButton(new Rect(30, 180, 50, 20), "↓"))
            {
                Move(new Vector2(0, -1));
            }

            if (GUI.RepeatButton(new Rect(0, 150, 50, 20), "←"))
            {
                Move(new Vector2(-1, 0));
            }

            if (GUI.RepeatButton(new Rect(60, 150, 50, 20), "→"))
            {
                Move(new Vector2(1, 0));
            }

        }
#endif

    void Update()
    {
        if (isAnimated)
            Animate();
    }

    public void TriggerAnimation()
    {
        isAnimated = true;
        count = 30;
        iRotateHorizontal = rotateHorizontal / count;
        iRotateVertical = rotateVertical / count;
        iMoveHorizontal = moveHorizontal / count;
        iMoveVertical = moveVertical / count;
        iOrthographic = (camera.orthographicSize - defaultOrthographic) / count;
    }
    private void Animate()
    {
        camera.orthographicSize -= iOrthographic;
        rotateHorizontal -= iRotateHorizontal;
        rotateVertical -= iRotateVertical;
        moveHorizontal -= iMoveHorizontal;
        moveVertical -= iMoveVertical;
        SetCameraTransform();
        count--;

        if (count == 0)
            isAnimated = false;
    }

    public void Init()
    {
        camera = GetComponent<Camera>();
    }

    public void Zoom(float deltaMagnitude)
    {
        if (isAnimated)
            return;
        if (camera.orthographic)
        {
            camera.orthographicSize -= deltaMagnitude * orthoZoomSpeed;
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, orthographicMin, orthographicMax);
        }
        else
        {
            camera.fieldOfView += deltaMagnitude * perspectiveZoomSpeed;
            camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 0.1f, 179.9f);
        }
    }

    public void Rotate(Vector2 deltaDegree)
    {
        if (isAnimated)
            return;

        rotateHorizontal += deltaDegree.x * rotateSpeed;
        rotateVertical += deltaDegree.y * rotateSpeed;

        rotateHorizontal = Maths.mod(rotateHorizontal, 360);
        if (rotateHorizontal > 180)
            rotateHorizontal -= 360;

        rotateVertical = Mathf.Clamp(rotateVertical, -30f, 50f);

        SetCameraTransform();
    }

    public void Move(Vector2 deltaVec)
    {
        if (isAnimated)
            return;

        moveHorizontal += deltaVec.x * moveSpeed;
        moveVertical += deltaVec.y * moveSpeed;

        SetCameraTransform();
    }

    public void SetCameraTransform()
    {
        Vector3 cameraPosition = camera.transform.position;
        Vector3 cameraRotation = camera.transform.eulerAngles;

        cameraRotation.x = defaultRotationX + rotateVertical;
        cameraRotation.y = defaultRotationY - rotateHorizontal;

        float rotateHorizontalRad = rotateHorizontal * Mathf.Deg2Rad;
        float rotateVerticalRad = (defaultRotationX + rotateVertical) * Mathf.Deg2Rad;
        float moveDegree = (defaultRotationY + rotateHorizontal + 180);
        float moveRad = moveDegree * Mathf.Deg2Rad;

        float d = defaultPositionXZ * (1 - Mathf.Cos(rotateVerticalRad));  //rotate vertical
        float dRad = (defaultRotationY + 180 - rotateHorizontal) * Mathf.Deg2Rad;

        cameraPosition.x = defaultPositionX * Mathf.Cos(rotateHorizontalRad) - defaultPositionZ * Mathf.Sin(rotateHorizontalRad) - d * Mathf.Sin(dRad);
        cameraPosition.z = defaultPositionX * Mathf.Sin(rotateHorizontalRad) + defaultPositionZ * Mathf.Cos(rotateHorizontalRad) - d * Mathf.Cos(dRad);
        cameraPosition.x += Mathf.Sin(moveRad) * moveHorizontal;
        cameraPosition.z += -Mathf.Cos(moveRad) * moveHorizontal;

        cameraPosition.y = defaultPositionXZ * Mathf.Sin(rotateVerticalRad) + defaultPositionY;
        cameraPosition.y -= moveVertical;

        camera.transform.position = cameraPosition;
        camera.transform.eulerAngles = cameraRotation;

        if (OnCameraRotate != null)
        {
            OnCameraRotate(moveDegree);
        }
    }

}
