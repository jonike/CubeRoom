using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspendItem : MonoBehaviour
{
    public ItemObject Item;
    public Action<ItemObject> OnClick;

    private float maxDistance = 0.5f;

    private Vector2? downPosition;
    public void Init()
    {
        Item = GetComponent<ItemObject>();
    }
    void OnMouseDown()
    {
        downPosition = Input.mousePosition;
    }

    void OnMouseUp()
    {
        if (downPosition.HasValue && Vector2.Distance(downPosition.Value, Input.mousePosition) < maxDistance)
        {
            if (OnClick != null) OnClick(Item);
        }
        downPosition = null;
    }

}
