using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sorumi.Util;

public class StudioPanel : MonoBehaviour
{
    private DragItemCellView itemCellView;

    private Button placeButton;
    private Button deleteButton;
    private RotateButton rotateButton;

    public UIActions.Vector2Action OnItemBeginDrag
    {
        set
        {
            itemCellView.OnItemBeginDrag = value;
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

    public UIActions.PointerEventAction OnPlaceClick
    {
        set
        {
            UIEventListener btnListener = placeButton.gameObject.AddComponent<UIEventListener>();

            btnListener.OnClick += value;
        }
    }

    public UIActions.PointerEventAction OnDeletelick
    {
        set
        {
            UIEventListener btnListener = deleteButton.gameObject.AddComponent<UIEventListener>();

            btnListener.OnClick += value;
        }
    }

    public void Init()
    {
        itemCellView = transform.Find("DragItemScrollView").GetComponent<DragItemCellView>();
        placeButton = transform.Find("PlaceButton").GetComponent<Button>();
        deleteButton = transform.Find("DeleteButton").GetComponent<Button>();
        rotateButton = transform.Find("RotateButton").GetComponent<RotateButton>();
    }

    public void SetItemCellViewActive(bool isActive)
    {
        itemCellView.gameObject.SetActive(isActive);
    }
}
