using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sorumi.Util;

public class GridGroup : MonoBehaviour
{
    private const int MAX_SIZE = 6;
    private const float GRID_OFFSET = 0.002f;
    private const float TRI_Z_OFFSET = 0.5f;

    private bool active;

    // gameobject
    private Vector2Int bottomSize;
    private Vector2Int sideSize;
    private Grid[,] bottomGrids;
    private Grid[,] sideGrids;

    private Grid triangle;

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

        // triangle
        GameObject triangleGO = Instantiate(Resources.Load("Prefabs/Triangle")) as GameObject;
        triangle = triangleGO.GetComponent<Grid>();
        triangle.Init();
        triangle.transform.SetParent(bottomGridsGroup);

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
        sideGridsGroup.gameObject.SetActive(item.Type == ItemType.Vertical);

        Vector3Int size = item.Item.Size;
        bottomSize = new Vector2Int(size.x, size.z);
        sideSize = new Vector2Int(size.x, size.y);

        float offsetX = 0.5f - size.x / 2.0f;
        float offsetZ = 0.5f - size.z / 2.0f;
        float offsetY = 0.5f - size.y / 2.0f;

        triangle.transform.position = new Vector3(0, GRID_OFFSET, size.z / 2.0f + TRI_Z_OFFSET);

        for (int i = 0; i < MAX_SIZE; i++)
        {
            for (int j = 0; j < MAX_SIZE; j++)
            {
                if (i < size.x && j < size.z)
                {
                    Grid grid = bottomGrids[i, j];
                    if (grid == null)
                    {
                        grid = gridPool.GetObject().GetComponent<Grid>();
                        grid.transform.SetParent(bottomGridsGroup);
                        bottomGrids[i, j] = grid;
                        grid.name = "(" + i + ", " + j + ")";
                    }
                    float x = i + offsetX;
                    float z = j + offsetZ;
                    grid.transform.position = new Vector3(x, GRID_OFFSET, z);
                    grid.transform.eulerAngles = new Vector3(0, 0, 0);
                }
                else // remove others
                {
                    Grid grid = bottomGrids[i, j];
                    if (grid != null)
                    {
                        gridPool.PutObject(grid.gameObject);
                        bottomGrids[i, j] = null;
                    }

                }
            }
        }

        if (item.Type == ItemType.Vertical)
        {
            offsetZ = -size.z / 2.0f;
            for (int i = 0; i < MAX_SIZE; i++)
            {
                for (int j = 0; j < MAX_SIZE; j++)
                {
                    if (i < size.x && j < size.y)
                    {
                        Grid grid = sideGrids[i, j];
                        if (grid == null)
                        {
                            grid = gridPool.GetObject().GetComponent<Grid>();
                            grid.transform.SetParent(sideGridsGroup);
                            sideGrids[i, j] = grid;
                            grid.name = "(" + i + ", " + j + ")";
                        }
                        float x = i + offsetX;
                        float y = j + offsetY;
                        grid.transform.position = new Vector3(x, y, offsetZ + GRID_OFFSET);
                        grid.transform.eulerAngles = new Vector3(90, 0, 0);

                    }
                    else // remove others
                    {
                        Grid grid = sideGrids[i, j];
                        if (grid != null)
                        {
                            gridPool.PutObject(grid.gameObject);
                            sideGrids[i, j] = null;
                        }
                    }
                }
            }
        }

        Vector3 position = item.gameObject.transform.position;
    }

    public void SetTransform(Item item)
    {
        Vector3 position = item.Position;
        Vector3 size = item.RotateSize;
        float rotateAngles = item.Dir.Rotation();
        if (item.PlaceType != PlaceType.None)
            bottomGridsGroup.position = new Vector3(position.x, 0, position.z);
        else
            bottomGridsGroup.position = new Vector3(position.x, position.y - size.y / 2.0f, position.z);
        bottomGridsGroup.eulerAngles = new Vector3(0, rotateAngles, 0);

        sideGridsGroup.position = position;
        sideGridsGroup.eulerAngles = new Vector3(0, rotateAngles, 0);
    }

    public void SetBottomGridsType(bool[,] gridTypes)
    {
        setGridsType(bottomGrids, bottomSize, gridTypes);
    }

    public void SetSideGridsType(bool[,] gridTypes)
    {
        setGridsType(sideGrids, sideSize, gridTypes);
    }

    public void SetTriangleType(bool gridType)
    {
        triangle.SetType(gridType);
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
