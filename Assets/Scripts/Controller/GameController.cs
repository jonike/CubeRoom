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
    private Room room;

    private bool isItemEdited;
    private ItemObject currentItem;
    private DragableItem currentItemDrag;
    // private GridObject gridObject;
    private GridGroup gridGroup;

    private void Start()
    {
        camera = Camera.main.GetComponent<RoomCamera>();
        camera.OnCameraRotate = onCameraRotate;

        GameObject roomGO = Instantiate(Resources.Load("Prefabs/Room")) as GameObject;
        room = roomGO.GetComponent<Room>();
        room.Init(roomSize);

        GameObject gridGO = Instantiate(Resources.Load("Prefabs/GridGroup")) as GameObject;
        gridGroup = gridGO.GetComponent<GridGroup>();
        gridGroup.Init();

    }

#if UNITY_EDITOR
    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 150, 60, 20), "+ H Item"))
        {
            if (isItemEdited) return;
            AddItem(ItemType.Horizontal);
        }

        if (GUI.Button(new Rect(80, 150, 60, 20), "+ V Item"))
        {
            if (isItemEdited) return;
            AddItem(ItemType.Vertical);
        }

        if (GUI.Button(new Rect(0, 180, 60, 20), "Rotate Item"))
        {
            if (!isItemEdited) return;
            RotateItem();
        }

        if (GUI.Button(new Rect(0, 210, 60, 20), "Ok Item"))
        {
            if (!isItemEdited) return;
            PlaceItem();
        }
    }
