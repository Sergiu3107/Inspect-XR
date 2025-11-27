using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
public class PointUIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pointUI;

    private UIDocument _pointUIDocument;
    private TextField _pointInfoTextField;
    private Button _pointSaveButton;
    private Button _pointDeleteButton;

    private AnnotationApi _annotationApi;
    private UIManager _uiManager;
    private PointAnnotation _pointAnnotation;

    public void Initialize(AnnotationApi annotationApi, UIManager uiManager)
    {
        _annotationApi = annotationApi;
        _uiManager = uiManager;
    }

    public void SetPointAnnotation(PointAnnotation annotation)
    {
        _pointAnnotation = annotation;
    }

    public void TogglePointUI(bool toggle)
    {
        pointUI.SetActive(toggle);
        Cursor.visible = toggle;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;

        if (toggle) SetupPointUI();
    }

    public bool GetPointUIActive()
    {
        return pointUI.activeSelf;
    }

    private void SetupPointUI()
    {
        if (_pointUIDocument == null)
            _pointUIDocument = pointUI.GetComponent<UIDocument>();

        var root = _pointUIDocument.rootVisualElement;
        _pointInfoTextField = root.Q<TextField>("point-info-text-field");
        _pointSaveButton = root.Q<Button>("point-save-button");
        _pointDeleteButton = root.Q<Button>("point-delete-button");

        if (_pointSaveButton != null)
        {
            _pointSaveButton.clicked -= OnPointSaveClick;
            _pointSaveButton.clicked += OnPointSaveClick;
        }
    
        if (_pointDeleteButton != null)
        {
            _pointDeleteButton.clicked -= OnPointDeleteClick;
            _pointDeleteButton.clicked += OnPointDeleteClick;
        }
    }

    private void OnPointDeleteClick()
    {
        if (!NetworkManager.Singleton.IsClient) return;
        if (_uiManager.CurrentActivePoint == null) return;

        var pointRef = _uiManager.CurrentActivePoint.GetComponent<NetworkObject>();
        StartCoroutine(_annotationApi.DeleteAnnotation(_uiManager.CurrentActivePointId,
            onSuccess: () =>
            {
                _uiManager.RemoveAnnotationServerRpc(pointRef);
                if (pointUI.activeSelf)
                {
                    TogglePointUI(false);
                    _pointAnnotation.ToggleMovementAndLooking(true);
                }
            },
            onError: (error) => { Debug.LogError("Failed to delete Point Annotation: " + error); }));
    }

    private void OnPointSaveClick()
    {
        string dataText = _pointInfoTextField.value;

        if (string.IsNullOrEmpty(dataText))
        {
            Debug.LogWarning("Data text is null or empty, cancelling annotation spawn.");
            _pointAnnotation.RemoveAnnotation();
            return;
        }

        if (_uiManager.CurrentActivePointId != 0)
        {
            UpdateExistingPointAnnotation(dataText);
        }
        else
        {
            CreateNewPointAnnotation(dataText);
        }
    }

    private void UpdateExistingPointAnnotation(string dataText)
    {
        PointAnnotationRequestDto updateRequest = new PointAnnotationRequestDto
        {
            createdById = LocalUserStorage.LoadUser().id,
            modelId = _uiManager.GetSelectedModelId(),
            sessionId = _uiManager.GetSelectedSessionId(),
            data = dataText,
            posX = _uiManager.CurrentPointPosition.x,
            posY = _uiManager.CurrentPointPosition.y,
            posZ = _uiManager.CurrentPointPosition.z
        };

        StartCoroutine(_annotationApi.UpdatePointAnnotation(_uiManager.CurrentActivePointId, updateRequest,
            onSuccess: (response) => { Debug.Log("Annotation updated successfully."); },
            onError: error => { Debug.LogError("Failed to update annotation: " + error); }));
    }

    private void CreateNewPointAnnotation(string dataText)
    {
        PointAnnotationRequestDto request = new PointAnnotationRequestDto
        {
            createdById = LocalUserStorage.LoadUser().id,
            modelId = _uiManager.GetSelectedModelId(),
            sessionId = _uiManager.GetSelectedSessionId(),
            data = dataText,
            posX = _uiManager.CurrentPointPosition.x,
            posY = _uiManager.CurrentPointPosition.y,
            posZ = _uiManager.CurrentPointPosition.z
        };

        StartCoroutine(_annotationApi.AddPointAnnotation(request,
            onSuccess: (response) =>
            {
                long id = response.id;
                _pointAnnotation.SetAnnotationId(id);
                _pointAnnotation.InitializeAnnotationServerRpc(id);
                _pointAnnotation.ToggleMovementAndLooking(true);
                TogglePointUI(false);
            },
            onError: error => { Debug.LogError("Failed to create point annotation: " + error); }));
    }

    public void ShowCurrentPoint(long pointId)
    {
        if (pointId == 0) return;

        StartCoroutine(_annotationApi.GetPointAnnotationById(pointId,
            onSuccess: (pointAnnotation) =>
            {
                if (!pointUI.activeSelf)
                {
                    _pointAnnotation.ToggleMovementAndLooking(false);
                    TogglePointUI(true);
                }

                _pointInfoTextField.value = pointAnnotation.data;
                _pointInfoTextField.label = "Annotation Point " + pointAnnotation.id;
            },
            onError: (error) => { Debug.LogError("Failed to fetch Point Annotation: " + error); }));
    }

    public void FetchPointAnnotations(long sessionId, GameObject pointPrefab)
    {
        StartCoroutine(_annotationApi.GetPointAnnotationsBySession(sessionId,
            onSuccess: (annotations) =>
            {
                foreach (var pointAnnotation in annotations.annotations)
                {
                    Vector3 pos = new Vector3(pointAnnotation.posX, pointAnnotation.posY, pointAnnotation.posZ);
                    GameObject pointGameObject = Instantiate(pointPrefab, pos, Quaternion.identity);

                    var networkObject = pointGameObject.GetComponent<NetworkObject>();
                    if (networkObject != null && NetworkManager.Singleton.IsServer)
                    {
                        networkObject.Spawn(true);
                        var annotation = pointGameObject.GetComponent<AnnotationComponent>();
                        annotation.Initialize(pointAnnotation.id);
                    }
                }
            },
            onError: error => { Debug.LogError("Failed to fetch point annotations: " + error); }));
    }
}