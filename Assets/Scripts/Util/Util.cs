using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Sorumi.Util
{
    public static class Util
    {
        public static Vector3 screenToWorldByPlane(Plane plane, Vector2 screenPosition)
        {
            Vector3 worldPosition = Vector3.zero;

            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            float distance;
            if (plane.Raycast(ray, out distance))
            {
                worldPosition = ray.GetPoint(distance);
            }

            return worldPosition;
        }

        public static Vector3 roundPosition(Vector3 position) {
            position.x = Mathf.Round(position.x * 10.0f) / 10.0f;
            position.y = Mathf.Round(position.y * 10.0f) / 10.0f;
            position.z = Mathf.Round(position.z * 10.0f) / 10.0f;
            return position;
        }
    }
}