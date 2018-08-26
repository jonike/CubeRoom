using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sorumi.Util;

public struct Item_S
{
    public string name;
    public string type;
    public Vector3Int size;
    public bool isOccupied;
}

public class ItemData : IData<ItemData>
{
    protected override string name {get; set;}

    public ItemData()
    {
        name = "items";
    }

    public static int Count()
    {
        return instance.csvArray.GetLength(0) - 1;
    }

    public static Item_S GetByRow(int i)
    {

        Item_S item = new Item_S();
        item.name = instance.csvArray[i, 0];
        item.type = instance.csvArray[i, 1];

        string[] sizeArray = instance.csvArray[i, 2].Split(',');
        int x = Int32.Parse(sizeArray[0]);
        int y = Int32.Parse(sizeArray[1]);
        int z = Int32.Parse(sizeArray[2]);
        item.size = new Vector3Int(x, y, z);

        item.isOccupied = instance.csvArray[i, 3] == "1";

        return item;
    }

    public static string GetNameByRow(int i)
    {
        return instance.csvArray[i, 0];
    }

}
