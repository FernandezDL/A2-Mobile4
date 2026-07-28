using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlacedController : MonoBehaviour
{
    [SerializeField] private ARController controller;
    [SerializeField] private Button[] prefabButtons;

    [SerializeField] private GameObject actionPanel;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button rotateLeftButton;
    [SerializeField] private Button rotateRightButton;
    [SerializeField] private Button clearAllButton;

    [SerializeField] private Button[] materialButtons;
    [SerializeField] private GameObject materialPanel;
    
    private void Start()
    {
        SetupPrefabButtons();
        SetupActionButtons();
        SetupMaterialButtons();
        actionPanel.SetActive(false);
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        controller.OnObjectSelected += OnObjectSelected;
        controller.OnSelectionCleared += OnSelectionCleared;
        controller.OnPlacedObjectsChanged += RefreshStatus;
    }

    private void SetupPrefabButtons()
    {
        for (int i = 0; i < prefabButtons.Length; i++)
        {
            int index = i;
            prefabButtons[i].onClick.AddListener(() => { controller.SetSelectedPrefab(index); });
        }
    }
    
    private void SetupActionButtons()
    {
        deleteButton.onClick.AddListener(() => controller.DeleteSelected());
        rotateLeftButton.onClick.AddListener(() => controller.RotateSelected(-1f));
        rotateRightButton.onClick.AddListener(() => controller.RotateSelected(1f));
        clearAllButton.onClick.AddListener(() => controller.ClearAll());
    }

    private void SetupMaterialButtons()
    {
        for (int i = 0; i < materialButtons.Length; i++)
        {
            int index = i;
            materialButtons[i].onClick.AddListener(() => { controller.SetSelectedMaterial(index); });
        }
    }

    private void OnObjectSelected(PlacedObjects obg)
    {
        actionPanel.SetActive(true);
        materialPanel.SetActive(true);
        RefreshStatus();
    }

    private void OnSelectionCleared()
    {
        actionPanel.SetActive(false);
        materialPanel.SetActive(false);
        RefreshStatus();
    }
    
    private void RefreshStatus()
    {
        
    }

    private void OnDestroy()
    {
        controller.OnObjectSelected -= OnObjectSelected;
        controller.OnSelectionCleared -= OnSelectionCleared;
        controller.OnPlacedObjectsChanged -= RefreshStatus;
    }
}