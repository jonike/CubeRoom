using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sorumi.Util;

public class ItemPO
{
    public string name;
    public ItemType type;
    public Vector3Int size;
    public bool isOccupied;
}

public class ItemData : IData<ItemData>
{
    protected override string name { get; set; }

    public ItemData()
    {
        name = "items";
    }

    public static int Count()
    {
        return instance.csvArray.GetLength(0) - 1;
    }

    public static ItemPO GetByRow(int i) // 1 ~ Count
    {
        i = i + 1;
        ItemPO item = new ItemPO();
        item.name = instance.csvArray[i, 0];

        string type = instance.csvArray[i, 1];
        if (type == "h")
            item.type = ItemType.Horizontal;
        else if (type == "v")
            item.type = ItemType.Vertical;

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
        return instance.csvArray[i + 1, 0];
    }

    public static ItemPO[] GetAll()
    {
        ItemPO[] items = new ItemPO[Count()];
        for (int i = 0; i < Count(); i++)
        {
            items[i] = GetByRow(i);
        }
        return items;
    }

}
