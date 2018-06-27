using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public ScrollRect parentScroll;

    private bool scroll;

    void Start () 
    {
        parentScroll = GetComponentInParent<ScrollRect> ();
    }
 	public void OnBeginDrag(PointerEventData eventData)
     {
	// 	Debug.Log("Press position + " + eventData.pressPosition);
	// 	Debug.Log("End position + " + eventData.position);

		float diffX = Mathf.Abs(eventData.position.x - eventData.pressPosition.x);
		float diffY = Mathf.Abs(eventData.position.y - eventData.pressPosition.y);
		if (diffX < diffY){
            Debug.Log(1);
			scroll = true;
            parentScroll.OnBeginDrag(eventData);
		} else {
            scroll = false;
        }
     }
 
     public void OnDrag(PointerEventData eventData)
     {
        if (scroll) {
            parentScroll.OnDrag(eventData);
        }
     }
 
     public void OnEndDrag(PointerEventData eventData)
     {
		 if (scroll) {
            parentScroll.OnEndDrag(eventData);
        }
     }
}
