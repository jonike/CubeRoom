using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{

    private const float ITEM_OFFSET = 0.003f;

    public ItemType Type;
    public Item Item;

    // public PlaceableItem PlaceableItem {
    // 	get {
    // 		return Item.PlaceableItem;
    // 	}
    // 	set {
    // 		Item.PlaceableItem = value;
    // 	}
    // }
    public void Init(ItemPO itemPO)
    {
        ItemType type = itemPO.type;
        Vector3Int size = itemPO.size;
        if (type == ItemType.Horizontal)
        {
            Item = new HorizontalItem();
        }
        else if (type == ItemType.Vertical)
        {
            Item = new VerticalItem();
        }
        this.Type = type;
        Item.Dir = Direction.A; // TODO
        Item.Size = size;
        Item.FlippedSize = new Vector3Int(size.z, size.y, size.x);
        Item.RotateSize = Item.Dir.IsFlipped() ? Item.FlippedSize : Item.Size;
        Item.IsOccupid = itemPO.isOccupied;

        gameObject.name = itemPO.name;
    }

    public void Init(ItemType type, Vector3Int size)
    {
        if (type == ItemType.Horizontal)
        {
            Item = new HorizontalItem();
        }
        else if (type == ItemType.Vertical)
        {
            Item = new VerticalItem();
        }
        this.Type = type;
        Item.Dir = Direction.A; // TODO
        Item.Size = size;
        Item.FlippedSize = new Vector3Int(size.z, size.y, size.x);
        Item.RotateSize = Item.Dir.IsFlipped() ? Item.FlippedSize : Item.Size;
        Item.IsOccupid = true; // TODO
    }
    public void Init(Vector3Int size)
    {
        Init(ItemType.Horizontal, size);
    }

    public void SetDir(Direction dir)
    {
        Item.Dir = dir;
        Item.RotateSize = dir.IsFlipped() ? Item.FlippedSize : Item.Size;
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.y = dir.Rotation();
        transform.eulerAngles = eulerAngles;
    }

    public void SetPosition(Vector3 position)
    {
        position.y += ITEM_OFFSET;
        Item.Position = position;
        transform.position = position - Item.CenterPositionOffset();
    }
}
