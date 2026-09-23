using System.Collections.Generic;
using UnityEngine;

namespace BitorTools.Parallax
{
    [System.Serializable]
public class ParallaxLayer
{
    [Header("Layer Settings")]
    [SerializeField] private Transform _layerParent;
    [SerializeField] private float _parallaxMultiplier = 0.5f;
    
    [Header("Auto-Setup")]
    [SerializeField] private bool autoDetectChildren = true;
    [SerializeField] private GameObject backgroundPrefab;
    [SerializeField] private int instanceCount = 3;
    
    private List<Transform> _backgroundElements;
    private Camera _camera;
    private float _elementWidth;
    private float _totalWidth;
    private Bounds _layerBounds;
    
    public void Initialize(Camera camera)
    {
        _camera = camera;
        _backgroundElements = new List<Transform>();
        
        if (_layerParent == null)
        {
            Debug.LogError("ParallaxLayer: Layer parent not assigned!");
            return;
        }
        
        SetupBackgroundElements();
        CalculateDimensions();
        ArrangeElements();
    }
    
    void SetupBackgroundElements()
    {
        if (autoDetectChildren)
        {
            // Use existing children
            for (int i = 0; i < _layerParent.childCount; i++)
            {
                _backgroundElements.Add(_layerParent.GetChild(i));
            }
            
            // If no children and we have a prefab, create instances
            if (_backgroundElements.Count == 0 && backgroundPrefab != null)
            {
                CreateInstances();
            }
        }
        else if (backgroundPrefab != null)
        {
            CreateInstances();
        }
        
        if (_backgroundElements.Count == 0)
        {
            Debug.LogError($"ParallaxLayer: No background elements found for {_layerParent.name}!");
        }
    }
    
    void CreateInstances()
    {
        for (int i = 0; i < instanceCount; i++)
        {
            GameObject instance = Object.Instantiate(backgroundPrefab, _layerParent);
            instance.name = $"{backgroundPrefab.name}_{i}";
            _backgroundElements.Add(instance.transform);
        }
    }
    
    void CalculateDimensions()
    {
        if (_backgroundElements.Count == 0) return;
        
        // Calculate element width from the first element
        Renderer renderer = _backgroundElements[0].GetComponent<Renderer>();
        if (renderer != null)
        {
            _elementWidth = renderer.bounds.size.x;
        }
        else
        {
            // Fallback: try to get from sprite renderer
            SpriteRenderer spriteRenderer = _backgroundElements[0].GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                _elementWidth = spriteRenderer.bounds.size.x;
            }
            else
            {
                Debug.LogWarning($"ParallaxLayer: Could not determine width for {_backgroundElements[0].name}, using default value of 10");
                _elementWidth = 10f;
            }
        }
        
        _totalWidth = _elementWidth * _backgroundElements.Count;
        
        // Calculate layer bounds for wrapping
        float cameraHeight = _camera.orthographicSize * 2;
        float cameraWidth = cameraHeight * _camera.aspect;
        float buffer = _elementWidth; // Add one element width as buffer
        
        _layerBounds = new Bounds(
            Vector3.zero,
            new Vector3(cameraWidth + buffer * 2, cameraHeight, 0)
        );
    }
    
    void ArrangeElements()
    {
        // Arrange elements side by side, centered around camera
        Vector3 cameraPos = _camera.transform.position;
        float startX = cameraPos.x - (_totalWidth * 0.5f) + (_elementWidth * 0.5f);
        
        for (int i = 0; i < _backgroundElements.Count; i++)
        {
            Vector3 pos = _backgroundElements[i].position;
            pos.x = startX + (i * _elementWidth);
            _backgroundElements[i].position = pos;
        }
    }
    
    public void UpdateParallax(Vector3 deltaMovement)
    {
        if (_backgroundElements.Count == 0) return;
        
        Vector3 parallaxMovement = deltaMovement * _parallaxMultiplier;
        
        // Move all elements
        foreach (var element in _backgroundElements)
        {
            element.position += parallaxMovement;
        }
        
        // Handle wrapping
        HandleWrapping();
    }
    
    void HandleWrapping()
    {
        Vector3 cameraPos = _camera.transform.position;
        _layerBounds.center = cameraPos;
        
        foreach (var element in _backgroundElements)
        {
            Vector3 elementPos = element.position;
            
            // Check if element is outside bounds and needs wrapping
            float leftBound = _layerBounds.min.x - _elementWidth * 0.5f;
            float rightBound = _layerBounds.max.x + _elementWidth * 0.5f;
            
            if (elementPos.x < leftBound)
            {
                // Wrap to right side
                elementPos.x += _totalWidth;
                element.position = elementPos;
            }
            else if (elementPos.x > rightBound)
            {
                // Wrap to left side
                elementPos.x -= _totalWidth;
                element.position = elementPos;
            }
        }
    }
}
}
