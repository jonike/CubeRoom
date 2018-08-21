using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragItemCell : MonoBehaviour, IPointerDownHandler {

    private ScrollRect parentScroll;
    private DragItemCellView cellView;

    private bool scroll;


    void Start () 
    {
        parentScroll = GetComponentInParent<ScrollRect>();
        cellView = GetComponentInParent<DragItemCellView>();
    }
    
    public void OnPointerDown (PointerEventData eventData) 
	{
        cellView.OnItemBeginDrag(eventData.position);
	}


 	// public void OnBeginDrag(PointerEventData eventData)
    //  {
	// // 	Debug.Log("Press position + " + eventData.pressPosition);
	// // 	Debug.Log("End position + " + eventData.position);

	// 	float diffX = Mathf.Abs(eventData.position.x - eventData.pressPosition.x);
	// 	float diffY = Mathf.Abs(eventData.position.y - eventData.pressPosition.y);
	// 	if (diffX < diffY){
	// 		scroll = true;
    //         parentScroll.OnBeginDrag(eventData);
	// 	} else {
    //         scroll = false;
    //         cellView.OnItemBeginDrag(eventData);
    //     }
    //  }
 
    //  public void OnDrag(PointerEventData eventData)
    //  {
    //     if (scroll) {
    //         parentScroll.OnDrag(eventData);
    //     } else {
    //         cellView.OnItemDrag(eventData);
    //     }
    //  }
 
    //  public void OnEndDrag(PointerEventData eventData)
    //  {
	// 	if (scroll) {
    //         parentScroll.OnEndDrag(eventData);
    //     } else {
    //         cellView.OnItemEndDrag(eventData);
    //     }
    //  }
}
