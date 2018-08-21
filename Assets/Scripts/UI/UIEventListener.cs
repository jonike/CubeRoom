using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIEventListener : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    public event UIActions.PointerEventAction OnClick;
    public event UIActions.PointerEventAction OnMouseEnter;
    public event UIActions.PointerEventAction OnMouseExit;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClick != null)
            OnClick(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnMouseEnter != null)
            OnMouseEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnMouseExit != null)
            OnMouseExit(eventData);
    }

}