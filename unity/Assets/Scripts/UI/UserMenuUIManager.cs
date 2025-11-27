using System;
using System.Collections.Generic;
using SFB;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class UserMenuUIManager : MonoBehaviour
{
    [Header("UI")] [SerializeField] private GameObject userMenuUI;
    [SerializeField] private GameObject loginUI;

    private UIDocument _userMenuUIDocument;
    private Button _addModelButton;
    private Button _deleteModelButton;
    private Button _disconnectButton;
    private Button _createSessionButton;
    private Button _joinSessionButton;
    private TextField _displayNameField;
    private TextField _sessionCodeField;
    private Label _usernameLabel;
    private ListView _modelsListView;
    private ListView _sessionsListView;

    // API services
    private UserApi _userApi;
    private ModelApi _modelApi;
    private SessionApi _sessionApi;
    private UIManager _uiManager;

    // Data
    private List<ModelResponseDto> _userModels;
    private List<SessionResponseDto> _userSessions;
    private ModelResponseDto _selectedModel;
    private SessionResponseDto _selectedSession;

    public void Initialize(UserApi userApi, ModelApi modelApi, SessionApi sessionApi, UIManager uiManager)
    {
        _userApi = userApi;
        _modelApi = modelApi;
        _sessionApi = sessionApi;
        _uiManager = uiManager;

        SetupUI();
    }

    private void SetupUI()
    {
        _userMenuUIDocument = userMenuUI.GetComponent<UIDocument>();

        // Setup buttons
        _addModelButton = _userMenuUIDocument.rootVisualElement.Q("add-button") as Button;
        _addModelButton!.clicked += OnAddModelClick;

        _deleteModelButton = _userMenuUIDocument.rootVisualElement.Q("delete-button") as Button;
        _deleteModelButton!.clicked += OnDeleteModelClick;

        _disconnectButton = _userMenuUIDocument.rootVisualElement.Q("disconnect-button") as Button;
        _disconnectButton!.clicked += OnDisconnectClick;

        _createSessionButton = _userMenuUIDocument.rootVisualElement.Q("create-session-button") as Button;
        _createSessionButton!.clicked += OnCreateSessionClick;

        _joinSessionButton = _userMenuUIDocument.rootVisualElement.Q("join-session-button") as Button;
        _joinSessionButton!.clicked += OnJoinSessionClick;

        _sessionCodeField = _userMenuUIDocument.rootVisualElement.Q("session-code-text-field") as TextField;
        _displayNameField = _userMenuUIDocument.rootVisualElement.Q("display-name-text-field") as TextField;
        _usernameLabel = _userMenuUIDocument.rootVisualElement.Q("username-label") as Label;
        _modelsListView = _userMenuUIDocument.rootVisualElement.Q("models-list-view") as ListView;
        _sessionsListView = _userMenuUIDocument.rootVisualElement.Q("sessions-list-view") as ListView;
    }

    public void SetAddModelButtonVisible(bool visible)
    {
        if (_addModelButton != null)
            _addModelButton.visible = visible;
    }

    public void SetUsernameLabel(string username)
    {
        if (_usernameLabel != null)
            _usernameLabel.text = username;
    }

    public void LoadUserModels()
    {
        long id = LocalUserStorage.LoadUser().id;

        StartCoroutine(_modelApi.GetModelsByOwnerId(id,
            onSuccess: models =>
            {
                _userModels = models.models;
                SetupModelsListView();
            },
            onError: error => { Debug.LogError("Model loading failed: " + error); }));
    }

    public void LoadUserSessions()
    {
        long id = LocalUserStorage.LoadUser().id;

        StartCoroutine(_sessionApi.GetSessionsByOwnerId(id,
            onSuccess: sessions =>
            {
                Debug.Log("Loading sessions from user");
                _userSessions = sessions.sessions;
                SetupSessionsListView();
            },
            onError: error => { Debug.LogError("Session loading failed: " + error); }));
    }

    private void SetupModelsListView()
    {
        VisualTreeAsset modelEntryTemplate = Resources.Load<VisualTreeAsset>("ModelEntryVT");

        _modelsListView.makeItem = () => { return modelEntryTemplate.CloneTree(); };

        _modelsListView.bindItem = (element, index) =>
        {
            if (index < 0 || index >= _userModels.Count) return;

            var model = _userModels[index];
            var radioButton = element.Q<RadioButton>("model-radio-button");
            radioButton.label = model.displayName;
            radioButton.value = false;

            radioButton.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    _selectedModel = model;
                    _uiManager.SetSelectedModel(model);
                }
            });
        };

        _modelsListView.itemsSource = _userModels;
        _modelsListView.fixedItemHeight = 40f;
        _modelsListView.Rebuild();
    }

    private void SetupSessionsListView()
    {
        VisualTreeAsset sessionEntryTemplate = Resources.Load<VisualTreeAsset>("SessionEntryVT");

        _sessionsListView.makeItem = () => { return sessionEntryTemplate.CloneTree(); };

        _sessionsListView.bindItem = (element, index) =>
        {
            if (index < 0 || index >= _userSessions.Count) return;
            var session = _userSessions[index];
            var radioButton = element.Q<RadioButton>("session-radio-button");
            var sessionIdLabel = element.Q<Label>("session-id-label");
            var sessionCreatedAtLabel = element.Q<Label>("session-created-at-label");


            radioButton.label = session.model.displayName;
            radioButton.value = false;
            sessionIdLabel.text = session.id.ToString();
            if (DateTime.TryParse(session.createdAt, out DateTime createdAt))
            {
                sessionCreatedAtLabel.text = createdAt.ToString("dd-MM-yyyy HH:mm");
            }

            radioButton.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    _selectedSession = session;
                    _selectedModel = session.model;
                    _uiManager.SetSelectedSession(session);
                    _uiManager.SetSelectedModel(session.model);
                }
            });
        };

        _sessionsListView.itemsSource = _userSessions;
        _sessionsListView.fixedItemHeight = 40f;
        _sessionsListView.Rebuild();
    }

    private void OnAddModelClick()
    {
        string displayName = _displayNameField.value?.Trim();

        if (string.IsNullOrEmpty(displayName))
        {
            Debug.LogWarning("Display name is required to add a model.");
            return;
        }

        var paths = StandaloneFileBrowser.OpenFilePanel("Select Model", "", new[]
        {
            new ExtensionFilter("3D Model File", "zip"),
        }, false);

        if (paths.Length == 0 || string.IsNullOrEmpty(paths[0]))
        {
            Debug.LogWarning("No model file selected.");
            return;
        }

        string selectedPath = paths[0];
        long userId = LocalUserStorage.LoadUser().id;

        var modelRequest = new ModelRequestDto
        {
            objectUrl = selectedPath,
            displayName = displayName,
            modelFile = null,
            ownerId = userId
        };

        StartCoroutine(_modelApi.AddModel(modelRequest, selectedPath,
            onSuccess: model => { LoadUserModels(); },
            onError: error => { Debug.LogError("Failed to add model: " + error); }));
    }

    private void OnDeleteModelClick()
    {
        if (_selectedModel == null)
        {
            Debug.LogWarning("No model selected to delete.");
            return;
        }

        StartCoroutine(_modelApi.DeleteModel(_selectedModel.id,
            onSuccess: () =>
            {
                _selectedModel = null;
                _uiManager.SetSelectedModel(null);
                LoadUserModels();
            },
            onError: error => { Debug.LogError("Failed to delete model: " + error); }));
    }

    private void OnDisconnectClick()
    {
        LocalUserStorage.Clear();

        userMenuUI.SetActive(false);
        loginUI.SetActive(true);

        _selectedModel = null;
        _uiManager.SetSelectedModel(null);
    }

    private void OnCreateSessionClick()
    {
        Debug.Log("Current model: " + (_selectedModel == null ? "None" : _selectedModel.displayName));
        Debug.Log("Current session: " + (_selectedSession == null ? "None" : _selectedSession.id.ToString()));
        if (_selectedSession != null)
        {
            Debug.Log("Creating existing session...");
            NetworkManager.Singleton.StartHost();
            UploadService.Instance.LoadModel(_selectedSession.model.objectUrl);

            userMenuUI.SetActive(false);
            _uiManager.FetchPointAnnotations();
            return;
        }

        if (_selectedModel == null)
        {
            Debug.LogWarning("You must select a model to create a session.");
            return;
        }

        var user = LocalUserStorage.LoadUser();
        if (user == null)
        {
            Debug.LogError("No user logged in.");
            return;
        }

        Debug.Log("Creating new session...");
        var request = new SessionRequestDto
        {
            ownerId = user.id,
            modelId = _selectedModel.id,
            participantIds = new List<long>()
        };

        StartCoroutine(_sessionApi.CreateSession(request,
            onSuccess: session =>
            {
                _selectedSession = session;
                _uiManager.SetSelectedSession(session);

                NetworkManager.Singleton.StartHost();
                UploadService.Instance.LoadModel(_selectedModel.objectUrl);

                userMenuUI.SetActive(false);
            },
            onError: error => { Debug.LogError("Failed to create session: " + error); }));
    }

    private void OnJoinSessionClick()
    {
        string sessionCodeStr = _sessionCodeField.value.Trim();
        if (string.IsNullOrEmpty(sessionCodeStr))
        {
            Debug.LogWarning("Please enter a session code.");
            return;
        }

        if (!long.TryParse(sessionCodeStr, out long sessionId))
        {
            Debug.LogWarning("Session code must be a valid number.");
            return;
        }

        StartCoroutine(_sessionApi.GetSession(sessionId,
            onSuccess: session =>
            {
                NetworkManager.Singleton.StartClient();
                UploadService.Instance.LoadModel(session.model.objectUrl);

                userMenuUI.SetActive(false);
            },
            onError: error => { Debug.LogError("Failed to join session: " + error); }));
    }

    public void ToggleUserMenuUI(bool toggle)
    {
        userMenuUI.SetActive(toggle);
    }
}