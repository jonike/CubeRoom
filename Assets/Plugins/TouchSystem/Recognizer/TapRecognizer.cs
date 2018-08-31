using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TapRecognizer : AbstractRecognizer
{
    public event Action<TapRecognizer> gestureRecognizedEvent;

    public Vector2 startPosition;
    public Vector2 position;
    public Vector2 lastPosition
    {
        get { return position - deltaPosition; }
    }
    public Vector2 deltaPosition;

    private float maxMoveDistance;

    public TapRecognizer(float maxTapMoveDistance = 1f)
    {
        maxMoveDistance = maxTapMoveDistance;
    }

    #region event


    internal void emitRecognizedEvent()
    {
        if (gestureRecognizedEvent != null)
            gestureRecognizedEvent(this);
    }

    #endregion


    internal override bool canTrigger(List<TSTouch> touches)
    {
        if (touches.Count == 1)
        {
            TSTouch touch = touches[0];
            if (touch.phase == TouchPhase.Ended && touch.deltaPosition.magnitude < maxMoveDistance)
            {
                return true;
            }
        }
        return false;
    }

    internal override void touchesBegan(List<TSTouch> touches)
    { }


    internal override void touchesMoved(List<TSTouch> touches)
    { }


    internal override void touchesEnded(List<TSTouch> touches)
    {
        if (touches.Count == 1)
        {
            TSTouch touch = touches[0];
            tracingTouches.Add(touch);
            state = RecognizerState.Recognized;
            emitRecognizedEvent();
        }

        state = RecognizerState.Ended;
        end();
        reset();

    }
}
