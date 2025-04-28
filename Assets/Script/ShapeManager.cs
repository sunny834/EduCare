using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShapeManager : MonoBehaviour
{
    public RectTransform previewShape;
    public Button cubeButton, cuboidButton, nextButton;
    public GameObject cubePanel, cuboidPanel;
    public TMP_InputField sideInput, lengthInput, breadthInput, heightInput;

    private string selectedShape = "";

    void Start()
    {
        cubeButton.onClick.AddListener(() => SelectShape("Cube"));
        cuboidButton.onClick.AddListener(() => SelectShape("Cuboid"));

        sideInput.onValueChanged.AddListener(delegate { OnInputChanged(); });
        lengthInput.onValueChanged.AddListener(delegate { OnInputChanged(); });
        breadthInput.onValueChanged.AddListener(delegate { OnInputChanged(); });
        heightInput.onValueChanged.AddListener(delegate { OnInputChanged(); });

        nextButton.interactable = false;
        previewShape.gameObject.SetActive(false);
    }

    void SelectShape(string shape)
    {
        selectedShape = shape;
        cubePanel.SetActive(shape == "Cube");
        cuboidPanel.SetActive(shape == "Cuboid");

        previewShape.gameObject.SetActive(true);
        ResetPreviewSize();
        OnInputChanged();
    }

    void OnInputChanged()
    {
        bool valid = false;

        if (selectedShape == "Cube" && float.TryParse(sideInput.text, out float a) && a > 0)
        {
            previewShape.sizeDelta = new Vector2(a * 100, a * 100); // scale for visibility
            valid = true;
        }
        else if (selectedShape == "Cuboid" &&
                 float.TryParse(lengthInput.text, out float l) && l > 0 &&
                 float.TryParse(heightInput.text, out float h) && h > 0)
        {
            previewShape.sizeDelta = new Vector2(l * 100, h * 100);
            valid = true;
        }

        nextButton.interactable = valid;
    }

    void ResetPreviewSize()
    {
        previewShape.sizeDelta = new Vector2(100, 100);
    }
}
