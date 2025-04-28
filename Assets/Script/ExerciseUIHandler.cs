using UnityEngine;

public class ExerciseUIHandler : MonoBehaviour
{
    public ExerciseState thisExercise;

    void OnEnable()
    {
        GameManager.OnExerciseChanged += HandleExerciseChange;
    }

    void OnDisable()
    {
        GameManager.OnExerciseChanged -= HandleExerciseChange;
    }

    void HandleExerciseChange(ExerciseState state)
    {
        gameObject.SetActive(state == thisExercise);
    }
}
