using UnityEngine;
using UnityEngine.UIElements;

public class LoginUIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject loginUI;

    private UIDocument _loginUIDocument;
    private TextField _usernameText;
    private TextField _passwordText;
    private Button _loginButton;

    private UserApi _userApi;
    private UIManager _uiManager;

    public void Initialize(UserApi userApi, UIManager uiManager)
    {
        _userApi = userApi;
        _uiManager = uiManager;
        SetupUI();
    }

    private void SetupUI()
    {
        _loginUIDocument = loginUI.GetComponent<UIDocument>();

        _loginButton = _loginUIDocument.rootVisualElement.Q("login-button") as Button;
        _loginButton!.clicked += OnLoginClick;
        
        _usernameText = _loginUIDocument.rootVisualElement.Q("username-field") as TextField;
        _passwordText = _loginUIDocument.rootVisualElement.Q("password-field") as TextField;
    }

    private void OnLoginClick()
    {
        string username = _usernameText.value;
        string password = _passwordText.value;
        
        UserRequestDto userRequest = new UserRequestDto 
        { 
            username = username, 
            password = password 
        };

        StartCoroutine(_userApi.Login(userRequest,
            onSuccess: user =>
            {
                LocalUserStorage.SaveUser(user);
                HideLogin();
                
                OnLoginSuccess(user);
            },
            onError: error => 
            { 
                Debug.LogError("Login failed: " + error); 
            }));
    }

    public void OnLoginSuccess(UserResponseDto user)
    {
        _uiManager.GetUserMenuUIManager().ToggleUserMenuUI(true);
    
        _uiManager.GetUserMenuUIManager().SetUsernameLabel(user.username);
    
        _uiManager.GetUserMenuUIManager().LoadUserModels();
        _uiManager.GetUserMenuUIManager().LoadUserSessions();
    }

    public void ShowLogin()
    {
        loginUI.SetActive(true);
    }

    public void HideLogin()
    {
        loginUI.SetActive(false);
    }
}