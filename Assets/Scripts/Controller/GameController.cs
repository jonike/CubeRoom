using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sorumi.Util;

public class GameController : MonoBehaviour
{

    // temp
    private Vector3Int roomSize = new Vector3Int(6, 6, 6);

    private float minX = -3f;
    private float maxX = 3f;
    private float minZ = -3f;
    private float maxZ = 3f;

    // end temp

    private RoomCamera camera;
    private RoomObject roomObject;
    private ItemObject currentItemObject;
    private GridObject gridObject;

    private void Start()
    {
        camera = Camera.main.GetComponent<RoomCamera>();
        camera.OnCameraRotate = onCameraRotate;

        GameObject roomGO = Instantiate(Resources.Load("Prefabs/Room")) as GameObject;
        roomObject = roomGO.GetComponent<RoomObject>();
        roomObject.Init(roomSize);

        GameObject gridGO = Instantiate(Resources.Load("Prefabs/Grids")) as GameObject;
        gridObject = gridGO.GetComponent<GridObject>();
        gridObject.Init();
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 150, 50, 20), "+ Item"))
        {
            if (currentItemObject != null) return;
            AddItem();
        }

        if (GUI.Button(new Rect(0, 180, 50, 20), "Rotate Item"))
        {
            if (currentItemObject == null) return;
            RotateItem();
        }

        if (GUI.Button(new Rect(0, 210, 50, 20), "Ok Item"))
        {
            if (currentItemObject == null) return;
            PlaceItem();
        }
    }
#endif

    private void onCameraRotate(float angle) {
        roomObject.RefreshByAngle(Math.mod(angle, 360));
    }

    private void AddItem()
    {
        Vector3Int size = new Vector3Int(3, 1, 2); // TODO

        GameObject itemGO = Instantiate(Resources.Load("Prefabs/Item")) as GameObject;
        currentItemObject = itemGO.GetComponent<ItemObject>();
        currentItemObject.Init(size);

        currentItemObject.OnDrag = DragItem;
        currentItemObject.OnDragBefore = BeforeDragItem;
        currentItemObject.OnDragAfter = AfterDragItem;

        Vector3 pos = currentItemObject.transform.position;
        pos = realPos(pos, size.x, size.z);
        currentItemObject.transform.position = pos;
        currentItemObject.RoomPosition = roomPos(pos);

        gridObject.SetGridsSize(new Vector2Int(size.x, size.z));
        Vector3 gridPos = gridObject.transform.position;
        gridPos.x = pos.x - size.x / 2f;
        gridPos.z = pos.z - size.z / 2f;
        gridObject.transform.position = gridPos;

        currentItemObject.SetActive();
        gridObject.SetActive();

        gridObject.SetGridsType(itemGridTypes(currentItemObject));

    }

    private void PlaceItem()
    {
        if (!currentItemObject) return;
        
        // TODO current item
        bool success = roomObject.ConflictSpace(currentItemObject).Count == 0;
        if (success) {
            roomObject.AddItem(currentItemObject);
            currentItemObject.SetInactive();
            gridObject.SetInactive();
            currentItemObject = null;
        }
    }

    private void RotateItem()
    {
        currentItemObject.Dir.Next();
        Vector3 eulerAngles = currentItemObject.transform.eulerAngles;
        eulerAngles.y = currentItemObject.Dir.Rotation();
        currentItemObject.transform.eulerAngles = eulerAngles;

        bool isFlipped = currentItemObject.Dir.IsFlipped();
        Vector3Int size = currentItemObject.Size;
        Vector3Int rotateSize = isFlipped ? new Vector3Int(size.z, size.y, size.x) : size;
        currentItemObject.RotateSize = rotateSize;
        Vector3 pos = currentItemObject.transform.position;
        pos = realPos(pos, rotateSize.x, rotateSize.z);
        currentItemObject.transform.position = pos;
        currentItemObject.RoomPosition = roomPos(pos);

        gridObject.SetGridsSize(new Vector2Int(rotateSize.x, rotateSize.z));
        Vector3 gridPos = gridObject.transform.position;
        gridPos.x = pos.x - rotateSize.x / 2f;
        gridPos.z = pos.z - rotateSize.z / 2f;
        gridObject.transform.position = gridPos;

        gridObject.SetGridsType(itemGridTypes(currentItemObject));
    }
    private void BeforeDragItem()
    {
        float z = currentItemObject.transform.position.z;
        Plane plane = new Plane(Vector3.back, z);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 mousePosition = ray.GetPoint(distance);
            currentItemObject.dragY = mousePosition.y;
        }
    }

    private void DragItem()
    {
        Vector3Int size = currentItemObject.RotateSize;

        Plane plane = new Plane(Vector3.down, currentItemObject.dragY);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 mousePosition = ray.GetPoint(distance);
            Vector3 pos = realPos(mousePosition, size.x, size.z);
            pos.y = 0;
            currentItemObject.transform.position = pos;
            currentItemObject.RoomPosition = roomPos(pos);

            Vector3 gridPos = gridObject.transform.position;
            gridPos.x = pos.x - size.x / 2f;
            gridPos.z = pos.z - size.z / 2f;
            gridObject.transform.position = gridPos;

            gridObject.SetGridsType(itemGridTypes(currentItemObject));
        }

    }

    private void AfterDragItem()
    {
        currentItemObject.dragY = 0.0f;
    }

    // util
    // 相对于世界的绝对位置
    private Vector3 realPos(Vector3 pos, int sx, int sz)
    {
        float x = Mathf.Clamp(Mathf.Round(pos.x - 0.5f * sx % 2) + 0.5f * sx % 2, minX + 0.5f * sx, maxX - 0.5f * sx);
        float z = Mathf.Clamp(Mathf.Round(pos.z - 0.5f * sz % 2) + 0.5f * sz % 2, minZ + 0.5f * sz, maxZ - 0.5f * sz);

        return new Vector3(x, pos.y, z);
    }

    // 相对于房间的位置
    private Vector3Int roomPos(Vector3 pos)
    {
        pos = pos * 2;
        return new Vector3Int(
         (int)pos.x + roomSize.x,
         (int)pos.y,
         (int)pos.z + roomSize.z);
    }

    private bool[,] itemGridTypes(ItemObject item) {
        List<Vector2Int> conflictSpaces = roomObject.ConflictSpace(item);
        List<Vector2Int> conflictGrids = conflictSpaceToGrid(item, conflictSpaces);
        bool[,] gridTypes = new bool[item.RotateSize.z, item.RotateSize.x];

        foreach (Vector2Int grid in conflictGrids)
        {
            gridTypes[grid.y, grid.x] = true;
        }

        return gridTypes;
    }
    private List<Vector2Int> conflictSpaceToGrid(ItemObject item, List<Vector2Int> spaces)
    {
        List<Vector2Int> grids = new List<Vector2Int>();
        Vector2Int roomPos = new Vector2Int(item.RoomPosition.x, item.RoomPosition.z);
        Vector2Int rotateSize = new Vector2Int(item.RotateSize.x, item.RotateSize.z);

        foreach (Vector2Int space in spaces)
        {
            Vector2Int grid = space + rotateSize - roomPos;
            grid.x /= 2;
            grid.y /= 2;
            grids.Add(grid);
            Debug.Log(grid);
        }
        return grids;
    }

}
