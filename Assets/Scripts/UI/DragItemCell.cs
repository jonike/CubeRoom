using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sorumi.UI;

public class DragItemCell : Cell, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private ScrollRect parentScroll;
    private DragItemCellView cellView;

    private bool scroll;
    private Text text;

    void Start()
    {
        parentScroll = GetComponentInParent<ScrollRect>();
        cellView = GetComponentInParent<DragItemCellView>();

    }

    public override void Init()
    {
        text = transform.Find("Text").GetComponent<Text>();
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
            cellView.OnBeginDrag(index, eventData.position);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (scroll)
        {
            parentScroll.OnDrag(eventData);
        }
        else
        {
            // cellView.OnItemDrag(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (scroll)
        {
            parentScroll.OnEndDrag(eventData);
        }
        else
        {
            // cellView.OnItemEndDrag(eventData);
        }
    }

    public void SetItem(ItemPO item)
    {
        text.text = item.name;
    }
}
