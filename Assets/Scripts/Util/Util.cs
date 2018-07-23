using System.Collections;
using System.Collections.Generic;
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
    }
}