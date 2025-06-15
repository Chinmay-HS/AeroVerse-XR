using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _authPanels;

    [SerializeField] private FirebaseManager _firebaseManager;

    [SerializeField] private Button _signUpButton, _logInButton, _forgotPasswordButton, _authLogInButton, _authSignUpButton;

    // Start is called before the first frame update
    void Start()
    {
        if (_firebaseManager == null) Debug.LogError("FirebaseAuthManager is not assigned!");
        if (_authPanels == null || _authPanels.Length == 0) Debug.LogError("authSections array is empty or not assigned!");

        AuthInitialiser();
    }

    private void AuthInitialiser()
    {
        
        _signUpButton.onClick.AddListener(() =>
        {
            SetActiveSection(1);
        });

        _logInButton.onClick.AddListener(() =>
        {
            SetActiveSection(0);
        });

        _authLogInButton.onClick.AddListener(_firebaseManager.Login);
        _authSignUpButton.onClick.AddListener(_firebaseManager.SignUp);
    }

    private void SetActiveSection(int index)
    {
        if (index < 0 || index >= _authPanels.Length)
        {
            Debug.LogError("SetActiveSection index is out of bounds!");
            return;
        }

        for (int i = 0; i < _authPanels.Length; i++)
        {
            _authPanels[i].SetActive(i == index);
        }
    }
}
