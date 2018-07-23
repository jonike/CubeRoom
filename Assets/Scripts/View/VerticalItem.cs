using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalItem : Item {
    public override void SetEdited(bool edited) {

	}

    public override Vector3 GetDragAnchor(Vector3 position) {
		Vector3 result = Vector3.Scale(Dir.Vector, position);
        return result;
    }

	public override Vector2 GetDragOffset(Vector3 position) {
        return new Vector2(Vector3.Dot(Dir.Vector, position), position.y);
    }

	public override Plane GetOffsetPlane(Vector3 position)
    {
		Vector3 dir = Vector3.Cross(Dir.Vector, Vector3.up);
        float distance = Vector3.Dot(dir.normalized, position);
		Plane plane = new Plane(dir, - distance);
		Debug.Log("offset plane: " + plane);
        return plane;
    }

}
