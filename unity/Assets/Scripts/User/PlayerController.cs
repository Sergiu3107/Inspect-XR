using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            _camera.enabled = false;
            _camera.GetComponent<AudioListener>().enabled = false;
        }
    }

}