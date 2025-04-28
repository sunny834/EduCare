using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class VolumePracticeTMP : MonoBehaviour
{
    public List<TMP_InputField> inputFields;      // Assign all input fields
    public List<int> correctAnswers;               // Correct answers
    public List<GameObject> questions;             // Drag your question panels here (SerializedField)
    public TextMeshProUGUI feedbackText;
    public Image feedbackIcon;
    public Sprite correctSprite;
    public Sprite incorrectSprite;
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip incorrectSound;

    private int currentInputFieldIndex = 0;
    private int currentQuestionIndex = 0;

    private void Start()
    {
        SelectInputField(0);
        ShowQuestion(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Backspace();
        }
    }

    public void SelectInputField(int index)
    {
        currentInputFieldIndex = index;
    }

    public void NumberButtonPressed(string number)
    {
        if (inputFields.Count > currentInputFieldIndex)
        {
            inputFields[currentInputFieldIndex].text += number;
            CheckInput(inputFields[currentInputFieldIndex].text, currentInputFieldIndex);
        }
    }

    public void ClearInput()
    {
        if (inputFields.Count > currentInputFieldIndex)
        {
            inputFields[currentInputFieldIndex].text = "";
        }
    }

    public void Backspace()
    {
        if (inputFields.Count > currentInputFieldIndex)
        {
            TMP_InputField field = inputFields[currentInputFieldIndex];
            if (field.text.Length > 0)
            {
                field.text = field.text.Substring(0, field.text.Length - 1);
            }
        }
    }

    private void CheckInput(string userInput, int index)
    {
        if (int.TryParse(userInput, out int userAnswer))
        {
            if (userAnswer == correctAnswers[index])
            {
                feedbackIcon.sprite = correctSprite;
                feedbackText.text = "Well done!";
                audioSource.PlayOneShot(correctSound);

                // Move to next question
                Invoke(nameof(GoToNextQuestion), 5.0f); // Wait 1 second after correct
            }
            else
            {
                feedbackIcon.sprite = incorrectSprite;
                feedbackText.text = "Try again!";
                audioSource.PlayOneShot(incorrectSound);
            }
        }
        else
        {
            feedbackText.text = "Please enter a valid number!";
        }
    }

    private void GoToNextQuestion()
    {
        if (questions.Count > currentQuestionIndex)
        {
            questions[currentQuestionIndex].SetActive(false); // Disable current
            currentQuestionIndex++;

            if (currentQuestionIndex < questions.Count)
            {
                questions[currentQuestionIndex].SetActive(true); // Enable next
            }
            else
            {
                Debug.Log("All questions completed!");
                feedbackText.text = "Congratulations!";
                // Maybe you can show a Final Panel here too
            }
        }
    }
    public void OnInputFieldChanged(string value)
    {
        if (inputFields.Count > currentInputFieldIndex)
        {
            CheckInput(value, currentInputFieldIndex);
        }
    }

    private void ShowQuestion(int index)
    {
        // Make sure only the selected question is active at start
        for (int i = 0; i < questions.Count; i++)
        {
            questions[i].SetActive(i == index);
        }
    }
}
