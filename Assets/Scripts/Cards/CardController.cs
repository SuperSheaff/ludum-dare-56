using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CardController : MonoBehaviour
{
    public static CardController instance;

    public List<Card> deck          = new List<Card>(); // Logical card data
    public List<Card> drawPile      = new List<Card>(); // Draw pile, shuffled from the deck
    public List<Card> discardPile   = new List<Card>(); // Discarded cards
    public List<CardDisplay> hand   = new List<CardDisplay>(); // Cards in the player's hand

    public CardDisplay selectedCard; // Card currently selected and played

    public GameObject cardPrefab; // Prefab containing the CardDisplay script
    public Transform cardSpawningPoint;
    public Transform spreadStartPoint;
    public Transform spreadEndPoint;

    private CardDisplay currentCardBeingPlayed; // The card that's currently being played
    public Transform playCardPosition;      // Position where cards move to when played

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Function to initialize the deck with predefined cards
    public void InitializeDeck()
    {
        deck.Add(new Card("Smoke Bomb",         CardType.Rogue, TargetType.AllAllies,   1, 10, 0));
        deck.Add(new Card("Poison",             CardType.Rogue, TargetType.Enemy,       2, 0, 15)); // Targets a specific ally, e.g., a Knight
        deck.Add(new Card("Heal",               CardType.Wizard, TargetType.AllAllies,  1, 5, 0));
        deck.Add(new Card("Fireball",           CardType.Knight, TargetType.Enemy,      1, 5, 0));
        deck.Add(new Card("Taunt",              CardType.Knight, TargetType.Knight,     1, 5, 0));
        deck.Add(new Card("Reckless",           CardType.Neutral, TargetType.Enemy,     1, 5, 0));
        deck.Add(new Card("Neutral Attack",     CardType.Neutral, TargetType.AllAllies, 1, 0, 5));
        deck.Add(new Card("Neutral Block",      CardType.Neutral, TargetType.AllAllies, 1, 0, 5));
    }

    // Function to shuffle the deck and move the cards to the draw pile
    public void ShuffleDeckIntoDrawPile()
    {
        // Clear the draw pile before moving cards
        drawPile.Clear();

        // Move all cards from the deck to the draw pile
        foreach (Card card in deck)
        {
            drawPile.Add(card);
        }

        // Shuffle the draw pile
        ShuffleList(drawPile);
    }

    // Helper function to shuffle any list
    private void ShuffleList(List<Card> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Card temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // Function to draw cards
    public void DrawHand()
    {
        int cardsToDraw = Mathf.Min(5, drawPile.Count, 8 - hand.Count);
        Vector3 startPosition = cardSpawningPoint.position;

        for (int i = 0; i < cardsToDraw; i++)
        {
            // Get logical card data
            Card cardToDraw = drawPile[0];
            drawPile.RemoveAt(0);

            // Instantiate visual card
            GameObject cardObj = Instantiate(cardPrefab, startPosition, Quaternion.identity);
            CardDisplay cardDisplay = cardObj.GetComponent<CardDisplay>();

            cardDisplay.SetTargetPosition(startPosition);

            // Initialize the display with the card's logical data
            cardDisplay.SetCardData(cardToDraw);

            // Add card display to hand
            hand.Add(cardDisplay);
        }

        // Move cards to target positions
        StartCoroutine(SetAllCardsTargetPositions());
    }

    // Function to check if the draw pile is empty, and if it is, shuffle the discard pile back in
    public void CheckDrawPile()
    {
        // Check if the draw pile is empty
        if (drawPile.Count == 0)
        {
            if (discardPile.Count > 0)
            {
                // Move all discard pile cards back into the draw pile
                drawPile.AddRange(discardPile);

                // Clear the discard pile
                discardPile.Clear();

                // Shuffle the new draw pile
                ShuffleList(drawPile);

                Debug.Log("Draw pile was empty. Discard pile shuffled into draw pile.");
            }
            else
            {
                Debug.Log("Both draw pile and discard pile are empty.");
            }
        }
    }

    // Function to return the card to the hand
    public void ReturnCardToHand()
    {
        // Add the card back to the hand
        hand.Add(selectedCard);
        selectedCard.isBeingPlayed = false;

        // Clear the selected card
        selectedCard = null;

        // Move the card back to its original position in the hand
        UpdateHandCardPositions();

        EnableHoverOnAllCards();
    }

    // Function called when a card is selected
    public void OnCardSelected(CardDisplay cardDisplay)
    {
        // Add the card to selectedCard
        selectedCard = cardDisplay;

        // Remove the card from the hand
        hand.Remove(cardDisplay);

        // Move the card to the play card position
        cardDisplay.SetTargetPosition(playCardPosition.position);

        // Notify the GameController to show the selected card UI
        GameController.instance.UpdateUICardSelected(cardDisplay);

        // Disable hover for all cards in the hand and make them slightly transparent
        DisableHoverOnAllCards();

        // Reset hand positions after removing the card
        UpdateHandCardPositions();
    }

    // Function to update the positions of all cards in the hand
    public void UpdateHandCardPositions()
    {
        Vector3 startPoint = spreadStartPoint.position;
        Vector3 endPoint = spreadEndPoint.position;

        float totalDistance = Vector3.Distance(startPoint, endPoint);
        float stepDistance = hand.Count > 1 ? totalDistance / (hand.Count - 1) : 0;

        for (int i = 0; i < hand.Count; i++)
        {
            float cardXPos = startPoint.x + (i * stepDistance);
            Vector3 targetPosition = new Vector3(cardXPos, startPoint.y, startPoint.z);

            float zPos = (-i * 1f) + 1f; // Cards placed further back have larger Z-values

            // Set target position including Z
            hand[i].SetTargetPosition(new Vector3(cardXPos, startPoint.y, zPos));
        }
    }

    // Move cards to their correct positions in the hand
    private IEnumerator SetAllCardsTargetPositions()
    {
        Vector3 startPoint = spreadStartPoint.position;
        Vector3 endPoint = spreadEndPoint.position;

        float totalDistance = Vector3.Distance(startPoint, endPoint);
        float stepDistance = totalDistance / (hand.Count - 1);

        for (int i = 0; i < hand.Count; i++)
        {
            // Reverse the order so the first card goes to the last position and so on
            int reverseIndex = hand.Count - 1 - i;

            // Calculate the X position for each card using the reverseIndex
            float cardXPos = startPoint.x + (reverseIndex * stepDistance);
            Vector3 targetPosition = new Vector3(cardXPos, startPoint.y, startPoint.z);

            // Adjust the Z position: smaller Z-value for cards dealt later (closer to the camera)
            float zPos = (i * 1f) + 1f; // Cards placed further back have larger Z-values

            // Set target position including Z
            hand[i].SetTargetPosition(new Vector3(cardXPos, cardSpawningPoint.position.y, zPos));

            // Add a delay for sequential movement
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void DeleteSelectedCard()
    {
        Destroy(selectedCard.gameObject);

        // Clear the selected card
        selectedCard = null;
    }

    public void DiscardCard(Card card)
    {
        discardPile.Add(card);  // Move the logical card data to the discard pile

        UpdateHandCardPositions();
    }

    // Function to disable hovering and set transparency on all cards in the hand
    public void DisableHoverOnAllCards()
    {
        foreach (CardDisplay card in hand)
        {
            card.DisableHover(); // Disable hover on each card
            card.cardSpriteRenderer.color = Color.gray;
        }
    }

    public void EnableHoverOnAllCards()
    {
        foreach (CardDisplay card in hand)
        {
            card.EnableHover(); // Enable hover on each card
            card.cardSpriteRenderer.color = Color.white;
        }
    }
}
