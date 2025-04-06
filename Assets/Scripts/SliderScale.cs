using UnityEngine;
using UnityEngine.UI;

public class SliderScaler : MonoBehaviour
{
    public Transform targetObject;      // The object to scale
    public Slider scaleSlider;          // UI Slider

    private float initialScale = 1f;
    private bool isUpdatingSlider = false;

    void Start()
    {
        if (targetObject != null)
        {
            initialScale = targetObject.localScale.x;
        }

        if (scaleSlider != null)
        {
            scaleSlider.onValueChanged.AddListener(OnSliderChanged);
            scaleSlider.value = 1f; // Initialize to default scale
        }
    }

    void Update()
    {
        // Keep slider synced with pinch scaling
        if (!isUpdatingSlider && scaleSlider != null && targetObject != null)
        {
            float scaleRatio = targetObject.localScale.x / initialScale;
            scaleSlider.value = Mathf.Clamp(scaleRatio, scaleSlider.minValue, scaleSlider.maxValue);
        }
    }

    private void OnSliderChanged(float value)
    {
        if (targetObject != null)
        {
            isUpdatingSlider = true;

            // Uniform scale across all axes
            targetObject.localScale = Vector3.one * (initialScale * value);

            isUpdatingSlider = false;
        }
    }

    private void OnDestroy()
    {
        if (scaleSlider != null)
        {
            scaleSlider.onValueChanged.RemoveListener(OnSliderChanged);
        }
    }
}