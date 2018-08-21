using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sorumi.UI;

[RequireComponent(typeof(CellView))]
public class DragItemCellView : MonoBehaviour {

    // Use this for initialization
    public int count;
    private CellView cellView;
	private GameObject curItem;


    public UIActions.Vector2Action OnItemBeginDrag;
	// public UIActions.PointerEventAction OnItemDrag;
	// public UIActions.PointerEventAction OnItemEndDrag;
    void Start()
    {
        cellView = GetComponent<CellView>();
        cellView.CountOfCell = CountOfCell;
        cellView.CellAtIndex = CellAtIndex;

        cellView.Init();
    }

    public void CellAtIndex(GameObject gameObject, int index)
    {
        gameObject.transform.Find("Text").GetComponent<Text>().text = index.ToString();
    }
    public int CountOfCell()
    {
        return count;
    }
}
