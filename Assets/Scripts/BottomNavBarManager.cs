using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BottomNavBarManager : MonoBehaviour
{
    [System.Serializable]
    public class TabData
    {
        public string tabName;
        public Button tabButton;
        public string sceneToLoad;
    }

    public List<TabData> tabs;
    public int defaultActiveTab = 0;

    private int currentIndex = -1;

    // Shared across scenes
    private static int lastActiveTabIndex = -1;

    void Start()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            int index = i;
            tabs[i].tabButton.onClick.RemoveAllListeners();
            tabs[i].tabButton.onClick.AddListener(() => OnTabClicked(index));
        }

        // Use last known active tab across scenes, or fallback to default
        if (lastActiveTabIndex >= 0 && lastActiveTabIndex < tabs.Count)
        {
            HighlightTab(lastActiveTabIndex);
            currentIndex = lastActiveTabIndex;
        }
        else
        {
            HighlightTab(defaultActiveTab);
            currentIndex = defaultActiveTab;
        }
    }

    void OnTabClicked(int index)
    {
        if (index == currentIndex) return;

        currentIndex = index;
        lastActiveTabIndex = index;

        HighlightTab(index);

        string sceneName = tabs[index].sceneToLoad;
        if (!string.IsNullOrEmpty(sceneName) && sceneName != SceneManager.GetActiveScene().name)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    void HighlightTab(int index)
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            var tabButton = tabs[i].tabButton;
            var image = tabButton.GetComponent<Image>();
            var text = tabButton.GetComponentInChildren<Text>();

            if (image != null)
                image.color = (i == index) ? new Color(0.2f, 0.6f, 1f, 1f) : Color.white;

            if (text != null)
                text.color = (i == index) ? Color.white : Color.black;
        }
    }
}
