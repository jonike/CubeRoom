using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour {

	private GameObject grid;
	void Start () 
	{
		grid = transform.Find("grid").gameObject;
	}
	
	public void ShowGrid(bool show)
	{
		grid.SetActive(show);
	}
}