#endif

    private void onCameraRotate(float angle)
    {
        room.RefreshByAngle(Math.mod(angle, 360));
    }

    private void AddItem(ItemType type)
    {
        Vector3Int size = new Vector3Int(2, 1, 1); // TODO

        GameObject itemGO = null;
        if (type == ItemType.Horizontal)
        {
            itemGO = Instantiate(Resources.Load("Prefabs/HItem")) as GameObject;
        }
        else if (type == ItemType.Vertical)
        {
            itemGO = Instantiate(Resources.Load("Prefabs/VItem")) as GameObject;
        }

        itemGO.transform.position = new Vector3(0, 0, 0);
        ItemObject item = itemGO.GetComponent<ItemObject>();
        item.Init(type, size);

        SetEdited(itemGO);

        // Vector3 pos = currentItem.transform.position;
        // pos = realPos(pos, size.x, size.z);
        // currentItem.transform.position = pos;
        // currentItem.RoomPosition = roomPos(pos);

        // gridObject.SetGridsSize(new Vector2Int(size.x, size.z));
        // Vector3 gridPos = gridObject.transform.position;
        // gridPos.x = pos.x - size.x / 2f;
        // gridPos.z = pos.z - size.z / 2f;
        // gridObject.transform.position = gridPos;

        // currentItem.SetActive();
        // gridObject.SetActive();

        // gridObject.SetGridsType(itemGridTypes(currentItem));

    }

    private void SetEdited(GameObject itemGO)
    {
        isItemEdited = true;

        currentItem = itemGO.GetComponent<ItemObject>();
        currentItemDrag = itemGO.GetComponent<DragableItem>();

        room.RefreshGrids(true, currentItem.Type);

        currentItemDrag.OnDrag = OnDragItem;
        currentItemDrag.OnDragBefore = OnBeginDragItem;
        currentItemDrag.OnDragAfter = OnEndDragItem;

        gridGroup.SetGrids(currentItem);

        gridGroup.SetTransform(currentItem);
    }

    private void PlaceItem()
    {

        isItemEdited = false;
        room.RefreshGrids(false, currentItem.Type);
        currentItem = null;
        // TODO current item
        // bool success = room.ConflictSpace(currentItem).Count == 0;
        // if (success) {
        //     room.AddItem(currentItem);
        //     currentItem.SetInactive();
        //     gridObject.SetInactive();
        //     currentItem = null;
        // }
    }

    private void RotateItem()
    {
        currentItem.SetDir(currentItem.Item.Dir.Next());
        gridGroup.SetTransform(currentItem);
        // currentItem.Dir.Next();
        // Vector3 eulerAngles = currentItem.transform.eulerAngles;
        // eulerAngles.y = currentItem.Dir.Rotation();
        // currentItem.transform.eulerAngles = eulerAngles;

        // bool isFlipped = currentItem.Dir.IsFlipped();
        // Vector3Int size = currentItem.Size;
        // Vector3Int rotateSize = isFlipped ? new Vector3Int(size.z, size.y, size.x) : size;
        // currentItem.RotateSize = rotateSize;
        // Vector3 pos = currentItem.transform.position;
        // pos = realPos(pos, rotateSize.x, rotateSize.z);
        // currentItem.transform.position = pos;
        // currentItem.RoomPosition = roomPos(pos);

        // gridObject.SetGridsSize(new Vector2Int(rotateSize.x, rotateSize.z));
        // Vector3 gridPos = gridObject.transform.position;
        // gridPos.x = pos.x - rotateSize.x / 2f;
        // gridPos.z = pos.z - rotateSize.z / 2f;
        // gridObject.transform.position = gridPos;

        // gridObject.SetGridsType(itemGridTypes(currentItem));
    }
    private void OnBeginDragItem()
    {
        Plane plane = currentItem.Item.GetOffsetPlane(currentItem.transform.position);

        Vector3 mousePosition = Util.screenToWorldByPlane(plane, Input.mousePosition);

        // currentItemDrag.DragAnchor = mousePosition;
        currentItemDrag.SetDragOffset(mousePosition);
        // currentItemDrag.DragPlane = itemDragPlane(currentItem, currentItemDrag);
    }

    private void OnDragItem()
    {
        // Vector3Int size = currentItem.RotateSize;

        // Plane plane = currentItemDrag.DragPlane;
        // Vector3 mousePosition = Util.screenToWorldByPlane(plane, Input.mousePosition);
        // Vector3 realPosition = itemRealPostion(currentItem, mousePosition);

        Vector3 realPosition = Vector3.zero;
        if (currentItem.Type == ItemType.Horizontal)
        {
            realPosition = room.ItemPositionOfGround(currentItem.Item, Input.mousePosition, currentItemDrag.DragOffset);
        }
        else if (currentItem.Type == ItemType.Vertical)
        {
            realPosition = room.ItemPositionOfWall(currentItem.Item, Input.mousePosition, currentItemDrag.DragOffset, currentItem.Item.Dir);
        }
        currentItem.transform.position = realPosition;

        gridGroup.SetTransform(currentItem);

        // Debug.Log(mousePosition);

        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // float distance;
        // if (plane.Raycast(ray, out distance))
        // {
        //     Vector3 mousePosition = ray.GetPoint(distance);
        //     Vector3 pos = realPos(mousePosition, size.x, size.z);
        //     pos.y = 0;
        //     currentItem.transform.position = pos;
        //     currentItem.RoomPosition = roomPos(pos);

        //     Vector3 gridPos = gridObject.transform.position;
        //     gridPos.x = pos.x - size.x / 2f;
        //     gridPos.z = pos.z - size.z / 2f;
        //     gridObject.transform.position = gridPos;

        //     gridObject.SetGridsType(itemGridTypes(currentItem));
        // }

    }

    private void OnEndDragItem()
    {
        // currentItem.dragY = 0.0f;
    }

    // private Plane itemDragPlane(ItemObject item, DragableItem itemDrag) {
    //     if (item.Type == ItemType.Horizontal) {
    //         return new Plane(Vector3.down, itemDrag.DragOffset.y);

    //     } else if (item.Type == ItemType.Vertical) {
    //         Vector3 dirVec = item.Item.Dir.Vector;
    //         Vector3 size = room.Size;
    //         float distance = (- Vector3.Scale(dirVec, size / 2) + itemDrag.DragAnchor).magnitude; 
    //         Plane plane = new Plane(- dirVec, - distance);
    //         Debug.Log("plane: " + plane);
    //         return plane;
    //     }

    //     return new Plane(Vector3.down, 0);

    // }
    // private Vector3 itemRealPostion(ItemObject item, Vector3 position) {
    //     if (item.Type == ItemType.Horizontal) {
    //         position.y = 0;
    //         return position;
    //     } else if (item.Type == ItemType.Vertical) {
    //         Vector3 dirVec = item.Item.Dir.Vector;
    //         Vector3 size = room.Size;
    //         Vector3 result = Vector3.Scale(- dirVec, size / 2);
    //         result += Vector3.Scale((Vector3.one - dirVec.normalized), position);
    //         Debug.Log("real pos: " + "  " + result);
    //         return result;
    //     }
    //     return Vector3.zero;
    // }



    // ----------------
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

    // private bool[,] itemGridTypes(ItemBehaviour item) {
    //     List<Vector2Int> conflictSpaces = room.ConflictSpace(item);
    //     List<Vector2Int> conflictGrids = conflictSpaceToGrid(item, conflictSpaces);
    //     bool[,] gridTypes = new bool[item.RotateSize.z, item.RotateSize.x];

    //     foreach (Vector2Int grid in conflictGrids)
    //     {
    //         gridTypes[grid.y, grid.x] = true;
    //     }

    //     return gridTypes;
    // }
    // private List<Vector2Int> conflictSpaceToGrid(ItemBehaviour item, List<Vector2Int> spaces)
    // {
    //     List<Vector2Int> grids = new List<Vector2Int>();
    //     Vector2Int roomPos = new Vector2Int(item.RoomPosition.x, item.RoomPosition.z);
    //     Vector2Int rotateSize = new Vector2Int(item.RotateSize.x, item.RotateSize.z);

    //     foreach (Vector2Int space in spaces)
    //     {
    //         Vector2Int grid = space + rotateSize - roomPos;
    //         grid.x /= 2;
    //         grid.y /= 2;
    //         grids.Add(grid);
    //         Debug.Log(grid);
    //     }
    //     return grids;
    // }

}
