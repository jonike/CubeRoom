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

    private Direction[] wallDirections = {
        Direction.G, Direction.E, Direction.C, Direction.A
    };

    private Dictionary<Direction, Wall> dirWallMap;
    private int[] showWalls;

    // private List<ItemBehaviour> items;

    private List<ItemObject> items;

    private ItemObject[,,] space;
    // private ItemBehaviour[,,] occupiedSpace;
    // private ItemBehaviour[,] groundSpace;
    // private ItemBehaviour[,] wallASpace;
    // private ItemBehaviour[,] wallBSpace;
    // private ItemBehaviour[,] wallCSpace;
    // private ItemBehaviour[,] wallDSpace;

    public void Init(Vector3Int size)
    {
        this.Size = size;
        items = new List<ItemObject>();
        space = new ItemObject[size.x * 2, size.y * 2, size.z * 2];

        ground = transform.Find("ground").GetComponent<Ground>();
        ground.Init(new Vector2Int(size.x * 2, size.z * 2));

        walls = new Wall[4];
        pillars = new Pillar[4];
        for (int i = 0; i < 4; i++)
        {
            Wall wall = transform.Find("wall_" + wallNames[i]).GetComponent<Wall>();
            Direction dir = wallDirections[i];
            int x = (int)Mathf.Abs(Vector3.Dot(dir.Vector, size)) * 2;
            Vector2Int wallSize = new Vector2Int(x, size.y * 2);
            wall.Init(wallSize, dir);
            walls[i] = wall;

            Pillar pillar = transform.Find("v_" + wallNames[i]).GetComponent<Pillar>();
            pillars[i] = pillar;
        }

        // showWalls
        showWalls = new int[2];

        // direction int map
        dirWallMap = new Dictionary<Direction, Wall>();
        for (int i = 0; i < 4; i++)
        {
            dirWallMap.Add(wallDirections[i], walls[i]);
        }

    }

    public void RefreshByAngle(float angle)
    {
        walls[0].Hide(angle >= 270 || angle < 90);
        walls[1].Hide(angle >= 0 && angle < 180);
        walls[2].Hide(angle >= 90 && angle < 270);
        walls[3].Hide(angle >= 180 && angle < 360);

        pillars[0].Hide(angle >= 270 || angle < 180);
        pillars[1].Hide(angle >= 0 && angle < 270);
        pillars[2].Hide(angle >= 90 && angle < 360);
        pillars[3].Hide(angle >= 180 || angle < 90);

        showWalls[0] = Math.mod((angle / 90) + 3, 4);
        showWalls[1] = Math.mod((angle / 90) + 2, 4);
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

    public Direction[] ShowWallsDirection()
    {
        return new Direction[2] { wallDirections[showWalls[0]], wallDirections[showWalls[1]] };
    }

    public Wall WallOfDirection(Direction dir)
    {
        return dirWallMap[dir];
    }

    public Ground Ground()
    {
        return ground;
    }


    public void PlaceItem(ItemObject item)
    {
        items.Add(item);

        Vector3Int rotateSize = item.Item.RotateSize;
        Vector3Int roomPosition = item.Item.RoomPosition;
        int minX = roomPosition.x - rotateSize.x;
        int maxX = roomPosition.x + rotateSize.x;
        int minY = roomPosition.y - rotateSize.y;
        int maxY = roomPosition.y + rotateSize.y;
        int minZ = roomPosition.z - rotateSize.z;
        int maxZ = roomPosition.z + rotateSize.z;

        if (minX < 0 || maxX > Size.x * 2 || minY < 0 || maxY > Size.y * 2 || minZ < 0 || maxZ > Size.z * 2)
        {
            Debug.LogWarning("The item position or size is wrong");
            return;
        }

        if (item.Item.IsOccupid)
        {
            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    for (int z = minZ; z < maxZ; z++)
                    {
                        if (space[x, y, z] != null)
                        {
                            string coordinate = x + ", " + y + ", " + z;
                            Debug.LogWarning(coordinate + " has already been occupied");
                        }
                        space[x, y, z] = item;
                    }
                }
            }
        }
    }

    public List<Vector3Int> ConflictSpace(ItemObject item)
    {
        List<Vector3Int> conflictSpace = new List<Vector3Int>();

        Vector3Int rotateSize = item.Item.RotateSize;
        Vector3Int roomPosition = item.Item.RoomPosition;
        int minX = roomPosition.x - rotateSize.x;
        int maxX = roomPosition.x + rotateSize.x;
        int minY = roomPosition.y - rotateSize.y;
        int maxY = roomPosition.y + rotateSize.y;
        int minZ = roomPosition.z - rotateSize.z;
        int maxZ = roomPosition.z + rotateSize.z;

        if (minX < 0 || maxX > Size.x * 2 || minY < 0 || maxY > Size.y * 2 || minZ < 0 || maxZ > Size.z * 2)
        {
            Debug.LogWarning("The item position or size is wrong");
            return conflictSpace;
        }

        if (item.Item.IsOccupid)
        {
            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    for (int z = minZ; z < maxZ; z++)
                    {

                        if (space[x, y, z] != null)
                        {
                            string coordinate = x + ", " + y + ", " + z;
                            conflictSpace.Add(new Vector3Int(x, y, z));
                        }
                    }
                }
            }
        }
        return conflictSpace;
    }
}
