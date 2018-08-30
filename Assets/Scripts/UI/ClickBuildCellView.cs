using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sorumi.UI;

[RequireComponent(typeof(CellView))]
public class ClickBuildCellView : MonoBehaviour
{
    private CellView cellView;
    private GameObject curItem;
    public event Action<int> OnBuildClick;

    public delegate BuildPO[] DataSourceDelegate();
    public DataSourceDelegate DataSource;

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
            ClickBuildCell clickBuildCell = cell.GetComponent<ClickBuildCell>();
            clickBuildCell.SetBuild(DataSource()[index]);
        }
    }
    public int CountOfCell()
    {
        if (DataSource != null)
            return DataSource().Length;
        return 0;
    }

    public void OnClick(int index)
    {
        if (OnBuildClick != null)
            OnBuildClick(index);
    }
}
