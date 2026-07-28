using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

public class ARController : MonoBehaviour
{
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private Camera arCamera;
    [SerializeField] private GameObject[] placeablePrefabs;
    [SerializeField] private GameObject pointerPrefab;
    [SerializeField] private Material[] swappableMaterials;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float scaleStep = 0.1f;
    [SerializeField] private float minScale = 0.05f;
    [SerializeField] private float maxScale = 2f;

    private int _selectedPrefabIndex;
    private PlacedObjects _selectedObject;

    private readonly List<ARRaycastHit> arHits = new List<ARRaycastHit>();
    private readonly List<PlacedObjects> _placedObjects = new List<PlacedObjects>();

    public PlacedObjects SelectedObject => _selectedObject;
    public int PlacedCount => _placedObjects.Count;
    public Material[] SwappableMaterials => swappableMaterials;

    public event System.Action<PlacedObjects> OnObjectSelected;
    public event System.Action OnSelectionCleared;
    public event System.Action OnPlacedObjectsChanged;


    private void Update()
    {
        UpdatePointer();

        if (TryGetPointerPlaced(out Vector2 screenPos) && !IsPointerOverUI()) HandleInput(screenPos);
    }

    private void UpdatePointer()
    {
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        bool valid = arRaycastManager.Raycast(screenCenter, arHits, TrackableType.PlaneWithinPolygon);

        pointerPrefab.SetActive(valid);

        if(valid)
        {
            Pose hitPose = arHits[0].pose;
            pointerPrefab.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
        }
    }

    private void HandleInput(Vector2 screenPos)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPos);
        if(Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            PlacedObjects placed = hitInfo.collider.GetComponentInParent<PlacedObjects>();
            if(placed != null)
            {
                SelectObject(placed);
                return;
            }
        }

        if(arRaycastManager.Raycast(screenPos, arHits, TrackableType.PlaneWithinPolygon))
        {
            ClearSelection();
            PlaceObject(arHits[0].pose);
        }
    }

    private void PlaceObject(Pose pose)
    {
        GameObject go = Instantiate(placeablePrefabs[_selectedPrefabIndex], pose.position, pose.rotation);

        PlacedObjects placed = go.GetComponent<PlacedObjects>();
        if(placed == null) placed = go.AddComponent<PlacedObjects>();

        _placedObjects.Add(placed);
        OnPlacedObjectsChanged?.Invoke();
    }

    public void ClearSelection()
    {
        if (_selectedObject != null)
        {
            _selectedObject.SetSelected(false);
            _selectedObject = null;
            OnSelectionCleared.Invoke();
        }
    }

    public void ClearAll()
    {
        foreach (PlacedObjects obj in _placedObjects)
            if(obj != null) Destroy(obj.gameObject);
        
        _placedObjects.Clear();
        _selectedObject = null;
        OnSelectionCleared.Invoke();
        OnPlacedObjectsChanged.Invoke();
    }

    private void SelectObject(PlacedObjects placed)
    {
        if (_selectedObject == placed) return;
        
        ClearSelection();
        _selectedObject = placed;
        _selectedObject.SetSelected(true);
        OnObjectSelected?.Invoke(_selectedObject);
    }

    private bool TryGetPointerPlaced(out Vector2 position)
    {
        Pointer pointer = Pointer.current;

        if (pointer != null && pointer.press.wasPressedThisFrame) 
        {
            position = pointer.position.ReadValue();
            return true;
        }

        position = default;
        return false;
    }

    public void RotateSelected(float direction)
    {
        if (_selectedObject != null) _selectedObject.transform.Rotate(Vector3.up, direction * rotationSpeed);
    }

    public void SetSelectedMaterial(int materialIndex)
    {
        if(_selectedObject != null && materialIndex >= 0 && materialIndex<swappableMaterials.Length) _selectedObject.SetMaterial(swappableMaterials[materialIndex]);
    }

    public bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    public void SetSelectedPrefab(int index)
    {
        _selectedPrefabIndex = Mathf.Clamp(index, 0, placeablePrefabs.Length - 1);
    }

    public void DeleteSelected()
    {
        if (_selectedObject == null) return;
        
        _placedObjects.Remove(_selectedObject);
        Destroy(_selectedObject.gameObject);
        _selectedObject = null;
        OnSelectionCleared.Invoke();
        OnPlacedObjectsChanged.Invoke();
    }
}
