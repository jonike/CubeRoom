using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour {

	public ItemType Type;
	public Item Item;


	// Use this for initialization
	public void Init (ItemType type, Vector3Int size) {
		if (type == ItemType.Horizontal) {
			Item = new HorizontalItem();
		} else if (type == ItemType.Vertical) {
			Item = new VerticalItem();
		}
		this.Type = type;
		Item.Size = size;
		Item.RotateSize = size;
		Item.Dir = Direction.A;
		Item.IsOccupid = true; // TODO
	}
	public void Init (Vector3Int size) {
		Init(ItemType.Horizontal, size);
	}
	
	
	// Update is called once per frame
	// void Update () {
		
	// }
}
