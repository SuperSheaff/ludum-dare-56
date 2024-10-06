using UnityEngine;
using UnityEngine.Events;

public class CustomClickable : MonoBehaviour
{
    // UnityEvent for custom actions when clicked
    public UnityEvent onClick;

    // Sprites for normal and hover states
    public Sprite hoverSprite;  // Sprite to display on hover
    private Sprite originalSprite;  // Store the original sprite
    private bool isInteractable = true;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Store the original sprite
            originalSprite = spriteRenderer.sprite;
        }
        else
        {
            Debug.LogError("No SpriteRenderer found on this object!");
        }
    }

    // Called when the mouse pointer enters the object’s bounds
    private void OnMouseEnter()
    {
        if (!isInteractable || hoverSprite == null || spriteRenderer == null) return;

        // Change the sprite to the hover sprite on hover
        spriteRenderer.sprite = hoverSprite;
    }

    // Called when the mouse pointer exits the object’s bounds
    private void OnMouseExit()
    {
        if (!isInteractable || originalSprite == null || spriteRenderer == null) return;

        // Restore the original sprite when the mouse exits
        spriteRenderer.sprite = originalSprite;
    }

    // Called when the mouse button is clicked while over the object
    private void OnMouseDown()
    {
        if (!isInteractable) return;

        spriteRenderer.sprite = originalSprite;

        // Invoke any custom onClick methods
        if (onClick != null)
        {
            onClick.Invoke();
        }
    }

    // Optionally, disable interaction if needed
    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
    }

    // Dynamically add a method to the onClick event
    public void AddClickListener(UnityAction action)
    {
        onClick.AddListener(action);
    }

    // Remove a listener from the onClick event
    public void RemoveClickListener(UnityAction action)
    {
        onClick.RemoveListener(action);
    }
}
