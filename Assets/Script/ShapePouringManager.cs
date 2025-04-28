using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class VolumeObject
{
    public GameObject obj;
    public float volume;
}

public class ShapePouringManager : MonoBehaviour
{
    [Header("UI References")]
    public Slider fillSlider;
    public Image firstFillImage;
    public Image secondFillImage_Cube;
    public Image secondFillImage_Cuboid;
    public GameObject cubePrefab;
    public GameObject cuboidPrefab;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI sliderValueText;
    public Button pausePlayButton;
    public Sprite pauseSprite;
    public Sprite playSprite;
    public Image TapWater;

    [Header("Input Fields")]
    public TMP_InputField sideInputField;     // For Cube
    public TMP_InputField lengthInputField;   // For Cuboid
    public TMP_InputField breadthInputField;
    public TMP_InputField heightInputField;
    public TextMeshProUGUI Volume;

    [Header("Settings")]
    public int totalVolume = 100;
    public float fillDuration = 3f;
    public float glowPulseDuration = 1f;
    public int glowPulseCount = 2;

    private bool isCube = true;
    private bool isPaused = false;
    private bool lockSlider = false;
    private bool isFilling = false;
    private Coroutine fillCoroutine;
    private int currentAreaObjectIndex = 0;

    private void Start()
    {
        SetupSlider();
        fillSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
        pausePlayButton.onClick.AddListener(TogglePausePlay);
        ShowShapeUI();
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
        float expectedVolume = CalculateExpectedVolume();
        Volume.text = expectedVolume.ToString("000");
        if (Mathf.Approximately(fillSlider.value, expectedVolume))
        {
            matchFound = true;
            TapWater.gameObject.SetActive(true);
        }

        if (fillCoroutine != null)
            StopCoroutine(fillCoroutine);

        if (matchFound)
        {
            messageText.text = "Success!";
            Debug.Log("Success Match!");

            // Smoothly fill second image based on Cube or Cuboid
            fillCoroutine = StartCoroutine(SmoothFillSecondImage(1f, () =>
            {
                StartCoroutine(BreathingGlowFillImage(glowPulseCount, glowPulseDuration));
            }));

            LockSlider();
        }
        else
        {
            messageText.text = "Try Again!";
            Debug.Log("Try Again");
            TapWater.gameObject.SetActive(false);
            fillCoroutine = StartCoroutine(SmoothFillSecondImage(0f));
        }
    }

    private float CalculateExpectedVolume()
    {
        if (isCube)
        {
            if (float.TryParse(sideInputField.text, out float side))
            {
                return side * side * side;
            }
        }
        else
        {
            if (float.TryParse(lengthInputField.text, out float l) &&
                float.TryParse(breadthInputField.text, out float b) &&
                float.TryParse(heightInputField.text, out float h))
            {
                return l * b * h;
            }
        }
        return 0f;
    }

    private IEnumerator SmoothFillSecondImage(float targetFill, System.Action onComplete = null)
    {
        Image secondImage = isCube ? secondFillImage_Cube : secondFillImage_Cuboid;

        float startFill = secondImage.fillAmount;
        float elapsed = 0f;

        while (elapsed < fillDuration)
        {
            elapsed += Time.deltaTime;
            secondImage.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / fillDuration);
            yield return null;
        }

        secondImage.fillAmount = targetFill;

        if (onComplete != null)
        {
            TapWater.gameObject.SetActive(false);
            onComplete.Invoke();
        }
    }

    private void LockSlider()
    {
        lockSlider = true;
    }

    private IEnumerator NextShapeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reset for next
        lockSlider = false;
        fillSlider.value = 0f;
        sliderValueText.text = "0";
        ResetSecondFillImages();
        messageText.text = "";

        ShowShapeUI();
    }

    private void ResetSecondFillImages()
    {
        // Reset the fill amount of the second images for both cube and cuboid
        secondFillImage_Cube.fillAmount = 0f;
        secondFillImage_Cuboid.fillAmount = 0f;
    }

    private void TogglePausePlay()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pausePlayButton.image.sprite = playSprite;
        }
        else
        {
            Time.timeScale = 1f;
            pausePlayButton.image.sprite = pauseSprite;
        }
    }

    private void ShowShapeUI()
    {
        if (isCube)
        {
            cubePrefab.SetActive(true);
            cuboidPrefab.SetActive(false);
        }
        else
        {
            cubePrefab.SetActive(false);
            cuboidPrefab.SetActive(true);
        }
    }

    public void SetCubeMode()
    {
        isCube = true;
        ShowShapeUI();
    }

    public void SetCuboidMode()
    {
        isCube = false;
        ShowShapeUI();
    }

    public void ResetPouring()
    {
        if (fillCoroutine != null)
            StopCoroutine(fillCoroutine);

        fillSlider.value = 0f;
        lockSlider = false;
        isPaused = false;
        isFilling = false;
        ResetSecondFillImages();
        messageText.text = "";
        sliderValueText.text = "0";
        TapWater.gameObject.SetActive(false);

        // Clear all input fields
        sideInputField.text = "";
        lengthInputField.text = "";
        breadthInputField.text = "";
        heightInputField.text = "";
        Volume.text = "000";

        // Reset First Fill Image
        firstFillImage.fillAmount = 0f;

        // Reset UI
        cubePrefab.SetActive(false);
        cuboidPrefab.SetActive(false);
    }

    private IEnumerator BreathingGlowFillImage(int pulseCount, float singlePulseDuration)
    {
        Image targetImage = firstFillImage;
        Color originalColor = targetImage.color;
        Color glowColor = new Color(1f, 1f, 0.5f); // Light yellow glow

        for (int i = 0; i < pulseCount; i++)
        {
            float elapsed = 0f;
            while (elapsed < singlePulseDuration / 2f)
            {
                elapsed += Time.deltaTime;
                targetImage.color = Color.Lerp(originalColor, glowColor, elapsed / (singlePulseDuration / 2f));
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < singlePulseDuration / 2f)
            {
                elapsed += Time.deltaTime;
                targetImage.color = Color.Lerp(glowColor, originalColor, elapsed / (singlePulseDuration / 2f));
                yield return null;
            }
        }

        targetImage.color = originalColor; // Restore final color
    }
}
