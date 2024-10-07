using UnityEngine;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public TextMeshPro manaCostText;
    public TextMeshPro descriptionText;  // Add a reference to the description text object
    public SpriteRenderer cardSpriteRenderer;

    public float moveSpeed = 5.0f;  // Speed at which the card moves
    public float hoverHeight = 1.0f; // How much the card should move up when hovered
    public float hoverZOffset = -1.0f; // How much the card should come forward on Z when hovered

    private bool isDisabled = false; // To track if the card is disabled
    public GameObject disabledIcon; // Reference to an icon that appears when the card is disabled

    public bool IsReward = false;  // This will identify if the card is a reward card

    // Add a reference to the card's logical data
    public Card cardData { get; private set; } // This is the missing 'cardData'

    public Vector3 targetPosition;  // The final position set by the controller

    public Vector3 hoverOffset = Vector3.zero;  // Offset added when hovering
    public bool isHovered      = false;  // Whether the card is hovered
    public bool isBeingPlayed  = false; // Whether the card is in the play position
    public bool hoverEnabled   = true; // Whether the card is in the play position

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
    public void SetCardData(Card cardData, bool isReward = false)
    {
        this.cardData = cardData; // Set the card's data
        this.IsReward = isReward;  // Mark if it's a reward card

        manaCostText.text = cardData.manaCost.ToString();

        if (cardData.isDisabled)
        {
            Disable();
        }

        // Change color based on card type
        switch (cardData.cardName)
        {
            case "Smoke Bomb":
                cardSpriteRenderer.sprite = smokebombCardSprite;
                SetDescription($"All ducks gain <color=#00FF00>50%</color> evade chance for <color=#FFFF00>{cardData.primaryAmount}</color> turns.");
                break;
            case "Poison":
                cardSpriteRenderer.sprite = poisonCardSprite;
                SetDescription($"Apply poison to target, dealing <color=#FF0000>{cardData.primaryAmount}</color> damage over <color=#FFFF00>{cardData.secondaryAmount}</color> turns.");
                break;
            case "Heal":
                cardSpriteRenderer.sprite = healCardSprite;
                SetDescription($"Heal <color=#00FF00>{cardData.primaryAmount}</color> health to target duck.");
                break;
            case "Fireball":
                cardSpriteRenderer.sprite = fireballCardSprite;
                SetDescription($"Deal <color=#FF4500>{cardData.primaryAmount}</color> damage to a target enemy.");
                break;
            case "Taunt":
                cardSpriteRenderer.sprite = tauntCardSprite;
                SetDescription($"Taunt the enemy, forcing them to target the <color=#ADD8E6>Knight</color>.");
                break;
            case "Reckless":
                cardSpriteRenderer.sprite = recklessCardSprite;
                SetDescription($"Deal <color=#FF0000>{cardData.primaryAmount}</color> damage to target and <color=#FFFF00>{cardData.secondaryAmount}</color> damage to knight duck.");
                break;
            case "Neutral Attack":
                cardSpriteRenderer.sprite = neutralAttackCardSprite;
                SetDescription($"Deal <color=#FF0000>{cardData.primaryAmount}</color> damage to a target enemy.");
                break;
            case "Neutral Block":
                cardSpriteRenderer.sprite = neutralBlockCardSprite;
                SetDescription($"Gain <color=#00FFFF>{cardData.primaryAmount}</color> block.");
                break;
        }
    }

    private void Update()
    {
        if (!IsReward)
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
    }

 
    // Handle hover effect
    private void OnMouseEnter()
    {
        if (!isBeingPlayed  && hoverEnabled && !isDisabled && !IsReward) // Disable hover if the card is being played
        {
            isHovered = true;
            hoverOffset = new Vector3(0, hoverHeight, hoverZOffset);  // Apply hover offset (Y and Z)
        }
    }

    private void OnMouseExit()
    {
        if (!isBeingPlayed  && hoverEnabled && !isDisabled && !IsReward) // Disable hover if the card is being played
        {
            isHovered = false;
            hoverOffset = Vector3.zero;  // Reset hover offset
        }
    }

    private void OnMouseDown()
    {
        if (IsReward)
        {
            CardController.instance.AddCardToDeck(this.cardData);
            GameController.instance.chooseNewCardText.SetActive(false);
        }
        // When clicked, move the card to the play position and start target selection
        else if (isHovered && !isBeingPlayed  && hoverEnabled && !isDisabled)
        {
            isBeingPlayed = true;
            hoverOffset = Vector3.zero;  // Reset hover offset

            // Send this card to the CardController
            CardController.instance.OnCardSelected(this);
        }
    }

    // Function to set the card's description based on primary and secondary amounts
    private void SetDescription(string description)
    {
        if (descriptionText != null)
        {
            descriptionText.text = description;
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
        // Check if the target position contains NaN values
        if (float.IsNaN(targetPosition.x) || float.IsNaN(targetPosition.y) || float.IsNaN(targetPosition.z))
        {
            Debug.LogError("targetPosition position contains NaN values: " + targetPosition);
            return;
        }

        targetPosition = newPosition;

        // Reset hover offset when setting the new position
        if (!isHovered)
        {
            hoverOffset = Vector3.zero;
        }
    }


    // Function to disable the card (prevents it from being played)
    public void Disable()
    {
        isDisabled = true;
        hoverEnabled = false; // Disable hovering
        cardSpriteRenderer.color = Color.gray; // Set sprite color to gray to visually indicate disabled state
        if (disabledIcon != null)
        {
            disabledIcon.SetActive(true); // Show disabled icon if present
        }
    }

    // Function to enable the card (allows it to be played again)
    public void Enable()
    {
        isDisabled = false;
        hoverEnabled = true; // Enable hovering
        cardSpriteRenderer.color = Color.white; // Reset sprite color to normal
        if (disabledIcon != null)
        {
            disabledIcon.SetActive(false); // Hide disabled icon if present
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
