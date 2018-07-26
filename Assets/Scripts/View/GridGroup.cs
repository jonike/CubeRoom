using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sorumi.Util;

public class GridGroup : MonoBehaviour
{
    public const int MAX_SIZE = 5;
    public const float GRID_OFFSET = 0.002f;
    // material
    public Material correctMaterial;
    public Material errorMaterial;

    // gameobject
    public GameObject[,] bottomGrids;
    public GameObject[,] sideGrids;

    public Transform bottomGridsGroup;
    public Transform sideGridsGroup;

    // pool
    private GameObject poolGO;
    private ObjectPool<GameObject> gridPool;

    public void Init()
    {
        bottomGrids = new GameObject[MAX_SIZE, MAX_SIZE];
        sideGrids = new GameObject[MAX_SIZE, MAX_SIZE];

        bottomGridsGroup = transform.Find("BottomGrids");
        sideGridsGroup = transform.Find("SideGrids");

        gridPool = new ObjectPool<GameObject>(CreateGrid, ResetGrid);

        poolGO = new GameObject("PoolGameObject");
        poolGO.transform.position = Vector3.zero;
        poolGO.SetActive(false);
    }

    private GameObject CreateGrid()
    {
        GameObject grid = Instantiate(Resources.Load("Prefabs/ItemGrid")) as GameObject;
        grid.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1, 1);
        grid.transform.SetParent(poolGO.transform);
        return grid;
    }

    private void ResetGrid(GameObject grid)
    {
        grid.transform.SetParent(poolGO.transform);
    }

    public void SetGrids(ItemObject item)
    {
        sideGridsGroup.gameObject.SetActive(item.Type == ItemType.Vertical);

        Vector3Int size = item.Item.Size;
        float offsetX = 0.5f - size.x / 2.0f;
        float offsetZ = item.Type == ItemType.Vertical ? 0.5f : 0.5f - size.z / 2.0f;
        float offsetY = 0.5f - size.y / 2.0f;

        for (int i = 0; i < size.z; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                if (bottomGrids[i, j] == null)
                {
                    GameObject grid = gridPool.GetObject();
                    grid.transform.SetParent(bottomGridsGroup);
                    float x = j + offsetX;
                    float z = i + offsetZ;
                    grid.transform.position = new Vector3(x, GRID_OFFSET, z);
                    bottomGrids[i, j] = grid;
                }
            }
        }

        if (item.Type == ItemType.Vertical)
        {
            for (int i = 0; i < size.y; i++)
            {
                for (int j = 0; j < size.x; j++)
                {
                    if (sideGrids[i, j] == null)
                    {
                        GameObject grid = gridPool.GetObject();
                        grid.transform.SetParent(sideGridsGroup);
                        float x = j + offsetX;
                        float y = i + offsetY;
                        grid.transform.position = new Vector3(x, y, GRID_OFFSET);
                        grid.transform.eulerAngles = new Vector3(90, 0, 0);
                        sideGrids[i, j] = grid;
                    }
                }
            }
        }

        Vector3 position = item.gameObject.transform.position;

    }

    public void SetTransform(ItemObject item)
    {
		Vector3 position = item.transform.position;
		Vector3 eulerAngles = item.transform.eulerAngles;
        bottomGridsGroup.position = new Vector3(position.x, GRID_OFFSET, position.z);
        bottomGridsGroup.eulerAngles = new Vector3(0, eulerAngles.y, 0);;
		
		sideGridsGroup.position = position;
		sideGridsGroup.eulerAngles = new Vector3(0, eulerAngles.y, 0);
    }
}
