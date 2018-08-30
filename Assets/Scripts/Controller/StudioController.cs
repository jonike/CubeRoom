using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sorumi.Util;

public class StudioController : MonoBehaviour
{

    // temp
    private Vector3Int roomSize = new Vector3Int(12, 12, 12);

    // end temp

    // View
    private RoomCamera camera;
    private Room room;
    private bool isRestricted;
    private bool isItemEdited;
    private ItemObject currentItem;
    private EditedItem editedItem;
    private GridGroup gridGroup;

    // UI
    private StudioPanel studioPanel;

    // Touch

    private Vector3 lastPosition;

    private bool isOverUI;
    private bool isUIHandleDrag;
    private bool isEditItemHandleDrag;
    private bool isEditItemHandleClick;

    private void Start()
    {
        InitUI();
        InitTouch();
        InitView();

        Reset();

        // Init
        studioPanel.SetResetButtonActive(false);
        studioPanel.SetMode(StudioMode.Type);
    }

    #region Init

    private void InitView()
    {
        GameObject roomGO = Instantiate(Resources.Load("Prefabs/Room")) as GameObject;
        room = roomGO.GetComponent<Room>();
        room.Init(roomSize);

        camera = Camera.main.GetComponent<RoomCamera>();
        camera.Init();
        camera.OnCameraRotate = HandleCameraRotate;
        camera.SetCameraTransform();

        GameObject gridGO = Instantiate(Resources.Load("Prefabs/GridGroup")) as GameObject;
        gridGroup = gridGO.GetComponent<GridGroup>();
        gridGroup.Init();

        // TODO
        isRestricted = true;
    }

    private void InitUI()
    {
        studioPanel = GameObject.Find("/Canvas/StudioPanel").GetComponent<StudioPanel>();
        studioPanel.Init();
        studioPanel.OnItemBeginDrag = HandleUIItemBeginDrag;
        studioPanel.OnBuildClick = PlaceWall;
        studioPanel.OnPlaceClick = () =>
        {
            isOverUI = true;
            PlaceItem();
        };
        studioPanel.OnDeleteClick = () =>
        {
            isOverUI = true;
            DeleteItem();
        };
        studioPanel.OnRotateChange = RotateItem;
        studioPanel.OnResetClick = () =>
        {
            isOverUI = true;
            camera.TriggerAnimation();
            studioPanel.SetResetButtonActive(false);
        };
        studioPanel.OnTypeClick = () =>
        {
            isOverUI = true;
        };

    }

    private void InitTouch()
    {
        // Pan
        PanRecognizer panRecognizer = new PanRecognizer();
        panRecognizer.zIndex = 2;

        panRecognizer.gestureBeginEvent += (r) =>
        {
            // Debug.Log("Pan Begin : " + r);
        };

        panRecognizer.gestureRecognizedEvent += (r) =>
        {
            if (isUIHandleDrag)
            {
                DragItem(r.position);
                return;
            }

            if (IsPointerOverUIObject())
            {
                isOverUI = true;
            }
            if (isOverUI) return;

            if (isEditItemHandleDrag)
            {

            }
            else
            {
                Vector2 delta = -(r.deltaPosition) * 0.1f;
                camera.Rotate(delta);
                studioPanel.SetResetButtonActive(true);
            }
        };

        panRecognizer.gestureEndEvent += r =>
        {
            isUIHandleDrag = false;
            isOverUI = false;
        };

        PanRecognizer panTwoRecognizer = new PanRecognizer(2);
        panTwoRecognizer.zIndex = 3;
        panTwoRecognizer.gestureRecognizedEvent += (r) =>
        {
            camera.Move(r.deltaPosition * 0.05f);
            studioPanel.SetResetButtonActive(true);
        };

        PinchRecognizer pinchRecognizer = new PinchRecognizer();
        pinchRecognizer.zIndex = 4;

        pinchRecognizer.gestureRecognizedEvent += (r) =>
        {
            camera.Zoom(r.deltaDistance * 0.05f);
            studioPanel.SetResetButtonActive(true);
        };

        TapRecognizer tapRecognizer = new TapRecognizer();
        tapRecognizer.gestureRecognizedEvent += (r) =>
        {
            if (isOverUI)
            {
                isOverUI = false;
                return;
            }
            if (IsPointerOverUIObject()) return;
            if (isItemEdited) return;

            studioPanel.Back();
        };

        TouchSystem.addRecognizer(panRecognizer);
        TouchSystem.addRecognizer(panTwoRecognizer);
        TouchSystem.addRecognizer(pinchRecognizer);
        TouchSystem.addRecognizer(tapRecognizer);

    }
    #endregion


