using Unity.Netcode;
using UnityEngine;

public class Looking : NetworkBehaviour
{
    public float lookSpeed = 2f;

    private float _yaw = 0f;
    private float _pitch = 0f;
    
    [SerializeField] private Camera _playerCamera;

    void Start()
    {
        if (!IsOwner)
        {
            _playerCamera.enabled = false; 
            return;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        
        _yaw += lookSpeed * Input.GetAxis("Mouse X");
        _pitch -= lookSpeed * Input.GetAxis("Mouse Y");
        _pitch = Mathf.Clamp(_pitch, -90f, 90f);

        transform.eulerAngles = new Vector3(_pitch, _yaw, 0f);
    }
}