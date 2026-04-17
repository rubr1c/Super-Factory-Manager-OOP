using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Presentation.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class AuthMenuUI : MonoBehaviour
    {
        private const string GameplaySceneName = "SampleScene";

        private TextField _emailField;
        private TextField _passwordField;
        private Button _signInButton;
        private Button _signUpButton;
        private Label _statusLabel;

        private FirebaseAuth _auth;
        private bool _ready;
        private bool _loadingScene;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _emailField = root.Q<TextField>("email-field");
            _passwordField = root.Q<TextField>("password-field");
            _signInButton = root.Q<Button>("sign-in-button");
            _signUpButton = root.Q<Button>("sign-up-button");
            _statusLabel = root.Q<Label>("status-label");

            _signInButton.clicked += OnSignInClicked;
            _signUpButton.clicked += OnSignUpClicked;

            SetInteractable(false);
            SetStatus("Loading...");
        }

        private void Start()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    SetStatus(task.Exception?.GetBaseException().Message ?? "Auth setup failed.");
                    return;
                }

                if (task.Result != DependencyStatus.Available)
                {
                    SetStatus("Auth is unavailable.");
                    return;
                }

                _auth = FirebaseAuth.DefaultInstance;

                if (_auth.CurrentUser != null && _auth.CurrentUser.IsValid())
                {
                    LoadGameplay();
                    return;
                }

                _ready = true;
                SetInteractable(true);
                SetStatus(string.Empty);
            });
        }

        private void OnDestroy()
        {
            if (_signInButton != null) _signInButton.clicked -= OnSignInClicked;
            if (_signUpButton != null) _signUpButton.clicked -= OnSignUpClicked;
        }

        private void OnSignInClicked()
        {
            Submit(signUp: false);
        }

        private void OnSignUpClicked()
        {
            Submit(signUp: true);
        }

        private void Submit(bool signUp)
        {
            if (!_ready || _auth == null) return;

            var email = _emailField.value?.Trim() ?? string.Empty;
            var password = _passwordField.value ?? string.Empty;

            if (!Validate(email, password)) return;

            SetInteractable(false);
            SetStatus(signUp ? "Creating account..." : "Signing in...");

            var request = signUp
                ? _auth.CreateUserWithEmailAndPasswordAsync(email, password)
                : _auth.SignInWithEmailAndPasswordAsync(email, password);

            request.ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    SetStatus("Request cancelled.");
                    SetInteractable(true);
                    return;
                }

                if (task.IsFaulted)
                {
                    SetStatus(task.Exception?.GetBaseException().Message ?? "Request failed.");
                    SetInteractable(true);
                    return;
                }

                LoadGameplay();
            });
        }

        private bool Validate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                SetStatus("Enter email and password.");
                return false;
            }

            if (password.Length < 6)
            {
                SetStatus("Password must be at least 6 characters.");
                return false;
            }

            return true;
        }

        private void LoadGameplay()
        {
            if (_loadingScene) return;

            var user = _auth?.CurrentUser;
            if (user == null || !user.IsValid())
            {
                SetStatus("Auth failed.");
                SetInteractable(true);
                return;
            }

            _loadingScene = true;
            SceneManager.LoadScene(GameplaySceneName);
        }

        private void SetInteractable(bool interactable)
        {
            if (_signInButton != null) _signInButton.SetEnabled(interactable);
            if (_signUpButton != null) _signUpButton.SetEnabled(interactable);
            if (_emailField != null) _emailField.SetEnabled(interactable);
            if (_passwordField != null) _passwordField.SetEnabled(interactable);
        }

        private void SetStatus(string message)
        {
            if (_statusLabel != null) _statusLabel.text = message;
        }
    }
}
