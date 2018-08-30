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
    private ClickBuildCellView buildCellView;
    private Transform setView;
    private Transform editView;

    private Button resetButton;
    private Button placeButton;
    private Button deleteButton;
    private RotateButton rotateButton;

    private Button wallButton;
    private Button floorButton;
    private Button itemButton;

    private ItemPO[] itemCellViewData;

    private BuildPO[] buildCellViewData;

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
    public Action<BuildPO> OnBuildClick
    {
        set
        {
            buildCellView.OnBuildClick += (index) =>
            {
                value(buildCellViewData[index]);
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

    public Action OnTypeClick
    {
        set
        {
            wallButton.onClick.AddListener(() =>
            {
                value();
                SetMode(StudioMode.SelectWall, true);
            });
            floorButton.onClick.AddListener(() =>
            {
                value();
                SetMode(StudioMode.SelectFloor, true);
            });
            itemButton.onClick.AddListener(() =>
            {
                value();
                SetMode(StudioMode.SelectItem, true);
            });
        }
    }

    #endregion
    public void Init()
    {
        typeView = transform.Find("TypeView");

        itemCellView = transform.Find("DragItemScrollView").GetComponent<DragItemCellView>();
        itemCellView.DataSource = ItemCellViewDataSource;
        itemCellView.Init();

        buildCellView = transform.Find("ClickBuildScrollView").GetComponent<ClickBuildCellView>();
        buildCellView.DataSource = BuildCellViewDataSource;
        buildCellView.Init();

        setView = transform.Find("SetView");
        resetButton = setView.Find("ResetButton").GetComponent<Button>();
        editView = transform.Find("EditView");
        placeButton = editView.Find("PlaceButton").GetComponent<Button>();
        deleteButton = editView.Find("DeleteButton").GetComponent<Button>();
        rotateButton = editView.Find("RotateButton").GetComponent<RotateButton>();

        wallButton = typeView.Find("WallButton").GetComponent<Button>();
        floorButton = typeView.Find("FloorButton").GetComponent<Button>();
        itemButton = typeView.Find("ItemButton").GetComponent<Button>();

    }

    public ItemPO[] ItemCellViewDataSource()
    {
        return itemCellViewData;
    }

    public BuildPO[] BuildCellViewDataSource()
    {
        return buildCellViewData;
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
        buildCellView.gameObject.SetActive(mode == StudioMode.SelectWall || mode == StudioMode.SelectFloor);
        editView.gameObject.SetActive(mode == StudioMode.EditItem);

        if (!refreshData) return;

        switch (mode)
        {
            case StudioMode.SelectItem:
                itemCellViewData = ItemData.GetAll();
                itemCellView.Refresh();
                break;
            case StudioMode.SelectWall:
                buildCellViewData = WallData.GetAll();
                buildCellView.Refresh();
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
