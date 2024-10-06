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
    private bool isHovered      = false;  // Whether the card is hovered
    private bool isBeingPlayed  = false; // Whether the card is in the play position
    private bool hoverEnabled   = true; // Whether the card is in the play position

    // Rogue Cards
    public Sprite smokebombCardSprite;
    public Sprite poisonCardSprite;

    // Wizard Cards
    public Sprite healCardSprite;
    public Sprite fireballCardSprite;

    // Knight Cards
    public Sprite tauntCardSprite;
    public Sprite recklessCardSprite;

    // Neutral Cards
    public Sprite neutralAttackCardSprite;
    public Sprite neutralBlockCardSprite;

    // A reference to the game controller
    private GameController gameController;
    private CardController cardController;

    private void Start()
    {
        gameController = GameController.instance; // Get reference to game controller
        cardController = CardController.instance; // Get reference to game controller
    }

    // Function to set card display based on the logical Card data
    public void SetCardData(Card cardData)
    {
        this.cardData = cardData; // Set the card's data

        cardNameText.text = cardData.cardName;
        manaCostText.text = cardData.manaCost.ToString();

        // Change color based on card type
        switch (cardData.cardName)
        {
            case "Smoke Bomb":
                cardSpriteRenderer.sprite = smokebombCardSprite;
                break;
            case "Poison":
                cardSpriteRenderer.sprite = poisonCardSprite;
                break;
            case "Heal":
                cardSpriteRenderer.sprite = healCardSprite;
                break;
            case "Fireball":
                cardSpriteRenderer.sprite = fireballCardSprite;
                break;
            case "Taunt":
                cardSpriteRenderer.sprite = tauntCardSprite;
                break;
            case "Reckless":
                cardSpriteRenderer.sprite = recklessCardSprite;
                break;
            case "Neutral Attack":
                cardSpriteRenderer.sprite = neutralAttackCardSprite;
                break;
            case "Neutral Attack":
                cardSpriteRenderer.sprite = neutralAttackCardSprite;
                break;
        }
    }

    private void Update()
    {
        if (isBeingPlayed && hoverEnabled)
        {
            // Move towards the play position when card is being played
            MoveCardTowards(cardController.playCardPosition.position);
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
        if (!isBeingPlayed  && hoverEnabled) // Disable hover if the card is being played
        {
            isHovered = true;
            hoverOffset = new Vector3(0, hoverHeight, hoverZOffset);  // Apply hover offset (Y and Z)
        }
    }

    private void OnMouseExit()
    {
        if (!isBeingPlayed  && hoverEnabled) // Disable hover if the card is being played
        {
            isHovered = false;
            hoverOffset = Vector3.zero;  // Reset hover offset
        }
    }

    private void OnMouseDown()
    {
        // When clicked, move the card to the play position and start target selection
        if (isHovered && !isBeingPlayed  && hoverEnabled)
        {
            isBeingPlayed = true;
            hoverOffset = Vector3.zero;  // Reset hover offset

            // Send this card to the CardController
            CardController.instance.OnCardSelected(this);
        }
    }

    public void DisableHover()
    {
        hoverEnabled = false; // Stop any active hover effect
    }

    public void EnableHover()
    {
        hoverEnabled = true; // Stop any active hover effect
    }
}
