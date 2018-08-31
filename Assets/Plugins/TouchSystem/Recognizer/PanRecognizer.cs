using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PanRecognizer : AbstractRecognizer
{
    public event Action<PanRecognizer> gestureBeginEvent;
    public event Action<PanRecognizer> gestureRecognizedEvent;
    public event Action<PanRecognizer> gestureEndEvent;


    public Vector2 startPosition;
    public Vector2 position;
    public Vector2 lastPosition
    {
        get { return position - deltaPosition; }
    }
    public Vector2 deltaPosition;

    private int touchCounts;
    private float minMoveDistance;

    public PanRecognizer(int panTouchCounts = 1, float minPanDistance = 0.5f)
    {
        touchCounts = panTouchCounts;
        minMoveDistance = minPanDistance;
    }

    #region event

    internal void emitBeginEvent()
    {
        if (gestureBeginEvent != null)
            gestureBeginEvent(this);
    }

    internal void emitRecognizedEvent()
    {
        if (gestureRecognizedEvent != null)
            gestureRecognizedEvent(this);
    }

    internal void emitEndEvent()
    {
        if (gestureEndEvent != null)
            gestureEndEvent(this);
    }

    #endregion


    internal override bool canTrigger(List<TSTouch> touches)
    {
        if (touches.Count == touchCounts)
        {
            bool result = true;
            for (int i = 0; i < touchCounts; i++)
            {
                TSTouch touch = touches[i];
                if (touch.phase != TouchPhase.Ended && touch.deltaPosition.magnitude > minMoveDistance)
                {

                }
                else
                {
                    result = false;
                }
            }

            if (result)
                return isIdentical(touches);

        }
        return false;
    }

    internal override void touchesBegan(List<TSTouch> touches)
    {
        if (touches.Count != touchCounts)
        {
            gestureEnd();
        }
    }

    internal override void touchesMoved(List<TSTouch> touches)
    {
        if (touches.Count == touchCounts)
        {
            // if (!isIdentical(touches))
            // {
            //     gestureEnd();
            // }
            if (state == RecognizerState.Possible)
            {
                state = RecognizerState.Began;

                for (int i = 0; i < touchCounts; i++)
                {
                    TSTouch touch = touches[i];
                    tracingTouches.Add(touch);
                }

                startPosition = midStartPosition(tracingTouches);
                position = midPosition(tracingTouches);
                deltaPosition = midDeltaPosition(tracingTouches);

                emitBeginEvent();
            }
            else if (state == RecognizerState.Began)
            {
                state = RecognizerState.Recognized;

                gestureRecognized();

            }
            else if (state == RecognizerState.Recognized)
            {
                gestureRecognized();
            }
        }
    }


    internal override void touchesEnded(List<TSTouch> touches)
    {
        gestureEnd();
    }


    private void gestureRecognized()
    {
        position = midPosition(tracingTouches);
        deltaPosition = midDeltaPosition(tracingTouches);

        emitRecognizedEvent();
    }
    private void gestureEnd()
    {

        if (state == RecognizerState.Recognized || state == RecognizerState.Began)
        {
            state = RecognizerState.Ended;

            position = midPosition(tracingTouches);
            deltaPosition = midDeltaPosition(tracingTouches);

            emitEndEvent();

            end();
            reset();
        }
    }

    public override string ToString()
    {
        return string.Format("[{0}] state: {1}, position: {2}, deltaPosition: {3}", this.GetType(), state, position, deltaPosition);
    }

    #region Fuction

    private bool isIdentical(List<TSTouch> touches)
    {
        if (touches.Count == 0)
            return false;
        bool x = touches[0].deltaPosition.x > 0;
        bool y = touches[0].deltaPosition.y > 0;

        for (int i = 1; i < touches.Count; i++)
        {
            if ((touches[i].deltaPosition.x > 0) != x)
                return false;
            if ((touches[i].deltaPosition.y > 0) != y)
                return false;
        }
        return true;
    }

    private Vector2 midStartPosition(List<TSTouch> touches)
    {
        if (touches.Count == 0)
            return Vector2.zero;

        Vector2 mid = touches[0].startPosition;
        for (int i = 1; i < touches.Count; i++)
        {
            mid += touches[i].startPosition;
        }
        mid /= touches.Count;
        return mid;
    }
        private Vector2 midPosition(List<TSTouch> touches)
    {
        if (touches.Count == 0)
            return Vector2.zero;

        Vector2 mid = touches[0].position;
        for (int i = 1; i < touches.Count; i++)
        {
            mid += touches[i].position;
        }
        mid /= touches.Count;
        return mid;
    }

    private Vector2 midDeltaPosition(List<TSTouch> touches)
    {
        if (touches.Count == 0)
            return Vector2.zero;

        Vector2 mid = touches[0].deltaPosition;
        for (int i = 1; i < touches.Count; i++)
        {
            mid += touches[i].deltaPosition;
        }
        mid /= touches.Count;
        return mid;
    }

    #endregion
}
