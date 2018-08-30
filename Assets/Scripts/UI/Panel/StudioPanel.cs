using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sorumi.Util;

public enum StudioMode
{
    Type,
    SelectWall,
    SelectFloor,
    SelectItem,
    EditItem,
}
public class StudioPanel : MonoBehaviour
{

    private StudioMode mode;
    private Transform typeView;
    private DragItemCellView itemCellView;
    private Transform setView;
    private Transform editView;

    private Button resetButton;

    private Button placeButton;
    private Button deleteButton;
    private RotateButton rotateButton;

    private ItemPO[] itemCellViewData;

    #region delegate
    public Action<ItemPO, Vector2> OnItemBeginDrag
    {
        set
        {
            itemCellView.OnItemBeginDrag += (index, position) =>
            {
                value(itemCellViewData[index], position);
            };
        }
    }

    public Action OnResetClick
    {
        set
        {
            resetButton.onClick.AddListener(() =>
            {
                value();
            });
        }
    }
    public Action OnPlaceClick
    {
        set
        {
            placeButton.onClick.AddListener(() =>
            {
                value();
            });
        }
    }

    public Action OnDeleteClick
    {
        set
        {
            deleteButton.onClick.AddListener(() =>
            {
                value();
            });
        }
    }

    public Action<float> OnRotateChange
    {
        set
        {
            rotateButton.OnChange += value;
        }
    }

    #endregion
    public void Init()
    {
        typeView = transform.Find("TypeView");
        itemCellView = transform.Find("DragItemScrollView").GetComponent<DragItemCellView>();
        itemCellView.DataSource = ItemCellViewDataSource;
        itemCellView.Init();

        setView = transform.Find("SetView");
        resetButton = setView.Find("ResetButton").GetComponent<Button>();
        editView = transform.Find("EditView");
        placeButton = editView.Find("PlaceButton").GetComponent<Button>();
        deleteButton = editView.Find("DeleteButton").GetComponent<Button>();
        rotateButton = editView.Find("RotateButton").GetComponent<RotateButton>();

        Button wallButton = typeView.Find("WallButton").GetComponent<Button>();
        wallButton.onClick.AddListener(() =>
        {
            SetMode(StudioMode.SelectWall, true);
        });
        Button floorButton = typeView.Find("FloorButton").GetComponent<Button>();
        floorButton.onClick.AddListener(() =>
        {
            SetMode(StudioMode.SelectFloor, true);
        });
        Button itemButton = typeView.Find("ItemButton").GetComponent<Button>();
        itemButton.onClick.AddListener(() =>
        {
            SetMode(StudioMode.SelectItem, true);
        });
    }

    public ItemPO[] ItemCellViewDataSource()
    {
        return itemCellViewData;
    }

    #region Controller API

    public StudioMode GetMode()
    {
        return mode;
    }
    public void SetMode(StudioMode mode, bool refreshData = false)
    {
        this.mode = mode;
        typeView.gameObject.SetActive(mode == StudioMode.Type);
        itemCellView.gameObject.SetActive(mode == StudioMode.SelectItem);
        editView.gameObject.SetActive(mode == StudioMode.EditItem);

        if (!refreshData) return;

        switch (mode)
        {
            case StudioMode.SelectItem:
                itemCellViewData = ItemData.GetAll();
                itemCellView.Refresh();
                break;
        }

    }

    public void Back()
    {
        switch (mode)
        {
            case StudioMode.EditItem:
                SetMode(StudioMode.SelectItem);
                break;
            case StudioMode.SelectItem:
            case StudioMode.SelectWall:
            case StudioMode.SelectFloor:
                SetMode(StudioMode.Type);
                break;
        }
    }
    public void SetRotateButtonValue(float degree)
    {
        rotateButton.SetValue(degree);
    }
    public void SetRotateButtonRotation(float degree)
    {
        rotateButton.SetRotation(degree);
    }

    public void SetPlaceButtonAbled(bool isAbled)
    {
        placeButton.interactable = isAbled;
    }

    public void SetResetButtonActive(bool isActive)
    {
        resetButton.gameObject.SetActive(isActive);
    }


    #endregion
}
