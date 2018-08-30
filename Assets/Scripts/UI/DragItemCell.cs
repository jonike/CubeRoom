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
    private Text nameText;
    private Image image;

    void Start()
    {
        parentScroll = GetComponentInParent<ScrollRect>();
        cellView = GetComponentInParent<DragItemCellView>();

    }

    public override void Init()
    {
        nameText = transform.Find("name").GetComponent<Text>();
        image = transform.Find("image").GetComponent<Image>();
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
        nameText.text = item.name;
        string path = string.Format("Images/Items/{0}_512", item.name);
        Sprite sprite = Resources.Load<Sprite>(path) as Sprite;
        if (sprite)
            image.sprite = sprite;
        else
            image.color = Color.clear;
    }
}
