using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class DragAndPlace : MonoBehaviour
{
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private Camera arCamera;
    [SerializeField] private GameObject placeablePrefab;
    [SerializeField] private float objectRadius = 0.5f;

    private readonly List<ARRaycastHit> hits = new();
    private Rigidbody dragTarget;
    private Collider dragCollider;
    private bool isDragging;
    private float dragDistance;

    private void Update()
    {
        Pointer pointer = Pointer.current;
        if (pointer == null) return;

        Vector2 screenPos = pointer.position.ReadValue();
        if(pointer.press.wasPressedThisFrame && !IsPointerOverUI())
            OnPressDown(screenPos);
        else if (pointer.press.isPressed && isDragging)
            OnDragging(screenPos);
        else if(pointer.press.wasReleasedThisFrame && isDragging)
            OnRealese(screenPos);
    }

    private void OnRealese(Vector2 screenPos)
    {
        throw new System.NotImplementedException();
    }

    private void OnDragging(Vector2 screenPos)
    {
        throw new System.NotImplementedException();
    }

    private void OnPressDown(Vector2 screenPos)
    {
        throw new System.NotImplementedException();
    }

    private bool IsPointerOverUI()
    {
        throw new System.NotImplementedException();
    }
}
