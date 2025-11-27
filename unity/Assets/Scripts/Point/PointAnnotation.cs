using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PointAnnotation : NetworkBehaviour, IAnnotationLogic
{
    [Header("Scene references")] [SerializeField]
    private Camera camera;

    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private LayerMask layerMask;

    [Header("UI")] [SerializeField] private GameObject _inputAnnotationCanvas;
    [SerializeField] private TMP_InputField _inputField;

    [Header("Player")] [SerializeField] private GameObject player;
    private Movement _playerMovement;

    private Looking _playerLooking;

    private NetworkVariable<NetworkObjectReference> _annotationPointRef = new();


    private long _currentAnnotationId;

    void Awake()
    {
        _playerMovement = player.GetComponent<Movement>();
        _playerLooking = player.GetComponent<Looking>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        UIManager.Instance.SetPointAnnotation(this);
    }

    void Update()
    {
        if (!IsOwner) return;

        if (UIManager.Instance.GetPointUIActive())
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.Instance.TogglePointUI(false);
                ToggleMovementAndLooking(true);
                UIManager.Instance.CurrentActivePoint = null;
                UIManager.Instance.CurrentActivePointId = 0;
            }
        }
    }

    public void Add()
    {
        if (!IsClient || !IsOwner) return;

        if (!Input.GetMouseButtonDown(0)) return;

        SpawnAnnotation();
    }

    private void SpawnAnnotation()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) return;
        if (!hit.collider.CompareTag("Model")) return;

        UIManager.Instance.CurrentPointPosition = hit.point;

        SpawnAnnotationServerRpc(hit.point);
        UIManager.Instance.TogglePointUI(true);
        ToggleMovementAndLooking(false);
    }


    [ServerRpc(RequireOwnership = false)]
    private void SpawnAnnotationServerRpc(Vector3 position)
    {
        GameObject obj = Instantiate(pointPrefab, position, Quaternion.identity);
        obj.GetComponent<NetworkObject>().Spawn();

        _annotationPointRef.Value = obj;
    }

    [ServerRpc(RequireOwnership = false)]
    public void InitializeAnnotationServerRpc(long id)
    {
        if (_annotationPointRef.Value.TryGet(out NetworkObject netObj) &&
            netObj.TryGetComponent(out AnnotationComponent annotation))
        {
            annotation.Initialize(id);
        }
    }

    public void SetAnnotationId(long id)
    {
        _currentAnnotationId = id;
    }

    public void ToggleMovementAndLooking(bool toggle)
    {
        _playerMovement.enabled = toggle;
        _playerLooking.enabled = toggle;
    }

    public void RemoveAnnotation()
    {
        if (!IsClient) return;

        RemoveAnnotationServerRpc();
        UIManager.Instance.TogglePointUI(false);
        ToggleMovementAndLooking(true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemoveAnnotationServerRpc()
    {
        if (_annotationPointRef.Value.TryGet(out NetworkObject netObj))
        {
            netObj.Despawn();
            Destroy(netObj.gameObject);
            _annotationPointRef.Value = default;
        }
    }
}