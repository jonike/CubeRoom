using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sorumi.Util;

public class GridObject : MonoBehaviour {

	public Material correctMaterial;
	public Material errorMaterial;
	private GameObject poolGO;
	private ObjectPool<GameObject> gridPool;
		
	private GameObject[,] grids = new GameObject[5,5];

	public void Init() {
		transform.position = new Vector3(0, 0.002f, 0);
		gridPool = new ObjectPool<GameObject>(CreateGrid, ResetGrid);
			
		poolGO = new GameObject("PoolGameObject");
		poolGO.transform.position = Vector3.zero;
		poolGO.SetActive(false);
	}


	private GameObject CreateGrid() {
		GameObject grid = Instantiate(Resources.Load("Prefabs/ItemGrid")) as GameObject;
		grid.transform.SetParent(poolGO.transform);
		return grid;
	}

	private void ResetGrid(GameObject grid) {
		grid.transform.SetParent(poolGO.transform);
	}

	// public void SetType(bool type) {
	// 	gameObject.GetComponent<Renderer>().sharedMaterial = type ? correctMaterial : errorMaterial;
	// }

	public void SetActive() {
		gameObject.SetActive(true);
	
	}
	
	public void SetInactive() {
		gameObject.SetActive(false);
	}

	public void SetGridsSize(Vector2Int size) {
		// delete
		for (int i = size.y; i < 5; i++) {
			for (int j = 0; j < 5; j++) {
				if (grids[i,j]) {
					gridPool.PutObject(grids[i,j]);
					grids[i,j] = null;
				}
			}
		}

		for (int i = size.x; i < 5; i++) {
			for (int j = 0; j < 5; j++) {
			if (grids[j,i]) {
					gridPool.PutObject(grids[j,i]);
					grids[j,i] = null;
				}
			}
		}

		Vector3 groupPosition = transform.position;

		for (int i = 0; i < size.y; i++)
		{
			for (int j = 0; j < size.x; j++)
			{
				if (grids[i,j] == null) {
					GameObject grid = gridPool.GetObject();
					grid.transform.SetParent(transform);
					float x = groupPosition.x + j + 0.5f;
					float z = groupPosition.z + i + 0.5f;
					grid.transform.position = new Vector3(x, 0.002f, z);
					grid.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1, 1);

					grids[i,j] = grid;
				}
			}
		}
	}
}
