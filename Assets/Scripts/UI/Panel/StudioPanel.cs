using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sorumi.Util;

public class StudioPanel : MonoBehaviour
{
    private DragItemCellView itemCellView;
    private Transform editView;

    private Button placeButton;
    private Button deleteButton;
    private RotateButton rotateButton;

    public Action<Vector2> OnItemBeginDrag
    {
        set
        {
            itemCellView.OnItemBeginDrag += value;
        }
    }
    // public UIActions.PointerEventAction OnItemDrag
    // {
    //     set
    //     {
    //         itemCellView.OnItemDrag = value;
    //     }
    // }
    // public UIActions.PointerEventAction OnItemEndDrag
    // {
    //     set
    //     {
    //         itemCellView.OnItemEndDrag = value;
    //     }
    // }

    public Action<PointerEventData> OnPlaceClick
    {
        set
        {
            UIEventListener btnListener = placeButton.gameObject.AddComponent<UIEventListener>();

            btnListener.OnClick += value;
        }
    }

    public Action<PointerEventData> OnDeleteClick
    {
        set
        {
            UIEventListener btnListener = deleteButton.gameObject.AddComponent<UIEventListener>();

            btnListener.OnClick += value;
        }
    }

    public Action<float> OnRotateChange
    {
        set
        {
            rotateButton.OnChange += value;
        }
    }

    public void Init()
    {
        itemCellView = transform.Find("DragItemScrollView").GetComponent<DragItemCellView>();
        editView = transform.Find("EditView");
        placeButton = editView.Find("PlaceButton").GetComponent<Button>();
        deleteButton = editView.Find("DeleteButton").GetComponent<Button>();
        rotateButton = editView.Find("RotateButton").GetComponent<RotateButton>();
    }

    public void SetItemCellViewActive(bool isActive)
    {
        itemCellView.gameObject.SetActive(isActive);
    }

    public void SetEditViewActive(bool isActive)
    {
        editView.gameObject.SetActive(isActive);
    }
    public void SetRotateButtonValue(float degree)
    {
        rotateButton.SetValue(degree);
    }
    public void SetRotateButtonRotation(float degree)
    {
        rotateButton.SetRotation(degree);
    }
}
