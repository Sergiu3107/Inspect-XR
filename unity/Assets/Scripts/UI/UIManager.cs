using System;
using Unity.Netcode;
using UnityEngine;
using Paint;

public class UIManager : NetworkBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Managers")] 
    
    [SerializeField] private UserMenuUIManager userMenuUIManager;
    [SerializeField] private LayersUIManager layersUIManager;
    [SerializeField] private LoginUIManager loginUIManager;
    [SerializeField] private PointUIManager pointUIManager;
    [SerializeField] private ZoomUIManager zoomUIManager;

    [Header("UI Canvases")] [SerializeField]
    private GameObject crosshairCanvas;

    private LayerManager _layerManager;
    private PaintManager _paintManager;

    private UserApi _userApi;
    private ModelApi _modelApi;
    private SessionApi _sessionApi;
    private AnnotationApi _annotationApi;

    // Network variables
    private NetworkVariable<long> SelectedModelId = new NetworkVariable<long>();
    private NetworkVariable<long> SelectedSessionId = new NetworkVariable<long>();

    // Current selections
    private ModelResponseDto _selectedModel;
    private SessionResponseDto _selectedSession;

    // Texture handling
    private ITextureHandler _textureHandler;
    private PointAnnotation _pointAnnotation;

    public PointAnnotation PointAnnotation => _pointAnnotation;
    public GameObject pointPrefab;
    [SerializeField] private LayerMask annotationLayerMask;

    public long CurrentActivePointId { get; set; } = 0;
    public NetworkObject CurrentActivePoint { get; set; } = null;
    public Vector3 CurrentPointPosition { get; set; }
    
    public UserMenuUIManager GetUserMenuUIManager() => userMenuUIManager;
    public LayersUIManager GetLayersUIManager() => layersUIManager;
    public LoginUIManager GetLoginUIManager() => loginUIManager;
    public PointUIManager GetPointUIManager() => pointUIManager;
    public ZoomUIManager GetZoomUIManager() => zoomUIManager;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializeAPIServices();
        InitializeManagers();
    }

    void Start()
    {
        _layerManager = LayerManager.Instance;
        _layerManager.OnLayerUpdated.AddListener(layersUIManager.ListLayers);
    }

    void Update()
    {
        zoomUIManager.HandleZoomInput();
    }

    void OnDestroy()
    {
        if (_layerManager != null)
            _layerManager.OnLayerUpdated.RemoveListener(layersUIManager.ListLayers);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (Instance == null)
            Instance = this;

        crosshairCanvas.SetActive(true);

        if (IsHost)
        {
            userMenuUIManager.SetAddModelButtonVisible(true);
            SelectedModelId.Value = _selectedModel?.id ?? 0;
            SelectedSessionId.Value = _selectedSession?.id ?? 0;
        }
    }

    private void InitializeAPIServices()
    {
        _userApi = new UserApi();
        _modelApi = new ModelApi();
        _sessionApi = new SessionApi();
        _annotationApi = new AnnotationApi();
    }

    private void InitializeManagers()
    {
        userMenuUIManager.Initialize(_userApi, _modelApi, _sessionApi, this);
        layersUIManager.Initialize(_annotationApi, this);
        loginUIManager.Initialize(_userApi, this);
        pointUIManager.Initialize(_annotationApi, this);
        zoomUIManager.Initialize();
    }

    public void SetSelectedModel(ModelResponseDto model)
    {
        _selectedModel = model;
        if (IsHost && SelectedModelId != null)
            SelectedModelId.Value = model?.id ?? 0;
    }

    public void SetSelectedSession(SessionResponseDto session)
    {
        _selectedSession = session;
        if (IsHost && SelectedSessionId != null)
            SelectedSessionId.Value = session?.id ?? 0;
    }

    public ModelResponseDto GetSelectedModel() => _selectedModel;
    public SessionResponseDto GetSelectedSession() => _selectedSession;

    public void SetPaintManager(PaintManager paintManager)
    {
        _paintManager = paintManager;
        zoomUIManager.SetPaintManager(paintManager);
        layersUIManager.SetPaintManager(paintManager);

        if (SelectedSessionId != null)
        {
            layersUIManager.FetchLayerAnnotations(SelectedSessionId.Value);
        }
    }

    public void SetPointAnnotation(PointAnnotation annotation)
    {
        _pointAnnotation = annotation;
        pointUIManager.SetPointAnnotation(annotation);
    }

    public void SetTextureHandler(ITextureHandler textureHandler)
    {
        _textureHandler = textureHandler;
        layersUIManager.SetTextureHandler(textureHandler);
    }

    public bool IsZoomOn() => zoomUIManager.IsZoomOn();

    public void ShowCurrentPoint()
    {
        pointUIManager.ShowCurrentPoint(CurrentActivePointId);
    }

    public void FetchPointAnnotations()
    {
        if (SelectedSessionId != null && SelectedSessionId.Value > 0)
        {
            pointUIManager.FetchPointAnnotations(SelectedSessionId.Value, pointPrefab);
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void RemoveAnnotationServerRpc(NetworkObjectReference pointRef)
    {
        if (pointRef.TryGet(out NetworkObject networkObject))
        {
            networkObject.Despawn(true);
            Destroy(networkObject.gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RefreshLayersForAllClientsServerRpc()
    {
        RefreshLayersForAllClientsClientRpc();
    }

    [ClientRpc]
    private void RefreshLayersForAllClientsClientRpc()
    {
        if (SelectedSessionId != null)
        {
            layersUIManager.FetchLayerAnnotations(SelectedSessionId.Value);
        }
    }

    public void ToggleLayerUI(bool toggle)
    {
        layersUIManager.ToggleLayerUI(toggle);
    }

    public void TogglePointUI(bool toggle)
    {
        pointUIManager.TogglePointUI(toggle);
    }
    
    public void ToggleUserMenuUI(bool toggle)
    {
        pointUIManager.TogglePointUI(toggle);
    }

    public bool GetPointUIActive()
    {
        return pointUIManager.GetPointUIActive();
    }

    public long GetSelectedModelId() => SelectedModelId?.Value ?? 0;
    public long GetSelectedSessionId() => SelectedSessionId?.Value ?? 0;
}