    private void HandleCameraRotate(float angle)
    {
        angle = Maths.mod(angle, 360);
        room.RefreshByAngle(angle);
        studioPanel.SetRotateButtonRotation(angle);
    }

    private void HandleItemBeginDrag()
    {
        isEditItemHandleDrag = true;

        Plane plane = currentItem.Item.GetOffsetPlane();
        Vector3 mousePosition = Util.screenToWorldByPlane(plane, Input.mousePosition);
        editedItem.SetDragOffset(mousePosition);
    }

    private void HandleItemDrag()
    {
        DragItem(Input.mousePosition);
    }

    private void HandleItemEndDrag()
    {
        isEditItemHandleDrag = false;
    }

    private void HandleUIItemBeginDrag(ItemPO item, Vector2 screenPosition)
    {
        isUIHandleDrag = true;

        AddItem(item);
        Vector3 worldPosition = ScreenToWorldOfOutside(room, currentItem.Item, screenPosition, Vector2.zero);
        SetCurrentItemPosition(room, currentItem, worldPosition);
        editedItem.SetDragOffset(worldPosition);
    }


    private void Reset()
    {
        isItemEdited = false;

        room.RefreshGrids(false);
        currentItem = null;
        editedItem = null;
        gridGroup.SetActive(false);
    }

    #region Room

    private void PlaceWall(BuildPO buildPO)
    {
        if (buildPO.type == BuildType.Wall)
        {
            WallPO wallPO = (WallPO)buildPO;
            room.PlaceWall(wallPO);
        }
    }

    #endregion

    #region Item

    private void AddItem(ItemPO itemPO)
    {
        if (isItemEdited) return;

        GameObject itemGO = null;

        itemGO = Instantiate(Resources.Load("Prefabs/Items/" + itemPO.name)) as GameObject;

        ItemObject item = itemGO.AddComponent<ItemObject>();
        item.Init(itemPO);

        SuspendItem suspendItem = itemGO.AddComponent<SuspendItem>();
        suspendItem.Init();
        suspendItem.OnClick = ClickItem;

        SetEdited(item);

        Direction dir = room.ShowWallsDirection()[0];
        SetCurrentItemDirection(item, dir);
    }

    private void ClickItem(ItemObject item)
    {
        isEditItemHandleClick = true;
        if (isItemEdited && editedItem.CanPlaced)
        {
            PlaceItem();
        }
        if (isItemEdited) return;
        room.DeleteItem(item);
        SetEdited(item);
        SetCurrentItemPosition(room, currentItem, item.Item.Position);
        SetCurrentItemDirection(item, item.Item.Dir);
    }

    private void SetEdited(ItemObject item)
    {
        isItemEdited = true;
        studioPanel.SetMode(StudioMode.EditItem);

        currentItem = item;

        editedItem = item.gameObject.AddComponent<EditedItem>();
        editedItem.Init();

        room.RefreshGrids(true, item.Type);

        editedItem.OnDragBefore = HandleItemBeginDrag;
        editedItem.OnDrag = HandleItemDrag;
        editedItem.OnDragAfter = HandleItemEndDrag;

        SuspendItem suspendItem = item.gameObject.GetComponent<SuspendItem>();
        suspendItem.enabled = false;

        gridGroup.SetActive(true);
        gridGroup.SetGrids(item);
        gridGroup.SetTransform(item.Item);
    }

