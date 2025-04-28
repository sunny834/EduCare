using UnityEngine;

public class Animation : MonoBehaviour
{
    private Animator animator;  // Reference to the Animator component

    // Start is called before the first frame update
    void Start()
    {
        // Get the Animator component attached to the GameObject
        animator = GetComponent<Animator>();

        
    }
    public void play()
    {
        // Play the animation on start
        if (animator != null)
        {
            animator.SetTrigger("PLay"); // Replace with your animation name
        }
        else
        {
            Debug.LogError("Animator component not found!");
        }
    }
}
