using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTool : MonoBehaviour
{
    public void AddBoxCollider()
    {
        float minX = 0, minY = 0, minZ = 0, maxX = 0, maxY = 0, maxZ = 0;

        BoxCollider collider = GetComponent<BoxCollider>();
        if (!collider)
        {
            collider = gameObject.AddComponent<BoxCollider>();
        }

        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < filters.Length; i++)
        {
            if (filters[i].GetComponent<Renderer>().enabled == false)
                continue;

            string name = filters[i].gameObject.name;
            Debug.Log(name);

            Mesh mesh = filters[i].sharedMesh;
            Bounds bounds = mesh.bounds;

            Vector3 center = bounds.center;
            Vector3 size = bounds.size / 2.0f;

            minX = Mathf.Min(minX, center.x - size.x);
            minY = Mathf.Min(minY, center.y - size.y);
            minZ = Mathf.Min(minZ, center.z - size.z);
            maxX = Mathf.Max(maxX, center.x + size.x);
            maxY = Mathf.Max(maxY, center.y + size.y);
            maxZ = Mathf.Max(maxZ, center.z + size.z);
        }
        Vector3 boxCenter, boxSize;
        boxCenter.x = (minX + maxX) / 2.0f;
        boxCenter.y = (minY + maxY) / 2.0f;
        boxCenter.z = (minZ + maxZ) / 2.0f;
        boxSize.x = maxX - minX;
        boxSize.y = maxY - minY;
        boxSize.z = maxZ - minZ;

        collider.center = boxCenter;
        collider.size = boxSize;
    }
}
