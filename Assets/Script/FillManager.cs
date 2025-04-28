using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class AreaObject
{
    public GameObject obj;
    public float volume;
}

public class FillManager : MonoBehaviour
{
    [Header("UI References")]
    public Slider fillSlider;
    public Image TapWater;
    public Image firstFillImage;
    public Image secondFillImage;
    public GameObject glowEffect;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI sliderValueText;
    public Image pausePlayButtonImage; // Reference to the pause/play button image
    public Sprite pauseSprite; // Sprite for pause
    public Sprite playSprite;  // Sprite for play

    [Header("Dimension Inputs")]
    public TMP_InputField sideInputField;
    public TMP_InputField lengthInputField;
    public TMP_InputField breadthInputField;
    public TMP_InputField heightInputField;
    public GameObject cubeInputPanel;
    public GameObject cuboidInputPanel;

    [Header("Volume Settings")]
    public List<AreaObject> areaObjects = new List<AreaObject>();
    public float totalVolume = 20f;

    [Header("Fill Settings")]
    public float fillDuration = 3f;

    private bool isFilling = false;
    private Coroutine fillCoroutine;
    private bool isPaused = false;
    private bool isCube = true;
    private int currentAreaObjectIndex = 0;
    private bool lockSlider = false;
    

    private void Start()
    {
        SetupSlider();
        fillSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
       // SetupShapeUI();
    }

    private void SetupSlider()
    {
        fillSlider.minValue = 0f;
        fillSlider.maxValue = totalVolume;
        fillSlider.value = 0f;
        UpdateFillImages();
    }

    private void OnSliderValueChanged()
    {
        if (!lockSlider && !isPaused)
        {
            // Snap slider to nearest 5
            float snappedValue = Mathf.Round(fillSlider.value / 5f) * 5f;
            fillSlider.value = snappedValue;
            UpdateFillImages();
        }
    }

    private void UpdateFillImages()
    {
        float percentFilled = fillSlider.value / totalVolume;
        firstFillImage.fillAmount = percentFilled;
        sliderValueText.text = fillSlider.value.ToString("F0");

        bool matchFound = false;

        if (currentAreaObjectIndex >= 0 && currentAreaObjectIndex < areaObjects.Count)
        {
            if (Mathf.Approximately(fillSlider.value, areaObjects[currentAreaObjectIndex].volume))
            {
                matchFound = true;
               // isFilling = true;
            }
        }

        if (fillCoroutine != null)
            StopCoroutine(fillCoroutine);

        if (matchFound)
        {
            TapWater.gameObject.SetActive(true);
            messageText.text = "Success!";
            Debug.Log("? Exact Volume Matched!");

            // Start filling animation, but do not trigger glow yet
            isFilling = true;
            fillCoroutine = StartCoroutine(SmoothFillSecondImage(fillDuration, 1f, () =>
            {
                // Glow effect only occurs after the fill is completed
                glowEffect.SetActive(true);
            }));

            LockSlider();
            StartCoroutine(NextAreaObjectAfterDelay(5f));
        }
        else
        {   isFilling=false;
            TapWater.gameObject.SetActive(false);
            messageText.text = "Try Again!";
            Debug.Log("? No Match, Keep Trying!");
            glowEffect.SetActive(false);
            fillCoroutine = StartCoroutine(SmoothFillSecondImage(fillDuration, 0f));
        }
    }

    private IEnumerator SmoothFillSecondImage(float duration, float targetFill, System.Action onComplete = null)
    {
        float startFill = secondFillImage.fillAmount;
        float elapsed = 0f;

        while (elapsed < duration && !isPaused)
        {
            elapsed += Time.deltaTime;
            secondFillImage.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / duration);
            yield return null;
        }

        secondFillImage.fillAmount = targetFill;

        // Once fill is completed, trigger the callback to activate the glow effect
        if (onComplete != null)
            onComplete.Invoke();
    }

    private void LockSlider()
    {
        lockSlider = true;
        fillSlider.interactable = false;  // Disable the slider
    }

    private IEnumerator NextAreaObjectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentAreaObjectIndex++;
        if (currentAreaObjectIndex >= areaObjects.Count)
        {
            messageText.text = "All Exercises Completed!";
            Debug.Log("No more area objects.");
            fillSlider.interactable = false;
        }
        else
        {
            
                lockSlider = false;
                fillSlider.interactable = true;  // Enable the slider again
                fillSlider.value = 0f;
                UpdateFillImages();
                // SetupShapeUI();
            
        }
    }

    public void TogglePausePlay()
    {
        isPaused = !isPaused;

        // Toggle sprite for pause/play button
        if (isPaused)
        {
            // Pause state
            pausePlayButtonImage.sprite = playSprite;
            StopCoroutine(fillCoroutine);
            messageText.text = "Paused";
            Debug.Log("? Paused");
        }
        else
        {
            // Play state
            pausePlayButtonImage.sprite = pauseSprite;
            messageText.text = "Resumed";
            Debug.Log("?? Resumed");
            if (isFilling)
            {
                
                fillCoroutine = StartCoroutine(SmoothFillSecondImage(fillDuration - (secondFillImage.fillAmount * fillDuration), 1f)); // Resume from current fillAmount
            }
        }
    }

    // ---------------------- Shape Selection -----------------------

    //private void SetupShapeUI()
    //{
    //    if (isCube)
    //    {
    //        cubeInputPanel.SetActive(true);
    //        cuboidInputPanel.SetActive(false);
    //    }
    //    else
    //    {
    //        cubeInputPanel.SetActive(false);
    //        cuboidInputPanel.SetActive(true);
    //    }
    //}

    //public void SetShapeCube()
    //{
    //    isCube = true;
    //    SetupShapeUI();
    //}

    //public void SetShapeCuboid()
    //{
    //    isCube = false;
    //    SetupShapeUI();
    //}

    public void CalculateVolume()
    {
        float volume = 0f;

        if (isCube)
        {
            if (float.TryParse(sideInputField.text, out float side))
            {
                volume = side * side * side;
            }
        }
        else
        {
            if (float.TryParse(lengthInputField.text, out float l) &&
                float.TryParse(breadthInputField.text, out float b) &&
                float.TryParse(heightInputField.text, out float h))
            {
                volume = l * b * h;
            }
        }

        Debug.Log("Calculated Volume = " + volume);

        if (currentAreaObjectIndex >= 0 && currentAreaObjectIndex < areaObjects.Count)
        {
            areaObjects[currentAreaObjectIndex].volume = volume;
        }
    }

    public void ResetPouring()
    {
        // Reset Slider
        fillSlider.value = 0f;
        fillSlider.interactable = true;
        lockSlider = false;

        // Reset Images
        firstFillImage.fillAmount = 0f;
        secondFillImage.fillAmount = 0f;

        // Hide Glow
        glowEffect.SetActive(false);
        TapWater.gameObject.SetActive(false);

        // Reset Text
        messageText.text = "";
        sliderValueText.text = "0";

        // Stop any fill coroutine if running
        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
        }

        //// Reverse Pour Animation
        //StartCoroutine(SmoothFillSecondImage(fillDuration, 3f));
        currentAreaObjectIndex = 0;
        isFilling = false;
        isPaused = false;

        Debug.Log("Pouring Reset!");
    }
}