    private void DragItem(Vector2 screenPosition)
    {
        if (!isItemEdited) return;
        Vector3 itemPosition = ItemPositionOfScreen(room, currentItem, editedItem, screenPosition, editedItem.DragOffset, isRestricted);
        if (currentItem.Item.PlaceType != PlaceType.None)
        {
            editedItem.CanOutside = false;
        }

        SetCurrentItemPosition(room, currentItem, itemPosition);
    }

    private void PlaceItem()
    {
        if (!isItemEdited) return;
        if (!editedItem.CanPlaced)
        {
            Debug.LogWarning("Current item can not be placed!");
            return;
        }

        room.PlaceItem(currentItem);

        // after
        Destroy(editedItem);
        SuspendItem suspendItem = currentItem.gameObject.GetComponent<SuspendItem>();
        suspendItem.enabled = true;

        Reset();
        studioPanel.Back();
    }

    private void DeleteItem()
    {
        if (!isItemEdited) return;
        Destroy(currentItem.gameObject);

        Reset();
        studioPanel.Back();
    }
    private void RotateItem(float degree)
    {
        currentItem.SetDir(Direction.Degree(degree));
        Vector3 itemPoition = ItemPositionOfCurrent(room, currentItem, editedItem, isRestricted);
        SetCurrentItemPosition(room, currentItem, itemPoition);
    }

    private void SetCurrentItemPosition(Room room, ItemObject item, Vector3 itemPosition)
    {

        item.SetPosition(itemPosition);
        item.Item.RoomPosition = roomPosition(item.Item, room.Size, itemPosition);

        bool[,] bottomGrids, sideGrids;
        bool canPlaced = gridTypes(item.Item, out bottomGrids, out sideGrids);
        editedItem.CanPlaced = canPlaced;
        studioPanel.SetPlaceButtonAbled(canPlaced);

        gridGroup.SetTriangleType(canPlaced);
        gridGroup.SetBottomGridsType(bottomGrids);
        if (item.Type == ItemType.Vertical)
            gridGroup.SetSideGridsType(sideGrids);

        gridGroup.SetTransform(item.Item);
    }

    private void SetCurrentItemDirection(ItemObject item, Direction dir)
    {
        item.SetDir(dir);
        studioPanel.SetRotateButtonValue(dir.Rotation());
    }

    #endregion

