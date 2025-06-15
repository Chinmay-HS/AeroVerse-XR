using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GridImageManager : MonoBehaviour
{
    [Header("UI References")]
    public ScrollRect imageScrollView;
    public GridLayoutGroup gridLayoutGroup;
    public Transform imageContentParent;

    [Header("Tab References - Assign Your Existing Tabs")]
    public Button[] tabButtons;
    public string[] tabCategories;

    [Header("Image Data")]
    public List<ImageData> allImages = new List<ImageData>();

    [Header("Grid Settings")]
    public Vector2 imageSize = new Vector2(160f, 160f);
    public float spacing = 20f;
    public int columnsCount = 2;

    [Header("Label Settings")]
    public Font labelFont;
    public int labelFontSize = 20;
    public Color labelFontColor = Color.white;
    public TextAnchor labelAlignment = TextAnchor.MiddleCenter;

    private List<GameObject> imageGameObjects = new List<GameObject>();
    private string currentFilter = "All";
    private int activeTabIndex = 0;

    [System.Serializable]
    public class ImageData
    {
        public Sprite imageSprite;
        public string category;
        public string imageName;
    }

    void Start()
    {
        SetupUI();
        SetupExistingTabs();
        ShowAllImages();
        SetActiveTab(0);
    }

    void SetupUI()
    {
        if (gridLayoutGroup != null)
        {
            gridLayoutGroup.cellSize = new Vector2(imageSize.x, imageSize.y + 30f);
            gridLayoutGroup.spacing = Vector2.one * spacing;
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = columnsCount;
            gridLayoutGroup.childAlignment = TextAnchor.UpperCenter;
        }

        if (imageScrollView != null)
        {
            if (imageScrollView.horizontalScrollbar != null)
                imageScrollView.horizontalScrollbar.gameObject.SetActive(false);
            if (imageScrollView.verticalScrollbar != null)
                imageScrollView.verticalScrollbar.gameObject.SetActive(false);

            imageScrollView.horizontal = false;
            imageScrollView.vertical = true;
        }
    }

    void SetupExistingTabs()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int tabIndex = i;
            if (tabButtons[i] != null)
            {
                tabButtons[i].onClick.RemoveAllListeners();
                tabButtons[i].onClick.AddListener(() => OnTabClicked(tabIndex));
            }
        }
    }

    public void OnTabClicked(int tabIndex)
    {
        if (tabIndex >= 0 && tabIndex < tabCategories.Length)
        {
            string category = tabCategories[tabIndex];
            FilterImagesByCategory(category);
            SetActiveTab(tabIndex);
        }
    }

    public void FilterImagesByCategory(string category)
    {
        currentFilter = category;

        if (category == "All")
        {
            ShowAllImages();
        }
        else
        {
            ShowImagesByCategory(category);
        }

        if (imageScrollView != null)
        {
            Canvas.ForceUpdateCanvases();
            imageScrollView.verticalNormalizedPosition = 1f;
        }
    }

    void ShowAllImages()
    {
        ClearImageGrid();
        foreach (ImageData imageData in allImages)
        {
            CreateImageItem(imageData);
        }
        RefreshLayout();
    }

    void ShowImagesByCategory(string category)
    {
        ClearImageGrid();
        var filteredImages = allImages.Where(img => img.category.Equals(category, System.StringComparison.OrdinalIgnoreCase));
        foreach (ImageData imageData in filteredImages)
        {
            CreateImageItem(imageData);
        }
        RefreshLayout();
    }

    void CreateImageItem(ImageData imageData)
    {
        // Parent container
        GameObject container = new GameObject($"ImageItem_{imageData.imageName}");
        container.transform.SetParent(imageContentParent, false);
        VerticalLayoutGroup layoutGroup = container.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.spacing = 5f;
        layoutGroup.childControlHeight = false;
        layoutGroup.childControlWidth = false;

        ContentSizeFitter sizeFitter = container.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Image object
        GameObject imageObj = new GameObject("Image");
        imageObj.transform.SetParent(container.transform, false);
        Image imageComponent = imageObj.AddComponent<Image>();
        imageComponent.sprite = imageData.imageSprite;
        imageComponent.preserveAspect = true;
        imageComponent.type = Image.Type.Simple;
        RectTransform imageRT = imageObj.GetComponent<RectTransform>();
        imageRT.sizeDelta = imageSize;

        Button imageButton = imageObj.AddComponent<Button>();
        imageButton.onClick.AddListener(() => OnImageClicked(imageData));

        // Label object
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(container.transform, false);
        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = imageData.imageName;
        labelText.font = labelFont != null ? labelFont : Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelText.fontSize = labelFontSize;
        labelText.color = labelFontColor;
        labelText.alignment = labelAlignment;
        labelText.horizontalOverflow = HorizontalWrapMode.Wrap;
        labelText.verticalOverflow = VerticalWrapMode.Truncate;

        RectTransform labelRT = labelObj.GetComponent<RectTransform>();
        labelRT.sizeDelta = new Vector2(imageSize.x, 30f);

        imageGameObjects.Add(container);
    }

    void ClearImageGrid()
    {
        foreach (GameObject obj in imageGameObjects)
        {
            if (obj != null)
                DestroyImmediate(obj);
        }
        imageGameObjects.Clear();
    }

    void RefreshLayout()
    {
        Canvas.ForceUpdateCanvases();
        if (gridLayoutGroup != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(gridLayoutGroup.GetComponent<RectTransform>());
        }

        ContentSizeFitter contentSizeFitter = imageContentParent.GetComponent<ContentSizeFitter>();
        if (contentSizeFitter != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(imageContentParent.GetComponent<RectTransform>());
        }
    }

    void SetActiveTab(int activeIndex)
    {
        activeTabIndex = activeIndex;
        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (tabButtons[i] != null)
            {
                UpdateTabVisual(tabButtons[i], i == activeIndex);
            }
        }
    }

    void UpdateTabVisual(Button tabButton, bool isActive)
    {
        Image tabImage = tabButton.GetComponent<Image>();
        Text tabText = tabButton.GetComponentInChildren<Text>();

        if (isActive)
        {
            if (tabImage != null)
                tabImage.color = new Color(0.2f, 0.6f, 1f, 1f);
            if (tabText != null)
                tabText.color = Color.white;
        }
        else
        {
            if (tabImage != null)
                tabImage.color = Color.white;
            if (tabText != null)
                tabText.color = Color.black;
        }
    }

    void OnImageClicked(ImageData imageData)
    {
        Debug.Log($"Clicked on image: {imageData.imageName} from category: {imageData.category}");
    }

    public void RefreshCurrentView()
    {
        FilterImagesByCategory(currentFilter);
    }

    public void AddImage(Sprite sprite, string category, string name)
    {
        ImageData newImage = new ImageData
        {
            imageSprite = sprite,
            category = category,
            imageName = name
        };

        allImages.Add(newImage);
        RefreshCurrentView();
    }

    public void RemoveImage(string imageName)
    {
        allImages.RemoveAll(img => img.imageName == imageName);
        RefreshCurrentView();
    }
}
