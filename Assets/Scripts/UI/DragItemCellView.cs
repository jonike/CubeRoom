using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sorumi.UI;

[RequireComponent(typeof(CellView))]
public class DragItemCellView : MonoBehaviour
{
    private CellView cellView;
    private GameObject curItem;
    public event Action<int, Vector2> OnItemBeginDrag;

    public delegate ItemPO[] DataSourceDelegate();
    public DataSourceDelegate DataSource;

    // public Action<Vector2> OnItemDrag;
    // public Action<Vector2> OnItemEndDrag;
    public void Init()
    {
        cellView = GetComponent<CellView>();
        cellView.CountOfCell = CountOfCell;
        cellView.CellAtIndex = CellAtIndex;

        cellView.Init();
    }

    public void Refresh()
    {
        cellView.Refresh();
    }

    public void CellAtIndex(Cell cell, int index)
    {
        if (DataSource != null) {
            DragItemCell dragItemCell = cell.GetComponent<DragItemCell>();
            dragItemCell.SetItem(DataSource()[index]);
        }
    }
    public int CountOfCell()
    {
        if (DataSource != null)
            return DataSource().Length;
        return 0;
    }

    public void OnBeginDrag(int index, Vector2 position)
    {
        if (OnItemBeginDrag != null)
            OnItemBeginDrag(index, position);
    }
}
