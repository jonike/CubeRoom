using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sorumi.Util;
public class Room : MonoBehaviour
{

    public Vector3Int Size { get; set; }

    private Ground ground;

    private Wall[] walls;

    private Pillar[] pillars;

    private string[] wallNames = { "a", "b", "c", "d" };

    private List<ItemBehaviour> items;

    // private ItemBehaviour[,,] occupiedSpace;
    // private ItemBehaviour[,] groundSpace;
    // private ItemBehaviour[,] wallASpace;
    // private ItemBehaviour[,] wallBSpace;
    // private ItemBehaviour[,] wallCSpace;
    // private ItemBehaviour[,] wallDSpace;

    public void Init(Vector3Int size)
    {
        this.Size = size;
        this.items = new List<ItemBehaviour>();
        // this.groundSpace = new ItemBehaviour[size.z * 2, size.x * 2];
        // this.wallASpace = new ItemBehaviour[size.y * 2, size.x * 2];
        // this.wallBSpace = new ItemBehaviour[size.y * 2, size.z * 2];
        // this.wallCSpace = new ItemBehaviour[size.y * 2, size.x * 2];
        // this.wallDSpace = new ItemBehaviour[size.y * 2, size.z * 2];

        //
        ground = transform.Find("ground").GetComponent<Ground>();
        walls = new Wall[4];
        pillars = new Pillar[4];
        for (int i = 0; i < 4; i++)
        {
            Wall wall = transform.Find("wall_" + wallNames[i]).GetComponent<Wall>();
            walls[i] = wall;
            Pillar pillar = transform.Find("v_" + wallNames[i]).GetComponent<Pillar>();
            pillars[i] = pillar;
        }

    }

    public void RefreshByAngle(float angle)
    {
        walls[0].Hide(angle > 270 || angle < 90);
        walls[1].Hide(angle > 0 && angle < 180);
        walls[2].Hide(angle > 90 && angle < 270);
        walls[3].Hide(angle > 180 && angle < 360);

        pillars[0].Hide(angle > 270 || angle < 180);
        pillars[1].Hide(angle > 0 && angle < 270);
        pillars[2].Hide(angle > 90 && angle < 360);
        pillars[3].Hide(angle > 180 || angle < 90);
    }

    public void RefreshGrids(bool isEdited, ItemType itemType)
    {
        if (itemType == ItemType.Horizontal)
        {
            ground.ShowGrid(isEdited);
        }
        else if (itemType == ItemType.Vertical)
        {
            for (int i = 0; i < 4; i++)
            {
                walls[i].ShowGrid(isEdited);
            }
        }
    }

    public Vector3 ItemPositionOfGround(
       Item item,
       Vector3 screenPosition,
       Vector2 offset)
    {
        Vector3 worldPosition = ScreenToWorldOfGround(screenPosition, offset);
        float maxX = (Size.x - item.RotateSize.x) / 2.0f;
        float minX = -maxX;
        float maxZ = (Size.z - item.RotateSize.z) / 2.0f;
        float minZ = -maxZ;
        float x = Mathf.Clamp(worldPosition.x, minX, maxX);
        float z = Mathf.Clamp(worldPosition.z, minZ, maxZ);

        return new Vector3(x, worldPosition.y, z);
    }

    public Vector3 ItemPositionOfWall(
        Item item,
        Vector3 screenPosition,
        Vector2 offset,
        Direction dir)
    {
        Vector3 worldPosition = ScreenToWorldOfWall(screenPosition, offset, dir);
        Vector3Int itemSize = item.RotateSize;
        float maxY = Size.y - itemSize.y / 2.0f;
        float minY = itemSize.y / 2.0f;
        float y = Mathf.Clamp(worldPosition.y, minY, maxY);

        if (!dir.IsFlipped())
        {
            float maxX = (Size.x - itemSize.x) / 2.0f;
            float minX = -maxX;
            float x = Mathf.Clamp(worldPosition.x, minX, maxX);
            return new Vector3(x, y, worldPosition.z);
        }
        else
        {
            float maxZ = (Size.z - itemSize.z) / 2.0f;
            float minZ = -maxZ;
            
            float z = Mathf.Clamp(worldPosition.z, minZ, maxZ);
            return new Vector3(worldPosition.x, y, z);
        }
    }
    public Vector3 ScreenToWorldOfGround(Vector3 screenPosition, Vector2 offset)
    {
        Plane plane = new Plane(Vector3.down, offset.y);
        Vector3 position = Util.screenToWorldByPlane(plane, screenPosition);
        position.y = 0;
        return position;
    }

    public Vector3 ScreenToWorldOfWall(Vector3 screenPosition, Vector2 offset, Direction dir)
    {
        // TODO
        if (dir.Value % 2 != 0) return Vector3.zero;

        Vector3 dirVec = dir.Vector;
        Vector3 size = Size;
        float distanceRoom = Mathf.Abs(Vector3.Dot(dirVec, size / 2));
        float distance = distanceRoom - offset.x;
        Plane plane = new Plane(dirVec, distance);
        // Debug.Log("plane: " + plane);

        Vector3 position = Util.screenToWorldByPlane(plane, screenPosition);

        position -= offset.x * dirVec;
        position.y -= offset.y;

        // Debug.Log("real pos: " + "  " + position);

        return position;
    }

    // ------------
    // public void AddItem(ItemBehaviour item) {
    //     items.Add(item);

    //     bool isFlipped = item.Dir.IsFlipped();
    //     Vector3Int size = item.Size;
    //     Vector3Int rotateSize = isFlipped ? new Vector3Int(size.z, size.y, size.x) : size;

    //     if (item.IsOccupid) {
    //         for (int z = item.RoomPosition.z - rotateSize.z; z < item.RoomPosition.z + rotateSize.z; z++)
    //         {
    //             for (int x = item.RoomPosition.x - rotateSize.x; x < item.RoomPosition.x + rotateSize.x; x++)
    //             {
    //                 groundSpace[z, x] = item;
    //             }
    //         }

    //     }
    // }

    // public List<Vector2Int> ConflictSpace(ItemBehaviour item){
    // 	List<Vector2Int> space = new List<Vector2Int>();

    // 	bool isFlipped = item.Dir.IsFlipped();
    //     Vector3Int size = item.Size;
    //     Vector3Int rotateSize = isFlipped ? new Vector3Int(size.z, size.y, size.x) : size;

    // 	for (int z = item.RoomPosition.z - rotateSize.z; z < item.RoomPosition.z + rotateSize.z; z++)
    //         {
    //             for (int x = item.RoomPosition.x - rotateSize.x; x < item.RoomPosition.x + rotateSize.x; x++)
    //             {
    //                 if (groundSpace[z, x] != null) {
    // 					space.Add(new Vector2Int(x, z));
    // 				}
    //             }
    //         }

    // 	return space;
    // }

}
