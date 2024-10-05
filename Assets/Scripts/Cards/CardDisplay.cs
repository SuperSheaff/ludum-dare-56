using UnityEngine;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public TextMeshPro cardNameText;
    public TextMeshPro manaCostText;
    public SpriteRenderer cardSpriteRenderer;

    public float moveSpeed = 5.0f;  // Speed at which the card moves
    public float hoverHeight = 1.0f; // How much the card should move up when hovered
    public float hoverZOffset = -1.0f; // How much the card should come forward on Z when hovered

    // Add a reference to the card's logical data
    public Card cardData { get; private set; } // This is the missing 'cardData'

    private Vector3 targetPosition;  // The final position set by the controller
    private Vector3 hoverOffset = Vector3.zero;  // Offset added when hovering
    private bool isHovered = false;  // Whether the card is hovered
    private bool isBeingPlayed = false; // Whether the card is in the play position

    // A reference to the game controller
    private GameController gameController;

    private void Start()
    {
        gameController = GameController.instance; // Get reference to game controller
    }

    // Function to set card display based on the logical Card data
    public void SetCardData(Card cardData)
    {
        this.cardData = cardData; // Set the card's data

        cardNameText.text = cardData.cardName;
        manaCostText.text = cardData.manaCost.ToString();

        // Change color based on card type
        switch (cardData.cardType)
        {
            case CardType.Rogue:
                cardSpriteRenderer.color = Color.red;
                break;
            case CardType.Knight:
                cardSpriteRenderer.color = Color.blue;
                break;
            case CardType.Wizard:
                cardSpriteRenderer.color = Color.green;
                break;
            case CardType.Neutral:
                cardSpriteRenderer.color = Color.white;
                break;
        }
    }

    private void Update()
    {
        if (isBeingPlayed)
        {
            // Move towards the play position when card is being played
            MoveCardTowards(gameController.playCardPosition.position);
        }
        else
        {
            // Normal movement towards the assigned position with hover offset
            Vector3 finalTargetPosition = targetPosition + hoverOffset;
            MoveCardTowards(finalTargetPosition);
        }
    }

    // Function to smoothly move the card towards a position
    private void MoveCardTowards(Vector3 target)
    {
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);
    }

    // Set the target position (used by the controller to move the card in the hand)
    public void SetTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;

        // Reset hover offset when setting the new position
        if (!isHovered)
        {
            hoverOffset = Vector3.zero;
        }
    }

    // Handle hover effect
    private void OnMouseEnter()
    {
        if (!isBeingPlayed) // Disable hover if the card is being played
        {
            isHovered = true;
            hoverOffset = new Vector3(0, hoverHeight, hoverZOffset);  // Apply hover offset (Y and Z)
        }
    }

    private void OnMouseExit()
    {
        if (!isBeingPlayed) // Disable hover if the card is being played
        {
            isHovered = false;
            hoverOffset = Vector3.zero;  // Reset hover offset
        }
    }

    private void OnMouseDown()
    {
        // When clicked, move the card to the play position and start target selection
        if (isHovered && !isBeingPlayed)
        {
            isBeingPlayed = true;
            gameController.OnCardPlayed(this); // Notify game controller
        }
    }
}
