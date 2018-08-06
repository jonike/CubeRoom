using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sorumi.Util;

public class GridGroup : MonoBehaviour
{
    private const int MAX_SIZE = 5;
    private const float GRID_OFFSET = 0.002f;

    private bool active;

    // gameobject
    private Vector2Int bottomSize;
    private Vector2Int sideSize;
    private Grid[,] bottomGrids;
    private Grid[,] sideGrids;

    private Transform bottomGridsGroup;
    private Transform sideGridsGroup;

    // pool
    private GameObject poolGO;
    private ObjectPool<GameObject> gridPool;

    public void Init()
    {
        bottomGrids = new Grid[MAX_SIZE, MAX_SIZE];
        sideGrids = new Grid[MAX_SIZE, MAX_SIZE];

        bottomGridsGroup = transform.Find("BottomGrids");
        sideGridsGroup = transform.Find("SideGrids");

        gridPool = new ObjectPool<GameObject>(CreateGrid, ResetGrid);

        poolGO = new GameObject("PoolGameObject");
        poolGO.transform.position = Vector3.zero;
        poolGO.SetActive(false);

        SetActive(false);
    }

    private GameObject CreateGrid()
    {
        GameObject gridGO = Instantiate(Resources.Load("Prefabs/Grid")) as GameObject;
        Grid grid = gridGO.GetComponent<Grid>();
        grid.Init();
        grid.transform.SetParent(poolGO.transform);
        return gridGO;
    }

    private void ResetGrid(GameObject grid)
    {
        grid.transform.SetParent(poolGO.transform);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        this.active = active;

        bottomGridsGroup.position = Vector3.zero;
        bottomGridsGroup.eulerAngles = Vector3.zero;

        sideGridsGroup.position = Vector3.zero;
        sideGridsGroup.eulerAngles = Vector3.zero;
    }
    public void SetGrids(ItemObject item)
    {
        Debug.Log(item.Type);
        sideGridsGroup.gameObject.SetActive(item.Type == ItemType.Vertical);

        Vector3Int size = item.Item.Size;
        bottomSize = new Vector2Int(size.x, size.z);
        sideSize = new Vector2Int(size.x, size.y);

        float offsetX = 0.5f - size.x / 2.0f;
        float offsetZ = item.Type == ItemType.Vertical ? 0.5f : 0.5f - size.z / 2.0f;
        float offsetY = 0.5f - size.y / 2.0f;

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                Grid grid;
                if ((grid = bottomGrids[i, j]) == null)
                {
                    grid = gridPool.GetObject().GetComponent<Grid>();
                    grid.transform.SetParent(bottomGridsGroup);
                    bottomGrids[i, j] = grid;
                }
                float x = i + offsetX;
                float z = j + offsetZ;
                grid.transform.position = new Vector3(x, GRID_OFFSET, z);
            }
        }

        for (int i = size.x; i < MAX_SIZE; i++)
        {
            for (int j = size.z; j < MAX_SIZE; j++)
            {
                Grid grid;
                if ((grid = bottomGrids[i, j]) != null)
                    gridPool.PutObject(grid.gameObject);
            }
        }

        if (item.Type == ItemType.Vertical)
        {
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    Grid grid;
                    if ((grid = sideGrids[i, j]) == null)
                    {
                        grid = gridPool.GetObject().GetComponent<Grid>();
                        grid.transform.SetParent(sideGridsGroup);
                        sideGrids[i, j] = grid;
                    }
                    float x = i + offsetX;
                    float y = j + offsetY;
                    grid.transform.position = new Vector3(x, y, GRID_OFFSET);
                    grid.transform.eulerAngles = new Vector3(90, 0, 0);

                }
            }

            for (int i = size.x; i < MAX_SIZE; i++)
            {
                for (int j = size.y; j < MAX_SIZE; j++)
                {
                    Grid grid;
                    if ((grid = sideGrids[i, j]) != null)
                        gridPool.PutObject(grid.gameObject);
                }
            }
        }

        Vector3 position = item.gameObject.transform.position;

    }

    public void SetTransform(ItemObject item)
    {
        Vector3 position = item.transform.position;
        Vector3 eulerAngles = item.transform.eulerAngles;
        bottomGridsGroup.position = new Vector3(position.x, 0, position.z);
        bottomGridsGroup.eulerAngles = new Vector3(0, eulerAngles.y, 0); ;

        sideGridsGroup.position = position;
        sideGridsGroup.eulerAngles = new Vector3(0, eulerAngles.y, 0);
    }

    public void SetBottomGridsType(bool[,] gridTypes)
    {
        setGridsType(bottomGrids, bottomSize, gridTypes);
    }

    public void SetSideGridsType(bool[,] gridTypes)
    {
        setGridsType(sideGrids, sideSize, gridTypes);
    }

    private void setGridsType(Grid[,] grids, Vector2Int size, bool[,] gridTypes)
    {
        int sizeX = gridTypes.GetLength(0);
        int sizeY = gridTypes.GetLength(1);

        if (sizeX > size.x || sizeY > size.y)
        {
            Debug.LogWarning("SetGridsType: GridTypes size error");
        }

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                grids[i, j].SetType(gridTypes[i, j]);
            }
        }
    }
}
