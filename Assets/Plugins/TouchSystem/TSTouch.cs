using UnityEngine;
using System.Collections;

public class TSTouch
{
    public readonly int fingerId;
    public Vector2 position;
    public Vector2 startPosition;
    public Vector2 lastPosition
    {
        get { return position - deltaPosition; }
    }
    public Vector2 deltaPosition;
    public float deltaTime;

    public TouchPhase phase = TouchPhase.Ended;

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER || UNITY_WEBGL
    private Vector2? lastMousePosition;
    private double lastClickTime;
#endif


    public TSTouch(int id)
    {
        fingerId = id;
    }

    public TSTouch updateByTouch(Touch touch)
    {
        position = touch.position;
        deltaPosition = touch.deltaPosition;
        deltaTime = touch.deltaTime;

        // tapCount = touch.tapCount;

        if (touch.phase == TouchPhase.Began)
        {
            startPosition = position;
        }
        if (touch.phase == TouchPhase.Canceled)
            phase = TouchPhase.Ended;
        else
            phase = touch.phase;

        return this;
    }

    public TSTouch updateByMouse()
    {
        // do we have some input to work with?
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))
        {
            var phase = TouchPhase.Moved;

            // guard against down and up being called in the same frame
            if (Input.GetMouseButtonDown(0) && Input.GetMouseButtonUp(0))
                phase = TouchPhase.Canceled;
            else if (Input.GetMouseButtonUp(0))
                phase = TouchPhase.Ended;
            else if (Input.GetMouseButtonDown(0))
                phase = TouchPhase.Began;

            Vector2 position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            this.populateWithPosition(position, phase);
        }

        return this;
    }

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER || UNITY_WEBGL
    public TSTouch populateWithPosition(Vector2 position, TouchPhase touchPhase)
    {
        double clickTime = Time.time;
        if (lastMousePosition.HasValue)
        {
            deltaPosition = position - lastMousePosition.Value;
            deltaTime = (float)(clickTime - lastClickTime);
        }
        else
        {
            deltaPosition = new Vector2(0, 0);
            deltaTime = 0.0f;
        }
        lastClickTime = clickTime;

        switch (touchPhase)
        {
            case TouchPhase.Began:
                phase = TouchPhase.Began;
                lastMousePosition = position;
                startPosition = position;
                break;
            case TouchPhase.Stationary:
            case TouchPhase.Moved:
                if (deltaPosition.sqrMagnitude == 0)
                    phase = TouchPhase.Stationary;
                else
                    phase = TouchPhase.Moved;

                lastMousePosition = position;
                break;
            case TouchPhase.Ended:
                phase = TouchPhase.Ended;
                lastMousePosition = null;
                break;
        }

        this.position = position;

        return this;
    }

#endif

    public override string ToString()
    {
        return string.Format("[TSTouch] fingerId: {0}, phase: {1}, position: {2}, startPosition: {3}", fingerId, phase, position, startPosition);
    }
}