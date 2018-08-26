using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sorumi.UI;

[RequireComponent(typeof(CellView))]
public class ButtonCellView : MonoBehaviour
{
    private CellView cellView;
    public void Start()
    {
        cellView = GetComponent<CellView>();
        cellView.CountOfCell = CountOfCell;
        cellView.CellAtIndex = CellAtIndex;

        cellView.Init();
        cellView.Refresh();
    }

    public void CellAtIndex(Cell cell, int index)
    {
        cell.transform.Find("Text").GetComponent<Text>().text = index.ToString();
    }
    public int CountOfCell()
    {
        return 200;
    }
}