using UnityEngine;
using UnityEngine.UI;

public class ARSceneController : MonoBehaviour
{
    [SerializeField] public Button returnHomeButton;

    void Start()
    {
        if (returnHomeButton != null)
        {
            returnHomeButton.onClick.AddListener(() =>
            {
                SwitchARtoMain.Instance.ReturnToHome();
            });
        }
    }
}