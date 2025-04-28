using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PouringExerciseManager : MonoBehaviour
{
    public AudioSource pouringAudioSource;
    public TextMeshProUGUI pouringCompleteText;
    public void Start()
    {
        if (pouringAudioSource == null) Debug.LogError("Pouring Audio Source is NOT assigned!");
        if (pouringCompleteText == null) Debug.LogError("Pouring Complete Text is NOT assigned!");
    }
     
    private void OnEnable()
    {
        GameManager.OnExerciseChanged += HandleExerciseChange;
        Debug.Log("heyyy");
    }

    private void OnDisable()
    {
        GameManager.OnExerciseChanged -= HandleExerciseChange;
    }

    private void HandleExerciseChange(ExerciseState state)
    {
        if (state == ExerciseState.PouringExercise)
        {
            StartCoroutine(PouringRoutine());
        }
    }

    private IEnumerator PouringRoutine()
    {
        pouringCompleteText.gameObject.SetActive(false); // Hide text first
        yield return new WaitForSeconds(7.5f); // 3 seconds delay
        pouringAudioSource.Play(); // Play pouring audio
        pouringCompleteText.gameObject.SetActive(true); // Enable text
    }
    public void StartPouringExercise()
    {
        GameManager.Instance.ChangeExercise(ExerciseState.PouringExercise);
        GameManager.Instance.ChangeGameState(GameState.Exercise); // still exercise state
    }
}
