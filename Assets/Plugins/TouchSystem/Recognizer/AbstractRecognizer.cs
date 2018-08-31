using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RecognizerState
{
    Possible,
    Began,
    Recognized,
    Ended,
}
public abstract class AbstractRecognizer : IComparable<AbstractRecognizer>
{

    public uint zIndex = 0;
    public RecognizerState state = RecognizerState.Possible;
    protected List<TSTouch> tracingTouches = new List<TSTouch>();

    #region Public

    internal void recognizeTouches(List<TSTouch> touches)
    {
        for (var i = touches.Count - 1; i >= 0; i--)
        {
            TSTouch touch = touches[i];
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        touchesBegan(touches);
                        break;
                    }
                case TouchPhase.Moved:
                    {
                        touchesMoved(touches);
                        break;
                    }
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        touchesEnded(touches);
                        break;
                    }
            }
        }

    }

    internal void reset()
    {
        state = RecognizerState.Possible;
        tracingTouches.Clear();
    }

    internal void end()
    {
        TouchSystem.resetRecoginer();
    }

    internal virtual bool canTrigger(List<TSTouch> touches)
    {
        return false;
    }

    internal virtual void touchesBegan(List<TSTouch> touches)
    { }


    internal virtual void touchesMoved(List<TSTouch> touches)
    { }


    internal virtual void touchesEnded(List<TSTouch> touches)
    { }


    #endregion


    #region IComparable and ToString implementation

    public int CompareTo(AbstractRecognizer other)
    {
        return zIndex.CompareTo(other.zIndex);
    }

    public override string ToString()
    {
        String positions = "";
        for (int i = 0; i < tracingTouches.Count; i++)
        {
            positions += tracingTouches[i].position;
        }
        return string.Format("[{0}] state: {1}, location: {2}, zIndex: {3}", this.GetType(), state, positions, zIndex);
    }

    #endregion
}
