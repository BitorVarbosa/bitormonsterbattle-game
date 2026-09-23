using UnityEngine;

namespace BitorTools.Parallax
{
    public class ParallaxController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera _targetCamera;
    
    [Header("Parallax Layers")]
    [SerializeField] private ParallaxLayer[] _parallaxLayers;
    
    private Vector3 _lastCameraPosition;
    
    void Start()
    {
        if (_targetCamera == null)
            _targetCamera = Camera.main;
            
        if (_targetCamera == null)
        {
            Debug.LogError("ParallaxManager: No camera assigned and no main camera found!");
            return;
        }
        
        _lastCameraPosition = _targetCamera.transform.position;
        
        // Initialize all layers
        foreach (var layer in _parallaxLayers)
        {
            layer.Initialize(_targetCamera);
        }
    }
    
    void LateUpdate()
    {
        if (_targetCamera == null) return;
        
        Vector3 currentCameraPosition = _targetCamera.transform.position;
        Vector3 deltaMovement = currentCameraPosition - _lastCameraPosition;
        
        // Update all parallax layers
        foreach (var layer in _parallaxLayers)
        {
            layer.UpdateParallax(deltaMovement);
        }
        
        _lastCameraPosition = currentCameraPosition;
    }
}
}
