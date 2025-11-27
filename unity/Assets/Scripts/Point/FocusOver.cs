using Unity.Netcode;
using UnityEngine;

public class FocusOver : NetworkBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private LayerMask annotationLayer;

    void Update()
    {
        if (!IsClient || !IsOwner) return;
        if (!Input.GetMouseButtonDown(0)) return;
        
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, annotationLayer)) return;

        GameObject annotationPoint = hit.collider.gameObject;
        annotationPoint.TryGetComponent(out AnnotationComponent annotationComponent);
        annotationPoint.TryGetComponent(out NetworkObject networkObject);
        
        UIManager.Instance.CurrentActivePointId = annotationComponent.id.Value;
        UIManager.Instance.CurrentActivePoint = networkObject;
        UIManager.Instance.CurrentPointPosition = annotationPoint.transform.position;
        UIManager.Instance.ShowCurrentPoint();
    }
}