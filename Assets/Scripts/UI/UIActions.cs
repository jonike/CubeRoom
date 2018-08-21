using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIActions
{
    public delegate void Vector2Action(Vector2 position);

    public delegate void PointerEventAction(PointerEventData eventData);
}
