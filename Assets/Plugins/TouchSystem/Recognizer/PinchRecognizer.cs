using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PinchRecognizer : AbstractRecognizer
{
    public event Action<PinchRecognizer> gestureBeginEvent;
    public event Action<PinchRecognizer> gestureRecognizedEvent;
    public event Action<PinchRecognizer> gestureEndEvent;

    public float startDistance;
    public float distance;
    public float deltaDistance;

    private float minMoveDistance;


    public PinchRecognizer(float minPanDistance = 0.5f)
    {

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
        if (touches.Count == 2)
        {
            bool result = true;
            for (int i = 0; i < 2; i++)
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
                return isOpposite(touches);

        }
        return false;
    }

    internal override void touchesBegan(List<TSTouch> touches)
    {
        if (touches.Count != 2)
        {
            gestureEnd();
        }
    }


    internal override void touchesMoved(List<TSTouch> touches)
    {
        if (touches.Count == 2)
        {
            // if (!isOpposite(touches))
            // {
            //     gestureEnd();
            // }
            if (state == RecognizerState.Possible)
            {
                state = RecognizerState.Began;

                for (int i = 0; i < 2; i++)
                {
                    TSTouch touch = touches[i];
                    tracingTouches.Add(touch);
                }

                TSTouch touch0 = tracingTouches[0];
                TSTouch touch1 = tracingTouches[1];

                startDistance = vector2Distance(touch0, touch1);
                distance = startDistance;
                deltaDistance = 0;

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
        TSTouch touch0 = tracingTouches[0];
        TSTouch touch1 = tracingTouches[1];

        float currentDistance = vector2Distance(touch0, touch1);
        deltaDistance = currentDistance - distance;
        distance = currentDistance;

        emitRecognizedEvent();
    }
    private void gestureEnd()
    {

        if (state == RecognizerState.Recognized || state == RecognizerState.Began)
        {
            state = RecognizerState.Ended;
            TSTouch touch0 = tracingTouches[0];
            TSTouch touch1 = tracingTouches[1];

            float currentDistance = vector2Distance(touch0, touch1);
            deltaDistance = currentDistance - distance;
            distance = currentDistance;

            emitEndEvent();

            end();
            reset();
        }
    }

    public override string ToString()
    {
        return string.Format("[{0}] state: {1}, deltaDistance: {2}", this.GetType(), state, deltaDistance);
    }

    #region Fuction

    private bool isOpposite(List<TSTouch> touches)
    {
        if (touches.Count != 2)
            return false;

        Vector2 delta0 = touches[0].position - touches[0].startPosition;
        Vector2 delta1 = touches[1].position - touches[1].startPosition;
        bool x0 = delta0.x > 0;
        bool y0 = delta0.y > 0;
        bool x1 = delta1.x > 0;
        bool y1 = delta1.y > 0;

        return x0 ^ x1 && y0 ^ y1;
    }

    private float vector2Distance(TSTouch t1, TSTouch t2)
    {
        return Vector2.Distance(t1.position, t2.position);
    }

    #endregion
}
