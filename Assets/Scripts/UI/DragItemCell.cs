using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragItemCell : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private ScrollRect parentScroll;
    private DragItemCellView cellView;

    private bool scroll;


    void Start()
    {
        parentScroll = GetComponentInParent<ScrollRect>();
        cellView = GetComponentInParent<DragItemCellView>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        float diffX = Mathf.Abs(eventData.position.x - eventData.pressPosition.x);
        float diffY = Mathf.Abs(eventData.position.y - eventData.pressPosition.y);
        if (diffX < diffY)
        {
            scroll = true;
            parentScroll.OnBeginDrag(eventData);
        }
        else
        {
            scroll = false;
            cellView.OnItemBeginDrag(eventData.position);
        }
    }

     public void OnDrag(PointerEventData eventData)
     {
        if (scroll) {
            parentScroll.OnDrag(eventData);
        } else {
            // cellView.OnItemDrag(eventData);
        }
     }

     public void OnEndDrag(PointerEventData eventData)
     {
    	if (scroll) {
            parentScroll.OnEndDrag(eventData);
        } else {
            // cellView.OnItemEndDrag(eventData);
        }
     }
}
