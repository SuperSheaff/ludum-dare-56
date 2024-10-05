using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer for visual feedback
    public Color defaultColor = Color.white;
    public Color hoverColor = Color.gray;
    public Color clickColor = Color.green;

    private void Start()
    {
        // Set the button to its default color
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = defaultColor;
    }

    private void OnMouseOver()
    {
        // Change the color of the button when the mouse hovers over it
        spriteRenderer.color = hoverColor;

        // Check if the mouse is clicked while hovering
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            OnClick();
        }
    }

    private void OnMouseExit()
    {
        // Change the color back to default when the mouse stops hovering
        spriteRenderer.color = defaultColor;
    }

    private void OnClick()
    {
        // Change the button's color to give feedback when clicked
        spriteRenderer.color = clickColor;

        // Call the function you want to trigger
        ExecuteFunction();
    }

    // Function that will be triggered when the button is clicked
    private void ExecuteFunction()
    {
        Debug.Log("Button clicked!");
        // Here, you can call any method or action you want, such as ending the player's turn:
        GameController.instance.EndPlayerTurn();
    }
}
