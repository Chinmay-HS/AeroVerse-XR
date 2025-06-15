using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] public Button startARButton;

    void Start()
    {
        if (startARButton != null)
        {
            startARButton.onClick.AddListener(() =>
            {
                SwitchARtoMain.Instance.LoadARScene();
            });
        }
    }
}