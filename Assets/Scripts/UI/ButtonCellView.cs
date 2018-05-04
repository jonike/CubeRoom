using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sorumi.UI;

[RequireComponent(typeof(CellView))]
public class ButtonCellView : MonoBehaviour {

	// Use this for initialization
	public int count;
	private CellView cellView;

	void Start () {
		cellView = GetComponent<CellView>();
		cellView.CountOfCell = CountOfCell;
		cellView.CellAtIndex = CellAtIndex;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void CellAtIndex(GameObject gameObject, int index) {
		gameObject.transform.Find("Text").GetComponent<Text>().text = index.ToString();
	}
	int CountOfCell() {
		return count;
	}
}
