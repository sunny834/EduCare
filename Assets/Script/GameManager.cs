using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static event Action<GameState> OnGameStateChanged;
    public static event Action<ExerciseState> OnExerciseChanged;

    private GameState currentGameState;
    private ExerciseState currentExercise;

    public GameState CurrentGameState => currentGameState;
    public ExerciseState CurrentExercise => currentExercise;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ChangeGameState(GameState.Menu);
        ChangeExercise(ExerciseState.None);
    }

    public void ChangeGameState(GameState newState)
    {
        if (currentGameState == newState) return;
        currentGameState = newState;
        Debug.Log("Game State changed to: " + newState);
        OnGameStateChanged?.Invoke(newState);
    }

    public void ChangeExercise(ExerciseState newExercise)
    {
        if (currentExercise == newExercise) return;
        currentExercise = newExercise;
        Debug.Log("Exercise changed to: " + newExercise);
        OnExerciseChanged?.Invoke(newExercise);
    }
}
