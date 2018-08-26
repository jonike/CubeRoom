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
    public event Action<Vector2> OnItemBeginDrag;
    // public Action<Vector2> OnItemDrag;
    // public Action<Vector2> OnItemEndDrag;
    public void Init()
    {
        cellView = GetComponent<CellView>();
        cellView.CountOfCell = CountOfCell;
        cellView.CellAtIndex = CellAtIndex;

        cellView.Init();
    }

    public void CellAtIndex(GameObject gameObject, int index)
    {
        gameObject.transform.Find("Text").GetComponent<Text>().text = ItemData.GetNameByRow(index + 1);
    }
    public int CountOfCell()
    {
        return ItemData.Count();
    }

    public void OnBeginDrag(Vector2 position)
    {
        if (OnItemBeginDrag != null)
            OnItemBeginDrag(position);
    }
}
