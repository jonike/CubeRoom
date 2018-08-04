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

    private bool isRestricted;
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

        // TODO
        isRestricted = false;

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
        Vector3Int size = new Vector3Int(2, 3, 1); // TODO

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
    }

    private void OnBeginDragItem()
    {
        Plane plane = currentItem.Item.GetOffsetPlane(currentItem.transform.position);

        Vector3 mousePosition = Util.screenToWorldByPlane(plane, Input.mousePosition);

        currentItemDrag.SetDragOffset(mousePosition);
    }

    private void OnDragItem()
    {
        Vector3 realPosition = Vector3.zero;

        realPosition = ItemPosition(room, currentItem, Input.mousePosition, currentItemDrag.DragOffset, isRestricted);

        currentItem.transform.position = realPosition;

        gridGroup.SetTransform(currentItem);

        bool[,] bottomGrids, sideGrids;
        gridTypes(currentItem, out bottomGrids, out sideGrids);
        gridGroup.SetBottomGridsType(bottomGrids);
    }

    private void OnEndDragItem()
    {
        // currentItem.dragY = 0.0f;
    }


    /***** Pure Function *****/

    /*** position ***/
    public Vector3 ItemPosition(
        Room room,
        ItemObject item,
        Vector3 screenPosition,
        Vector2 offset,
        bool isRestricted)
    {

        Vector3 itemPostion = Vector3.zero;
        PlaceableItem placeableItem = null;
        if (currentItem.Type == ItemType.Horizontal)
        {
            float distance;
            itemPostion = ItemPositionOfGround(room, item, screenPosition, offset, item.Item.Dir, isRestricted, out distance, out placeableItem);
        }
        else if (currentItem.Type == ItemType.Vertical)
        {
            Direction itemDir = item.Item.Dir;
            Direction[] showWallsDirection = room.ShowWallsDirection();
            Direction dirL = showWallsDirection[0];
            Direction dirR = showWallsDirection[1];

            Vector2 distanceL, distanceR;
            PlaceableItem placeableItemL, placeableItemR;
            Vector3 itemPostionL = ItemPositionOfWall(room, item, screenPosition, offset, showWallsDirection[0], isRestricted, out distanceL, out placeableItemL);
            Vector3 itemPostionR = ItemPositionOfWall(room, item, screenPosition, offset, showWallsDirection[1], isRestricted, out distanceR, out placeableItemR);

            float distanceG;
            PlaceableItem placeableItemG;
            Vector3 itemPostionG = ItemPositionOfGround(room, item, screenPosition, offset, item.Item.Dir, isRestricted, out distanceG, out placeableItemG);

            // Debug.Log(distanceG + " " + distanceL + " " + distanceR);

            if (distanceL.y >= -0.5f || distanceR.y >= -0.5f)
            {
                if (itemDir == dirL && distanceL.x >= 0)
                {
                    itemPostion = itemPostionL;
                    placeableItem = placeableItemL;
                }
                else if (itemDir == dirR && distanceR.x >= 0)
                {
                    itemPostion = itemPostionR;
                    placeableItem = placeableItemR;
                }
                else if (distanceL.x >= distanceR.x)
                {
                    itemPostion = itemPostionL;
                    placeableItem = placeableItemL;
                    item.SetDir(dirL);
                }
                else if (distanceL.x < distanceR.x)
                {
                    itemPostion = itemPostionR;
                    placeableItem = placeableItemR;
                    item.SetDir(dirR);
                }
            }
            else
            {
                itemPostion = itemPostionG;
            
            }

        }
        item.PlaceableItem = placeableItem;
        return itemPostion;
    }

    // distance 边缘距离
    public Vector3 ItemPositionOfGround(
        Room room,
        ItemObject item,
        Vector3 screenPosition,
        Vector2 offset,
        Direction dir,
        bool isRestricted,
        out float distance,
        out PlaceableItem placeableItem)
    {
        Vector3Int itemSize = item.Item.RotateSize;
        Vector3Int roomSize = room.Size;

        if (item.Type == ItemType.Vertical)
        {
            offset.y += itemSize.y / 2.0f;
        }

        Vector3 worldPosition = ScreenToWorldOfGround(room, screenPosition, offset);

        float maxX = (roomSize.x - itemSize.x) / 2.0f;
        float minX = -maxX;
        float maxZ = (roomSize.z - itemSize.z) / 2.0f;
        float minZ = -maxZ;
        float x = Mathf.Clamp(worldPosition.x, minX, maxX);
        float z = Mathf.Clamp(worldPosition.z, minZ, maxZ);

        if (isRestricted)
        {
            x = Mathf.Round(x * 2) / 2.0f;
            z = Mathf.Round(z * 2) / 2.0f;
        }

        Vector3 itemPoition = new Vector3(x, worldPosition.y, z); ;
        if (item.Type == ItemType.Vertical)
        {
            itemPoition.y = itemSize.y / 2.0f;

            // position in normal
            float normalSize = Mathf.Abs(Vector3.Dot(itemSize, dir.Vector)) / 2.0f;
            Vector3 offsetVector = normalSize * dir.Vector;

            itemPoition -= offsetVector;

        }
        // if worldPosition on the ground
        Direction[] showWallsDirection = room.ShowWallsDirection();

        Vector3 axisDirL = Vector3.Cross(Vector3.up, showWallsDirection[0].Vector);
        float axisL = Vector3.Dot(worldPosition, axisDirL);
        float edgeL = (Mathf.Abs(Vector3.Dot(roomSize, axisDirL)) - Mathf.Abs(Vector3.Dot(itemSize, axisDirL))) / 2.0f;

        Vector3 axisDirR = Vector3.Cross(Vector3.up, showWallsDirection[1].Vector);
        float axisR = Vector3.Dot(worldPosition, axisDirR);
        float edgeR = (Mathf.Abs(Vector3.Dot(roomSize, axisDirR)) - Mathf.Abs(Vector3.Dot(itemSize, axisDirR))) / 2.0f;

        float distanceL = edgeR - axisR;
        float distanceR = edgeL + axisL;

        distance = Mathf.Min(distanceL, distanceR);

        placeableItem = room.Ground().PlaceableItem;

        return itemPoition;
    }

    public Vector3 ItemPositionOfWall(
        Room room,
        ItemObject item,
        Vector3 screenPosition,
        Vector2 offset,
        Direction dir,
        bool isRestricted,
        out Vector2 distance,
        out PlaceableItem placeableItem)
    {
        Vector3Int itemSize = item.Item.RotateSize;
        Vector3Int roomSize = room.Size;
        Vector3 worldPosition = ScreenToWorldOfWall(room, screenPosition, offset, dir);
        float maxY = roomSize.y - itemSize.y / 2.0f;
        float minY = itemSize.y / 2.0f;
        float y = Mathf.Clamp(worldPosition.y, minY, maxY);

        if (isRestricted)
            y = Mathf.Round(y * 2) / 2.0f;

        Vector3 itemPoition;
        if (!dir.IsFlipped())
        {
            float maxX = (roomSize.x - itemSize.x) / 2.0f;
            float minX = -maxX;
            float x = Mathf.Clamp(worldPosition.x, minX, maxX);

            if (isRestricted)
                x = Mathf.Round(x * 2) / 2.0f;

            itemPoition = new Vector3(x, y, worldPosition.z);
        }
        else
        {
            float maxZ = (roomSize.z - itemSize.z) / 2.0f;
            float minZ = -maxZ;
            float z = Mathf.Clamp(worldPosition.z, minZ, maxZ);

            if (isRestricted)
                z = Mathf.Round(z * 2) / 2.0f;

            itemPoition = new Vector3(worldPosition.x, y, z);
        }

        distance = new Vector2();

        // if worldPosition on the wall
        Direction[] showWallsDirection = room.ShowWallsDirection();

        Vector3 axisDir = Vector3.Cross(Vector3.up, dir.Vector);
        float axis = Vector3.Dot(worldPosition, axisDir);
        float edge = Mathf.Abs(Vector3.Dot(roomSize, axisDir)) / 2.0f;

        if (dir == showWallsDirection[0]) //左
            distance.x = edge + axis;
        else if (dir == showWallsDirection[1]) //右
            distance.x = edge - axis;

        distance.y = worldPosition.y - itemSize.y / 2.0f;

        placeableItem = room.WallOfDirection(dir).PlaceableItem;
        return itemPoition;
    }
    public Vector3 ScreenToWorldOfGround(Room room, Vector3 screenPosition, Vector2 offset)
    {

        Plane plane = new Plane(Vector3.down, offset.y);
        Vector3 position = Util.screenToWorldByPlane(plane, screenPosition);
        position.y = 0;
        return Util.roundPosition(position);
    }

    public Vector3 ScreenToWorldOfWall(Room room, Vector3 screenPosition, Vector2 offset, Direction dir)
    {
        // TODO
        if (dir.Value % 2 != 0) return Vector3.zero;

        Vector3 dirVec = dir.Vector;
        Vector3 size = room.Size;
        float distanceRoom = Mathf.Abs(Vector3.Dot(dirVec, size / 2));
        float distance = distanceRoom - offset.x;
        Plane plane = new Plane(dirVec, distance);
        Vector3 position = Util.screenToWorldByPlane(plane, screenPosition);

        position -= offset.x * dirVec;
        position.y -= offset.y;

        return Util.roundPosition(position);
    }

    /*** grids ***/
    public void gridTypes(ItemObject item, out bool[,] bottomGrids, out bool[,] sideGrids)
    {
        Vector3Int itemSize = item.Item.Size;
        int sizeX = itemSize.x;
        int sizeY = itemSize.y;
        int sizeZ = itemSize.z;
        bottomGrids = new bool[sizeX, sizeZ];
        sideGrids = new bool[sizeX, sizeY];

        if (item.PlaceableItem == null)
        {
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeZ; j++)
                {
                    bottomGrids[i, j] = false;
                }

                for (int j = 0; j < sizeY; j++)
                {
                    sideGrids[i, j] = false;
                }
            }
            return;
        }

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                bottomGrids[i, j] = true;
            }

            for (int j = 0; j < sizeY; j++)
            {
                sideGrids[i, j] = true;
            }
        }
    }

    private Vector2Int rotateVector2(Vector2Int size, Direction dir, Vector2Int coordinate)
    {
        switch (dir.Value)
        {
            case 0:
                return coordinate;
            case 2:
                {
                    int x = size.y - coordinate.x - 1;
                    int y = coordinate.x;
                    return new Vector2Int(x, y);
                }
            case 4:
                {
                    int x = size.x - coordinate.x - 1;
                    int y = size.y - coordinate.y - 1;
                    return new Vector2Int(x, y);
                }
            case 6:
                {
                    int x = coordinate.y;
                    int y = size.x - coordinate.y - 1;
                    return new Vector2Int(x, y);
                }
            default:
                return coordinate;
        }
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
