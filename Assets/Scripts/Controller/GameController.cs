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
    private EditedItem editedItem;
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
        isRestricted = true;

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
        editedItem = itemGO.AddComponent<EditedItem>();

        room.RefreshGrids(true, currentItem.Type);

        editedItem.OnDrag = OnDragItem;
        editedItem.OnDragBefore = OnBeginDragItem;
        editedItem.OnDragAfter = OnEndDragItem;

        gridGroup.SetActive(true);
        gridGroup.SetGrids(currentItem);
        gridGroup.SetTransform(currentItem);
    }

    private void PlaceItem()
    {
        if (editedItem == null)
            return;
        if (!editedItem.CanPlaced)
        {
            Debug.LogWarning("Current item can not be placed!");
            return;
        }

        room.PlaceItem(currentItem);
        // after
        Destroy(editedItem);
        isItemEdited = false;
        room.RefreshGrids(false, currentItem.Type);
        currentItem = null;
        editedItem = null;
        gridGroup.SetActive(false);
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

        editedItem.SetDragOffset(mousePosition);
    }

    private void OnDragItem()
    {
        Vector3 realPosition = Vector3.zero;

        realPosition = ItemPosition(room, currentItem, Input.mousePosition, editedItem.DragOffset, isRestricted);

        currentItem.transform.position = realPosition;
        currentItem.Item.RoomPosition = roomPosition(currentItem.Item, room.Size, realPosition);

        gridGroup.SetTransform(currentItem);

        bool[,] bottomGrids, sideGrids;
        bool canPlaced = gridTypes(currentItem, out bottomGrids, out sideGrids);
        gridGroup.SetBottomGridsType(bottomGrids);
        if (currentItem.Type == ItemType.Vertical)
            gridGroup.SetSideGridsType(sideGrids);

        editedItem.CanPlaced = canPlaced;
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
            itemPoition -= item.Item.PositionOffset();
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
    public bool gridTypes(ItemObject item, out bool[,] bottomGrids, out bool[,] sideGrids)
    {
        Vector3Int itemSize = item.Item.Size;
        Vector3Int rotateSize = item.Item.RotateSize;
        Vector2Int bottomSize = new Vector2Int(itemSize.x, itemSize.z);
        Direction itemDir = item.Item.Dir;
        int sizeX = itemSize.x;
        int sizeY = itemSize.y;
        int sizeZ = itemSize.z;
        bottomGrids = new bool[sizeX, sizeZ];
        sideGrids = new bool[sizeX, sizeY];

        if (item.PlaceableItem == null)
        {
            for (int i = 0; i < rotateSize.x; i++)
            {
                for (int j = 0; j < rotateSize.z; j++)
                {
                    Vector2Int vec = rotateVector2(bottomSize, itemDir, new Vector2Int(i, j));
                    bottomGrids[vec.x, vec.y] = false;
                }
            }

            for (int i = 0; i < itemSize.x; i++)
            {
                for (int j = 0; j < itemSize.y; j++)
                {
                    sideGrids[i, j] = false;
                }
            }
            return false;
        }

        // 
        HashSet<Vector2Int> xzGrids, xyGrids, zyGrids;
        List<Vector3Int> conflictSpaces = room.ConflictSpace(item);
        conflictSpaceToGrids(item, conflictSpaces, out xzGrids, out xyGrids, out zyGrids);
        for (int i = 0; i < rotateSize.x; i++)
            {
                for (int j = 0; j < rotateSize.z; j++)
                {
                    Vector2Int vec = rotateVector2(bottomSize, itemDir, new Vector2Int(i, j));
                    bottomGrids[vec.x, vec.y] = true;
                }
            }

            for (int i = 0; i < itemSize.x; i++)
            {
                for (int j = 0; j < itemSize.y; j++)
                {
                    sideGrids[i, j] = true;
                }
            }

        if ((xzGrids.Count + xyGrids.Count + zyGrids.Count) == 0)
        {
            return true;
        }

        foreach (Vector2Int grid in xzGrids)
        {
            Vector2Int vec = rotateVector2(bottomSize, itemDir, grid);
            Debug.Log(vec);
            bottomGrids[vec.x, vec.y] = false;
        }

        if (item.Item.Dir.Value % 4 == 0)
        {
            foreach (Vector2Int grid in xyGrids)
                sideGrids[grid.x, grid.y] = false;
        }
        else
        {
            foreach (Vector2Int grid in zyGrids)
                sideGrids[grid.x, grid.y] = false;
        }
        return true;
    }

    private Vector2Int rotateVector2(Vector2Int size, Direction dir, Vector2Int coordinate)
    {
        switch (dir.Value)
        {
            case 0:
                return coordinate;
            case 2:
                {
                    int x = size.x - coordinate.y - 1;
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
                    int y = size.y - coordinate.x - 1;
                    return new Vector2Int(x, y);
                }
            default:
                return coordinate;
        }
    }

    private Vector3Int roomPosition(Item item, Vector3Int roomSize, Vector3 itemPosition)
    {
        itemPosition = (itemPosition + item.PositionOffset()) * 2;
        return new Vector3Int(
         (int)Mathf.Round(itemPosition.x + roomSize.x),
         (int)Mathf.Round(itemPosition.y),
         (int)Mathf.Round(itemPosition.z + roomSize.z));
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
    private void conflictSpaceToGrids(ItemObject item, List<Vector3Int> spaces, out HashSet<Vector2Int> xzGrids, out HashSet<Vector2Int> xyGrids, out HashSet<Vector2Int> zyGrids)
    {
        xzGrids = new HashSet<Vector2Int>();
        xyGrids = new HashSet<Vector2Int>();
        zyGrids = new HashSet<Vector2Int>();
        Vector3Int roomPosition = item.Item.RoomPosition;
        Vector3Int rotateSize = item.Item.RotateSize;

        foreach (Vector3Int space in spaces)
        {
            Vector3Int grid = space + rotateSize - roomPosition;
            Debug.Log("conflict: " + grid);
            grid.x /= 2;
            grid.y /= 2;
            grid.z /= 2;
            xzGrids.Add(new Vector2Int(grid.x, grid.z));
            xyGrids.Add(new Vector2Int(grid.x, grid.y));
            zyGrids.Add(new Vector2Int(grid.z, grid.y));
        }
    }

}

