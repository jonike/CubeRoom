using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sorumi.Util;

public class WallData : IData<WallData>
{
    protected override string name { get; set; }

    public WallData()
    {
        name = "wall";
    }

    public static int Count()
    {
        return instance.csvArray.GetLength(0) - 1;
    }

    public static WallPO GetByRow(int i) // 1 ~ Count
    {
        i = i + 1;
        WallPO wall = new WallPO();
        wall.type = BuildType.Wall;
        wall.name = instance.csvArray[i, 0];
        return wall;
    }

    public static string GetNameByRow(int i)
    {
        return instance.csvArray[i + 1, 0];
    }

    public static WallPO[] GetAll()
    {
        WallPO[] walls = new WallPO[Count()];
        for (int i = 0; i < Count(); i++)
        {
            walls[i] = GetByRow(i);
        }
        return walls;
    }

}
