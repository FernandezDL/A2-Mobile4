using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PhysicsPlaneManager : MonoBehaviour
{
    [SerializeField] private ARPlaneManager planeManager;

    private void Update()
    {
        foreach (ARPlane plane in planeManager.trackables)
        {
            EnsureCollider(plane);
        }
    }

    private void EnsureCollider(ARPlane plane)
    {
        MeshCollider col = plane.GetComponent<MeshCollider>();
        if(col == null) col = plane.gameObject.AddComponent<MeshCollider>();
        
        MeshFilter meshFilter = plane.GetComponent<MeshFilter>();
        if(meshFilter != null && meshFilter.sharedMesh != null && col.sharedMesh != meshFilter.sharedMesh) col.sharedMesh = meshFilter.sharedMesh;
    }
}
