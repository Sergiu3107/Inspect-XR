
using Unity.Netcode;
using UnityEngine;

public class FollowCamera : NetworkBehaviour
{
    private Transform _cameraTransform;

    void Start()
    {
        _cameraTransform = Camera.main!.transform;
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        transform.position = _cameraTransform.position;
        transform.rotation = _cameraTransform.rotation;
    }
}
