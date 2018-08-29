using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sorumi.Util;

public class RotateButton : MonoBehaviour, IDragHandler
{

    private RectTransform rectTransform;
    private RectTransform dot;

    private float value;

    public event Action<float> OnChange;
    public void Start()
    {
        rectTransform = transform.GetComponent<RectTransform>();
        dot = transform.Find("dot").GetComponent<RectTransform>();
        // Debug.Log(dot.localEulerAngles);
    }
    public void OnDrag(PointerEventData eventData)
    {

        Vector2 position = new Vector2(
            rectTransform.position.x,
            rectTransform.position.y);
        position = eventData.position - position;
        // Debug.Log(position);
        float x = position.x;
        float y = position.y;
        float degree = Mathf.Atan(Mathf.Abs(y / x)) * Mathf.Rad2Deg;
        if (x >= 0 && y <= 0)
        {
        }
        else if (x <= 0 && y <= 0)
        {
            degree = 180 - degree;
        }
        else if (x <= 0 && y >= 0)
        {
            degree = 180 + degree;
        }
        else if (x >= 0 && y >= 0)
        {
            degree = 360 - degree;
        }

        degree += rectTransform.localEulerAngles.z;
        SetValue(degree, true);
    }

    public void SetRotation(float degree)
    {
        degree = -(int)(degree / 90) * 90.0f;
        rectTransform.localEulerAngles = new Vector3(0, 0, degree);
    }
    public void SetValue(float degree, bool isTriggerChange = false)
    {
        degree = degree % 360;
        if (degree < 0)
            degree += 360;
        // Debug.Log("drag: " + degree);

        value = degree;

        if (degree >= 0 && degree < 90)
        {
            dot.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (degree >= 90 && degree < 180)
        {
            dot.localEulerAngles = new Vector3(0, 0, 270);
        }
        else if (degree >= 180 && degree < 270)
        {
            dot.localEulerAngles = new Vector3(0, 0, 180);
        }
        else if (degree >= 270 && degree < 360)
        {
            dot.localEulerAngles = new Vector3(0, 0, 90);
        }

        if (isTriggerChange && OnChange != null)
            OnChange(value);
    }
}
