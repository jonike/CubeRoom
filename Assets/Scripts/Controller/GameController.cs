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
        GameObject roomGO = Instantiate(Resources.Load("Prefabs/Room")) as GameObject;
        room = roomGO.GetComponent<Room>();
        room.Init(roomSize);

        camera = Camera.main.GetComponent<RoomCamera>();
        camera.Init();
        camera.OnCameraRotate = onCameraRotate;
        camera.SetCameraTransform();

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
        if (GUI.Button(new Rect(80, 210, 60, 20), "X Item"))
        {
            if (!isItemEdited) return;
            DeleteItem();
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


        ItemObject item = itemGO.GetComponent<ItemObject>();
        item.Init(type, size);

        SetEdited(itemGO);

        SetCurrentItemPosition(room, item, new Vector3(0, 3, 0));

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
        Reset();
    }

    private void DeleteItem()
    {
        if (editedItem == null)
            return;
        Destroy(currentItem.gameObject);
        Reset();
    }

    private void Reset()
    {
        isItemEdited = false;
        room.RefreshGrids(false, currentItem.Type);
        currentItem = null;
        editedItem = null;
        gridGroup.SetActive(false);
    }

    private void RotateItem()
    {
        currentItem.SetDir(currentItem.Item.Dir.Next());
        // gridGroup.SetTransform(currentItem);
        SetCurrentItemPosition(room, currentItem, currentItem.transform.position);

    }

    private void OnBeginDragItem()
    {
        Plane plane = currentItem.Item.GetOffsetPlane();

        Vector3 mousePosition = Util.screenToWorldByPlane(plane, Input.mousePosition);

        editedItem.SetDragOffset(mousePosition);
    }

    private void OnDragItem()
    {

        Vector3 itemPosition = ItemPositionOfScreen(room, currentItem, editedItem, Input.mousePosition, editedItem.DragOffset, isRestricted);
        if (currentItem.Item.PlaceType != PlaceType.None) {
            editedItem.CanOutside = false;
        }

        SetCurrentItemPosition(room, currentItem, itemPosition);

        // Debug.Log(currentItem.Item.PlaceType);
    }

    private void OnEndDragItem()
    {
        // currentItem.dragY = 0.0f;
    }


    public void SetCurrentItemPosition(Room room, ItemObject item, Vector3 itemPosition)
    {

        item.SetPosition(itemPosition);
        item.Item.RoomPosition = roomPosition(item.Item, room.Size, itemPosition);

        bool[,] bottomGrids, sideGrids;
        bool canPlaced = gridTypes(item.Item, out bottomGrids, out sideGrids);
          editedItem.CanPlaced = canPlaced;

        gridGroup.SetBottomGridsType(bottomGrids);
        if (item.Type == ItemType.Vertical)
            gridGroup.SetSideGridsType(sideGrids);

        gridGroup.SetTransform(item.Item);
    }
    /***** Pure Function *****/

    /*** position ***/
    public Vector3 ItemPositionOfScreen(
        Room room,
        ItemObject itemObject,
        EditedItem editedItem,
        Vector3 screenPosition,
        Vector2 offset,
        bool isRestricted)
    {
        Item item = itemObject.Item;
        Vector3 itemPostion = Vector3.zero;
        PlaceType placeType = PlaceType.None;
        if (currentItem.Type == ItemType.Horizontal)
        {
            float distance;
            Vector3 worldPosition = ScreenToWorldOfGround(room, item, screenPosition, offset);
            itemPostion = WorldToItemOfGround(room, item, worldPosition, isRestricted, out distance);
            placeType = PlaceType.Ground;

            if (editedItem.CanOutside && distance < 0)
            {
                worldPosition = ScreenToWorldOfOutside(room, item, screenPosition, offset);
                itemPostion = WorldToItemOfOutside(room, item, worldPosition, isRestricted);
                placeType = PlaceType.None;
            }
        }
        else if (currentItem.Type == ItemType.Vertical)
        {
            Direction itemDir = item.Dir;
            Direction[] showWallsDirection = room.ShowWallsDirection();
            Direction dirL = showWallsDirection[0];
            Direction dirR = showWallsDirection[1];

            Vector3 distance, distanceL, distanceR;
            Vector3 itemPostionL = WorldToItemOfWall(room, item, ScreenToWorldOfWall(room, item, screenPosition, offset, showWallsDirection[0]), showWallsDirection[0], isRestricted, out distanceL);
            Vector3 itemPostionR = WorldToItemOfWall(room, item, ScreenToWorldOfWall(room, item, screenPosition, offset, showWallsDirection[1]), showWallsDirection[1], isRestricted, out distanceR);

            float distanceG;
            Vector3 itemPostionG = WorldToItemOfGround(room, item, ScreenToWorldOfGround(room, item, screenPosition, offset), isRestricted, out distanceG);
            Vector3 itemPostionO = WorldToItemOfOutside(room, item, ScreenToWorldOfOutside(room, item, screenPosition, offset), isRestricted);
            
            // Debug.Log(distanceG + " " + distanceL + " " + distanceR);

            if (distanceL.y >= -0.5f || distanceR.y >= -0.5f)
            {
                placeType = PlaceType.Wall;
                if (itemDir == dirL && distanceL.x >= 0)
                {
                    distance = distanceL;
                    itemPostion = itemPostionL;
                }
                else if (itemDir == dirR && distanceR.x >= 0)
                {
                    distance = distanceR;
                    itemPostion = itemPostionR;
                }
                else if (distanceL.x >= distanceR.x)
                {
                    distance = distanceL;
                    itemPostion = itemPostionL;
                    itemObject.SetDir(dirL);
                }
                else
                {
                    distance = distanceR;
                    itemPostion = itemPostionR;
                    itemObject.SetDir(dirR);
                }

                if (editedItem.CanOutside && (distance.y < -0.5f || distance.z < 0))
                {
                    itemPostion = itemPostionO;
                    placeType = PlaceType.None;
                }
            }
            else
            {
                itemPostion = itemPostionG;
                placeType = PlaceType.Ground;
                if (editedItem.CanOutside && distanceG < 0) {
                    itemPostion = itemPostionO;
                    placeType = PlaceType.None;
                }
            }

        }
        item.PlaceType = placeType;
        return itemPostion;
    }

    // distance 边缘距离
    public Vector3 WorldToItemOfGround(
        Room room,
        Item item,
        Vector3 worldPosition,
        bool isRestricted,
        out float distance)
    {
        Vector3Int itemSize = item.RotateSize;
        Vector3Int roomSize = room.Size;

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

        Vector3 itemPoition = new Vector3(x, itemSize.y / 2.0f, z); ;

        // if worldPosition on the ground
        Direction[] showWallsDirection = room.ShowWallsDirection();

        Vector3 axisDirL = Vector3.Cross(Vector3.up, showWallsDirection[0].Vector);
        float axisL = Vector3.Dot(worldPosition, axisDirL);
        float edgeL = Mathf.Abs(Vector3.Dot(roomSize, axisDirL));

        Vector3 axisDirR = Vector3.Cross(Vector3.up, showWallsDirection[1].Vector);
        float axisR = Vector3.Dot(worldPosition, axisDirR);
        float edgeR = Mathf.Abs(Vector3.Dot(roomSize, axisDirR));

        float distanceL = edgeR / 2.0f - axisR;
        float distanceR = edgeL / 2.0f + axisL;

        if (distanceL > edgeR)
            distanceL = edgeR - distanceL;

        if (distanceR > edgeL)
            distanceR = edgeL - distanceR;

        // Debug.Log(distanceL + " " + distanceR);

        distance = Mathf.Min(distanceL, distanceR);

        return itemPoition;
    }

    public Vector3 WorldToItemOfOutside(
       Room room,
       Item item,
       Vector3 worldPosition,
       bool isRestricted)
    {

        Vector3Int itemSize = item.RotateSize;
        Vector3Int roomSize = room.Size;

        // return worldPosition;
        float x = worldPosition.x;
        float z = worldPosition.z;

        if (isRestricted)
        {
            x = Mathf.Round(x * 2) / 2.0f;
            z = Mathf.Round(z * 2) / 2.0f;
        }

        Vector3 itemPoition = new Vector3(x, itemSize.y / 2.0f + roomSize.y, z); ;

        return itemPoition;
    }

    public Vector3 WorldToItemOfWall(
        Room room,
        Item item,
        Vector3 worldPosition,
        Direction dir,
        bool isRestricted,
        out Vector3 distance)
    {
        Vector3Int itemSize = item.RotateSize;
        Vector3Int roomSize = room.Size;

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
        float edge = Mathf.Abs(Vector3.Dot(roomSize, axisDir));

        if (dir == showWallsDirection[0])  //左
            distance.x = edge / 2.0f + axis;
        else if (dir == showWallsDirection[1]) //右
            distance.x = edge / 2.0f - axis;


        distance.z = edge - distance.x;


        if (worldPosition.y > room.Size.y)
        {
            distance.y = room.Size.y - worldPosition.y;
        }
        else
        {
            distance.y = worldPosition.y - itemSize.y / 2.0f;
        }

        return itemPoition;
    }

    public Vector3 ScreenToWorldOfGround(Room room, Item item, Vector3 screenPosition, Vector2 offset)
    {
        Plane plane = new Plane(Vector3.down, offset.y + item.Size.y / 2.0f);
        Vector3 position = Util.screenToWorldByPlane(plane, screenPosition);
        position.y -= offset.y;
        return Util.roundPosition(position);
    }

    public Vector3 ScreenToWorldOfOutside(Room room, Item item, Vector3 screenPosition, Vector2 offset)
    {

        Plane plane = new Plane(Vector3.down, offset.y + item.Size.y / 2.0f + room.Size.y);
        Vector3 position = Util.screenToWorldByPlane(plane, screenPosition);
        position.y -= offset.y;
        return Util.roundPosition(position);
    }

    public Vector3 ScreenToWorldOfWall(Room room, Item item, Vector3 screenPosition, Vector2 offset, Direction dir)
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
        position += item.Size.z / 2.0f * dirVec;
        position.y -= offset.y;
        return Util.roundPosition(position);
    }

    /*** grids ***/
    public bool gridTypes(Item item, out bool[,] bottomGrids, out bool[,] sideGrids)
    {
        Vector3Int itemSize = item.Size;
        Vector3Int rotateSize = item.RotateSize;
        Vector2Int bottomSize = new Vector2Int(itemSize.x, itemSize.z);
        Vector2Int sideSize = new Vector2Int(itemSize.x, itemSize.y);
        Direction itemDir = item.Dir;
        int sizeX = itemSize.x;
        int sizeY = itemSize.y;
        int sizeZ = itemSize.z;
        bottomGrids = new bool[sizeX, sizeZ];
        sideGrids = new bool[sizeX, sizeY];

        if (!item.CanPlaceOfType())
        {
            for (int i = 0; i < rotateSize.x; i++)
            {
                for (int j = 0; j < rotateSize.z; j++)
                {
                    Vector2Int vec = rotateBottomVector(bottomSize, itemDir, new Vector2Int(i, j));
                    bottomGrids[vec.x, vec.y] = false;
                }
            }

            for (int i = 0; i < itemSize.x; i++)
            {
                for (int j = 0; j < itemSize.y; j++)
                {
                    Vector2Int vec = rotateSideVector(sideSize, itemDir, new Vector2Int(i, j));
                    sideGrids[vec.x, vec.y] = false;
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
                Vector2Int vec = rotateBottomVector(bottomSize, itemDir, new Vector2Int(i, j));
                bottomGrids[vec.x, vec.y] = true;
            }
        }

        for (int i = 0; i < itemSize.x; i++)
        {
            for (int j = 0; j < itemSize.y; j++)
            {
                Vector2Int vec = rotateSideVector(sideSize, itemDir, new Vector2Int(i, j));
                sideGrids[vec.x, vec.y] = true;
            }
        }

        if ((xzGrids.Count + xyGrids.Count + zyGrids.Count) == 0)
        {
            return true;
        }

        foreach (Vector2Int grid in xzGrids)
        {
            Vector2Int vec = rotateBottomVector(bottomSize, itemDir, grid);
            bottomGrids[vec.x, vec.y] = false;
        }

        if (item.Dir.Value % 4 == 0)
        {
            foreach (Vector2Int grid in xyGrids)
            {
                Vector2Int vec = rotateSideVector(sideSize, itemDir, grid);
                sideGrids[vec.x, vec.y] = false;
            }

        }
        else
        {
            foreach (Vector2Int grid in zyGrids)
            {
                Vector2Int vec = rotateSideVector(sideSize, itemDir, grid);
                sideGrids[vec.x, vec.y] = false;
            }
        }
        return false;
    }

    private Vector2Int rotateBottomVector(Vector2Int size, Direction dir, Vector2Int coordinate)
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

    private Vector2Int rotateSideVector(Vector2Int size, Direction dir, Vector2Int coordinate)
    {
        switch (dir.Value)
        {
            case 0:
            case 6:
                return coordinate;
            case 2:
            case 4:
                {
                    int x = size.x - coordinate.x - 1;
                    int y = coordinate.y;
                    return new Vector2Int(x, y);
                }
            default:
                return coordinate;
        }
    }

    private Vector3Int roomPosition(Item item, Vector3Int roomSize, Vector3 itemPosition)
    {
        itemPosition = itemPosition * 2;
        return new Vector3Int(
         (int)Mathf.Round(itemPosition.x + roomSize.x),
         (int)Mathf.Round(itemPosition.y),
         (int)Mathf.Round(itemPosition.z + roomSize.z));
    }
    private void conflictSpaceToGrids(Item item, List<Vector3Int> spaces, out HashSet<Vector2Int> xzGrids, out HashSet<Vector2Int> xyGrids, out HashSet<Vector2Int> zyGrids)
    {
        xzGrids = new HashSet<Vector2Int>();
        xyGrids = new HashSet<Vector2Int>();
        zyGrids = new HashSet<Vector2Int>();
        Vector3Int roomPosition = item.RoomPosition;
        Vector3Int rotateSize = item.RotateSize;

        foreach (Vector3Int space in spaces)
        {
            Vector3Int grid = space + rotateSize - roomPosition;
            grid.x /= 2;
            grid.y /= 2;
            grid.z /= 2;
            xzGrids.Add(new Vector2Int(grid.x, grid.z));
            xyGrids.Add(new Vector2Int(grid.x, grid.y));
            zyGrids.Add(new Vector2Int(grid.z, grid.y));
        }
    }

}