    #region Item Position
    private Vector3 ItemPositionOfScreen(
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
            Vector3 worldPosition = ScreenToWorldOfFloor(room, item, screenPosition, offset);
            itemPostion = WorldToItemOfFloor(room, item, worldPosition, isRestricted, out distance);
            placeType = PlaceType.Floor;

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
            Vector3 itemPostionG = WorldToItemOfFloor(room, item, ScreenToWorldOfFloor(room, item, screenPosition, offset), isRestricted, out distanceG);
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
                    SetCurrentItemDirection(itemObject, dirL);
                }
                else
                {
                    distance = distanceR;
                    itemPostion = itemPostionR;
                    SetCurrentItemDirection(itemObject, dirR);
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
                placeType = PlaceType.Floor;
                if (editedItem.CanOutside && distanceG < 0)
                {
                    itemPostion = itemPostionO;
                    placeType = PlaceType.None;
                }
            }

        }
        item.PlaceType = placeType;
        return itemPostion;
    }


    // 根据 Item 当前位置，进行适当调整
    private Vector3 ItemPositionOfCurrent(
        Room room,
        ItemObject itemObject,
        EditedItem editedItem,
        bool isRestricted)
    {
        Item item = itemObject.Item;
        Vector3 itemPostion = item.Position;

        if (item.PlaceType == PlaceType.Floor || item.PlaceType == PlaceType.Wall)
        {
            float distance;
            itemPostion = WorldToItemOfFloor(room, item, itemPostion, isRestricted, out distance);
            item.PlaceType = PlaceType.Floor;
        }

        return itemPostion;
    }
    // distance 边缘距离
    private Vector3 WorldToItemOfFloor(
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
            x = itemSize.x % 2 == 1 ? Mathf.Floor(x) + 0.5f : Mathf.Floor(x + 0.5f);
            z = itemSize.z % 2 == 1 ? Mathf.Floor(z) + 0.5f : Mathf.Floor(z + 0.5f);
        }

        Vector3 itemPoition = new Vector3(x, itemSize.y / 2.0f, z); ;

        // if worldPosition on the floor
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

    private Vector3 WorldToItemOfOutside(
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
            x = itemSize.x % 2 == 1 ? Mathf.Floor(x) + 0.5f : Mathf.Floor(x + 0.5f);
            z = itemSize.z % 2 == 1 ? Mathf.Floor(z) + 0.5f : Mathf.Floor(z + 0.5f);
        }

        Vector3 itemPoition = new Vector3(x, itemSize.y / 2.0f + roomSize.y, z); ;

        return itemPoition;
    }

    private Vector3 WorldToItemOfWall(
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
            y = itemSize.y % 2 == 1 ? Mathf.Floor(y) + 0.5f : Mathf.Floor(y + 0.5f);

        Vector3 itemPoition;
        if (!dir.IsFlipped())
        {
            float maxX = (roomSize.x - itemSize.x) / 2.0f;
            float minX = -maxX;
            float x = Mathf.Clamp(worldPosition.x, minX, maxX);

            if (isRestricted)
                x = itemSize.x % 2 == 1 ? Mathf.Floor(x) + 0.5f : Mathf.Floor(x + 0.5f);

            itemPoition = new Vector3(x, y, worldPosition.z);
        }
        else
        {
            float maxZ = (roomSize.z - itemSize.z) / 2.0f;
            float minZ = -maxZ;
            float z = Mathf.Clamp(worldPosition.z, minZ, maxZ);

            if (isRestricted)
                z = itemSize.z % 2 == 1 ? Mathf.Floor(z) + 0.5f : Mathf.Floor(z + 0.5f);

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

    private Vector3 ScreenToWorldOfFloor(Room room, Item item, Vector3 screenPosition, Vector2 offset)
    {
        Plane plane = new Plane(Vector3.down, offset.y + item.Size.y / 2.0f);
        Vector3 position = Util.screenToWorldByPlane(plane, screenPosition);
        position.y -= offset.y;
        return Util.roundPosition(position);
    }

    private Vector3 ScreenToWorldOfOutside(Room room, Item item, Vector3 screenPosition, Vector2 offset)
    {

        Plane plane = new Plane(Vector3.down, offset.y + item.Size.y / 2.0f + room.Size.y);
        Vector3 position = Util.screenToWorldByPlane(plane, screenPosition);
        position.y -= offset.y;
        return Util.roundPosition(position);
    }

    private Vector3 ScreenToWorldOfWall(Room room, Item item, Vector3 screenPosition, Vector2 offset, Direction dir)
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

    #endregion

    #region Grids
    private bool gridTypes(Item item, out bool[,] bottomGrids, out bool[,] sideGrids)
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


        // initialize all true
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

        if (!isRestricted || !item.IsOccupid)
            return true;

        HashSet<Vector2Int> xzGrids, xyGrids, zyGrids;
        List<Vector3Int> conflictSpaces = room.ConflictSpace(item);

        conflictSpaceToGrids(item, conflictSpaces, out xzGrids, out xyGrids, out zyGrids);

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
        Vector3 itemSize = item.RotateSize;
        itemPosition = itemPosition - 0.5f * itemSize;
        return new Vector3Int(
         (int)Mathf.Round(itemPosition.x + 0.5f * roomSize.x),
         (int)Mathf.Round(itemPosition.y),
         (int)Mathf.Round(itemPosition.z + 0.5f * roomSize.z));
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
            Vector3Int grid = space - roomPosition;
            xzGrids.Add(new Vector2Int(grid.x, grid.z));
            xyGrids.Add(new Vector2Int(grid.x, grid.y));
            zyGrids.Add(new Vector2Int(grid.z, grid.y));
        }
    }

    #endregion
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}

