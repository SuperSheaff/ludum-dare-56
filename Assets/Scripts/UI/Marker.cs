using UnityEngine;

public class Marker : MonoBehaviour
{
    private Animator animator; // Reference to the Animator component

    private void Awake()
    {
        // Grab the Animator component on the object
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component missing on Marker!");
        }
    }

    // Function to enable or disable the marker (for idle state)
    public void SetEnabled(bool isEnabled)
    {
        if (animator != null)
        {
            animator.SetBool("idle", isEnabled);
        }
    }

    // Function to set the hover state
    public void SetHover(bool isHovered)
    {
        if (animator != null)
        {
            animator.SetBool("hover", isHovered);
        }
    }
}
