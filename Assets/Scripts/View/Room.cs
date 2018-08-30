using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sorumi.Util;
public class Room : MonoBehaviour
{
    public Material wallMaterial;

    public Material floorMaterial;
    public Vector3Int Size { get; set; }

    private Floor floor;

    private Wall[] walls;

    private Pillar[] pillarsH;
    private Pillar[] pillarsV;

    private string[] wallNames = { "a", "b", "c", "d" };

    private Direction[] wallDirections = {
        Direction.G, Direction.E, Direction.C, Direction.A
    };

    private int[] showWalls;
    private List<ItemObject> items;

    private Dictionary<int, List<ItemObject>> space;

    public void Init(Vector3Int size)
    {
        this.Size = size;
        items = new List<ItemObject>();
        space = new Dictionary<int, List<ItemObject>>();

        floor = transform.Find("floor").GetComponent<Floor>();
        floor.Init(new Vector2Int(size.x, size.z));

        walls = new Wall[4];
        pillarsV = new Pillar[4];
        pillarsH = new Pillar[4];
        for (int i = 0; i < 4; i++)
        {
            Wall wall = transform.Find("wall_" + wallNames[i]).GetComponent<Wall>();
            Direction dir = wallDirections[i];
            int x = (int)Mathf.Abs(Vector3.Dot(dir.Vector, size));
            Vector2Int wallSize = new Vector2Int(x, size.y);
            wall.Init(wallSize, dir);
            walls[i] = wall;

            Pillar pillarV = transform.Find("v_" + wallNames[i]).GetComponent<Pillar>();
            pillarV.Init();
            pillarsV[i] = pillarV;

            Pillar pillarH = transform.Find("h_" + wallNames[i]).GetComponent<Pillar>();
            pillarH.Init();
            pillarsH[i] = pillarH;
        }

        // showWalls
        showWalls = new int[2];
    }

    public void RefreshByAngle(float angle)
    {
        walls[0].Hide(angle >= 270 || angle < 90);
        walls[1].Hide(angle >= 0 && angle < 180);
        walls[2].Hide(angle >= 90 && angle < 270);
        walls[3].Hide(angle >= 180 && angle < 360);

        pillarsV[0].Hide(angle >= 270 || angle < 180);
        pillarsV[1].Hide(angle >= 0 && angle < 270);
        pillarsV[2].Hide(angle >= 90 && angle < 360);
        pillarsV[3].Hide(angle >= 180 || angle < 90);

        pillarsH[0].Hide(angle >= 270 || angle < 90);
        pillarsH[1].Hide(angle >= 0 && angle < 180);
        pillarsH[2].Hide(angle >= 90 && angle < 270);
        pillarsH[3].Hide(angle >= 180 && angle < 360);

        showWalls[0] = (int)Maths.mod((angle / 90) + 3, 4);
        showWalls[1] = (int)Maths.mod((angle / 90) + 2, 4);
    }

    public void RefreshGrids(bool isEdited, ItemType itemType = 0)
    {
        if (isEdited)
        {
            if (itemType == ItemType.Horizontal)
            {
                floor.ShowGrid(true);
            }
            else if (itemType == ItemType.Vertical)
            {
                for (int i = 0; i < 4; i++)
                {
                    walls[i].ShowGrid(true);
                }
            }
        }
        else
        {
            floor.ShowGrid(isEdited);
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

    public Floor Floor()
    {
        return floor;
    }


    public void PlaceItem(ItemObject item)
    {
        items.Add(item);

        if (item.Item.IsOccupid)
        {
            int minX, maxX, minY, maxY, minZ, maxZ;
            bool success = ItemXYZ(item.Item, out minX, out maxX, out minY, out maxY, out minZ, out maxZ);
            if (!success)
                return;

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    for (int z = minZ; z < maxZ; z++)
                    {
                        // string coordinate = x + ", " + y + ", " + z;
                        // Debug.Log(coordinate);
                        int key = x * Size.y * Size.z + y * Size.z + z;
                        if (!space.ContainsKey(key))
                            space.Add(key, new List<ItemObject>());

                        if (space[key].Count > 0)
                        {
                            string coordinate = x + ", " + y + ", " + z;
                            Debug.LogWarning(coordinate + " has already been occupied");
                        }

                        space[key].Add(item);
                    }
                }
            }
        }
    }

    public void DeleteItem(ItemObject item)
    {
        items.Remove(item);

        if (item.Item.IsOccupid)
        {
            int minX, maxX, minY, maxY, minZ, maxZ;
            bool success = ItemXYZ(item.Item, out minX, out maxX, out minY, out maxY, out minZ, out maxZ);
            if (!success)
                return;

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    for (int z = minZ; z < maxZ; z++)
                    {
                        int key = x * Size.y * Size.z + y * Size.z + z;
                        if (space.ContainsKey(key))
                            space[key].Remove(item);
                    }
                }
            }
        }
    }


    public List<Vector3Int> ConflictSpace(Item item)
    {
        List<Vector3Int> conflictSpace = new List<Vector3Int>();

        if (item.IsOccupid)
        {
            int minX, maxX, minY, maxY, minZ, maxZ;
            bool success = ItemXYZ(item, out minX, out maxX, out minY, out maxY, out minZ, out maxZ);
            if (!success)
                return conflictSpace;


            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    for (int z = minZ; z < maxZ; z++)
                    {
                        int key = x * Size.y * Size.z + y * Size.z + z;
                        if (space.ContainsKey(key) && space[key].Count > 0)
                            conflictSpace.Add(new Vector3Int(x, y, z));
                    }
                }
            }
        }
        return conflictSpace;
    }

    public void PlaceWall(WallPO wall)
    {
        Texture2D tex = Resources.Load("Textures/Walls/wall_" + wall.name) as Texture2D;
        wallMaterial.SetTexture("_DetailTex", tex);
    }

    private bool ItemXYZ(Item item, out int minX, out int maxX, out int minY, out int maxY, out int minZ, out int maxZ)
    {
        Vector3Int rotateSize = item.RotateSize;
        Vector3Int roomPosition = item.RoomPosition;
        minX = roomPosition.x;
        maxX = roomPosition.x + rotateSize.x;
        minY = roomPosition.y;
        maxY = roomPosition.y + rotateSize.y;
        minZ = roomPosition.z;
        maxZ = roomPosition.z + rotateSize.z;

        if (minX < 0 || maxX > Size.x || minY < 0 || maxY > Size.y || minZ < 0 || maxZ > Size.z)
        {
            Debug.LogWarning("The item position or size is wrong");
            return false;
        }
        return true;
    }
}
