using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartButtonVO : MonoBehaviour
{
    public Button startButton;               // Start button reference
    public AudioSource voAudio;              // VO AudioSource

    public GameObject[] objectsToEnable;     // UI/GameObjects to show after VO
    public GameObject[] objectsToDisable;    // UI/GameObjects to hide after VO

    void Start()
    {
        startButton.onClick.AddListener(PlayVO);
    }

    void PlayVO()
    {
        if (voAudio != null && !voAudio.isPlaying)
        {
            startButton.interactable = false;  // optional: prevent re-clicks
            voAudio.Play();
            StartCoroutine(HandleAfterVO());
          //  GameManager.Instance.ChangeExercise(ExerciseState.CubeInput);
           // GameManager.Instance.ChangeGameState(GameState.Exercise);

        }
    }

    IEnumerator HandleAfterVO()
    {
        yield return new WaitForSeconds(voAudio.clip.length);

        // Enable desired objects
        foreach (GameObject obj in objectsToEnable)
        {
            obj.SetActive(true);
        }

        // Disable others (e.g., the Start screen)
        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(false);
        }
    }
}
