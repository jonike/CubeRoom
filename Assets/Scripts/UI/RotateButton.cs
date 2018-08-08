using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RotateButton : MonoBehaviour, IDragHandler
{

    private RectTransform rectTransform;
    private RectTransform dot;
    public void Start()
    {
        rectTransform = transform.GetComponent<RectTransform>();
        dot = transform.Find("dot").GetComponent<RectTransform>();
        Debug.Log(dot.localEulerAngles);
    }
    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log(transform.position);
        Vector2 position = new Vector2(
            rectTransform.position.x + rectTransform.rect.width / 2,
            rectTransform.position.y + rectTransform.rect.height / 2);
        position = eventData.position - position;
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

        setValue(degree);
    }

    private void setValue(float degree)
    {
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
    }
}
