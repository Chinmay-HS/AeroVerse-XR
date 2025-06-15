using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirebaseManager : MonoBehaviour
{
    private FirebaseApp _app;
    private FirebaseAuth _auth;
    private FirebaseUser _user;
    private FirebaseFirestore _firestore;
    private CollectionReference _users;

    // Auth Fields
    [SerializeField] private TMP_InputField emailText, passwordText, usernameSignUp, emailSignUp, passwordSignUp, _emailInputResetPassword;

    void Start()
    {
        // Firebase Initialisation
        _app = FirebaseApp.DefaultInstance;
        _auth = FirebaseAuth.DefaultInstance;
        _firestore = FirebaseFirestore.DefaultInstance;
        _users = _firestore.Collection("users");

        if (_auth != null && _firestore != null && _app != null)
        {
            Debug.Log("Firebase Initialised Successfully!");
        }
        else
        {
            Debug.LogError("Error Initialising Firebase");
        }
    }

    public void Login()
    {
        string email = emailText.text.Trim();
        string password = passwordText.text;

        _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted || !task.IsCompleted)
            {
                Debug.LogError("Error signing in user: " + task.Exception?.Flatten().Message);
            }
            else
            {
                _user = task.Result.User;
                Debug.Log("Authentication Successful: " + _user.UserId);
                SceneManager.LoadScene("MainMenuScene");
            }
        });
    }

    public void SignUp()
    {
        string email = emailSignUp.text.Trim();
        string password = passwordSignUp.text.Trim();
        string username = usernameSignUp.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
        {
            Debug.Log("Please fill the mandatory fields...");
            return;
        }

        if (password.Length < 6)
        {
            Debug.Log("Password needs to be at least 6 characters...");
            return;
        }

        _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread((task) =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Error: " + task.Exception?.Flatten().Message);
                return;
            }

            _user = task.Result.User;
            SaveUserToFirestore(_user.UserId, username, _user.Email);
        });
    }

    public void ForgotPassword()
    {
        string email = _emailInputResetPassword.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            Debug.Log("Enter your email to reset password.");
            return;
        }

        _auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("Failed to send reset link.");
                return;
            }

            Debug.Log("Reset link sent. Check your email.");
        });
    }

    public void LogOut()
    {
        if (_auth.CurrentUser != null)
        {
            _auth.SignOut();
            Debug.Log("User logged out successfully.");
            SceneManager.LoadScene("LogIn");
        }
        else
        {
            Debug.LogWarning("No user is currently logged in.");
        }
    }

    private void SaveUserToFirestore(string userId, string username, string email)
    {
        UserData userData = new UserData
        {
            userId = userId,
            username = username,
            email = email,
            createdAt = FieldValue.ServerTimestamp.ToString()
        };

        Dictionary<string, object> userDict = new Dictionary<string, object>
        {
            { "userId", userData.userId },
            { "username", userData.username },
            { "email", userData.email },
            { "createdAt", FieldValue.ServerTimestamp }
        };

        _users.Document(userId).SetAsync(userDict).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("User saved to Firestore!");
                SceneManager.LoadScene("MainMenuScene");
            }
            else
            {
                Debug.LogError("Error saving user to Firestore: " + task.Exception?.Flatten().Message);
            }
        });
    }

    // Struct with all string fields
    private struct UserData
    {
        public string userId;
        public string username;
        public string email;
        public string createdAt;
    }
}
