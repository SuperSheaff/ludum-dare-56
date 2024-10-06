using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CardController : MonoBehaviour
{
    public static CardController instance;

    public List<Card> cardLibrary               = new List<Card>(); // Logical card data
    public List<Card> deck                      = new List<Card>(); // Logical card data
    public List<Card> drawPile                  = new List<Card>(); // Draw pile, shuffled from the deck
    public List<Card> discardPile               = new List<Card>(); // Discarded cards
    public List<CardDisplay> hand               = new List<CardDisplay>(); // Cards in the player's hand
    public List<Card> rewardCards               = new List<Card>(); // Cards in the player's hand
    public List<CardDisplay> rewardCardDisplays = new List<CardDisplay>(); // Cards in the player's hand

    public CardDisplay selectedCard; // Card currently selected and played

    public Transform[] endPositions;

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
        deck.Add(new Card("Smoke Bomb",         CardType.Rogue,     TargetType.AllAllies,       1, 1, 0));
        deck.Add(new Card("Poison",             CardType.Rogue,     TargetType.Enemy,           2, 2, 3));
        deck.Add(new Card("Heal",               CardType.Wizard,    TargetType.AllAllies,       1, 4, 0));
        deck.Add(new Card("Fireball",           CardType.Wizard,    TargetType.Enemy,           1, 3, 0));
        deck.Add(new Card("Taunt",              CardType.Knight,    TargetType.Knight,          1, 1, 0));
        deck.Add(new Card("Reckless",           CardType.Knight,    TargetType.Enemy,           1, 8, 4));
        deck.Add(new Card("Neutral Attack",     CardType.Neutral,   TargetType.Enemy,           1, 1, 0));
        deck.Add(new Card("Neutral Attack",     CardType.Neutral,   TargetType.Enemy,           1, 1, 0));
        deck.Add(new Card("Neutral Block",      CardType.Neutral,   TargetType.AllAllies,       1, 5, 0));
        deck.Add(new Card("Neutral Block",      CardType.Neutral,   TargetType.AllAllies,       1, 5, 0));
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

    // Function to disable cards of a specific type across all piles (hand, draw, discard)
    public void DisableCardsByType(CharacterType characterType)
    {
        
        // Disable cards in hand
        foreach (CardDisplay card in hand)
        {
            if (card.cardData.cardType == GetCardTypeFromCharacterType(characterType))
            {
                card.Disable();
            }
        }

        // Disable cards in the draw pile
        foreach (Card card in drawPile)
        {
            if (card.cardType == GetCardTypeFromCharacterType(characterType))
            {
                card.isDisabled = true; // Flag as disabled for when it's drawn
            }
        }

        // Disable cards in the discard pile
        foreach (Card card in discardPile)
        {
            if (card.cardType == GetCardTypeFromCharacterType(characterType))
            {
                card.isDisabled = true; // Flag as disabled for when it's drawn
            }
        }
    }

    // Function to enable cards of a specific type across all piles (hand, draw, discard)
    public void EnableCardsByType(CharacterType characterType)
    {
        // Enable cards in hand
        foreach (CardDisplay card in hand)
        {
            if (card.cardData.cardType == GetCardTypeFromCharacterType(characterType))
            {
                card.Enable();
            }
        }

        // Enable cards in the draw pile
        foreach (Card card in drawPile)
        {
            if (card.cardType == GetCardTypeFromCharacterType(characterType))
            {
                card.isDisabled = false; // Remove disabled flag
            }
        }

        // Enable cards in the discard pile
        foreach (Card card in discardPile)
        {
            if (card.cardType == GetCardTypeFromCharacterType(characterType))
            {
                card.isDisabled = false; // Remove disabled flag
            }
        }
    }
    
    // Utility function to map CharacterType to CardType
    private CardType GetCardTypeFromCharacterType(CharacterType characterType)
    {
        switch (characterType)
        {
            case CharacterType.Knight:
                return CardType.Knight;
            case CharacterType.Wizard:
                return CardType.Wizard;
            case CharacterType.Rogue:
                return CardType.Rogue;
            default:
                return CardType.Neutral;
        }
    }

    // Function to clear all cards from draw pile, discard pile, and hand, and remove all card displays
    public void ClearAllCards()
    {
        // Clear all displays of cards in hand
        foreach (CardDisplay cardDisplay in hand)
        {
            Destroy(cardDisplay.gameObject); // Destroy the visual representation in the hand
        }

        // Clear the hand, draw pile, and discard pile
        hand.Clear();
        drawPile.Clear();
        discardPile.Clear();
        discardPile.Clear();

        Debug.Log("All cards, including displays, have been cleared.");
    }

    public void InitializeCardLibrary()
    {
        // Initialize the card library with all unique cards
        cardLibrary.Add(new Card("Smoke Bomb",         CardType.Rogue,     TargetType.AllAllies,       1, 1, 0));
        cardLibrary.Add(new Card("Poison",             CardType.Rogue,     TargetType.Enemy,           2, 2, 3));
        cardLibrary.Add(new Card("Heal",               CardType.Wizard,    TargetType.AllAllies,       1, 4, 0));
        cardLibrary.Add(new Card("Fireball",           CardType.Wizard,    TargetType.Enemy,           1, 3, 0));
        cardLibrary.Add(new Card("Taunt",              CardType.Knight,    TargetType.Knight,          1, 1, 0));
        cardLibrary.Add(new Card("Reckless",           CardType.Knight,    TargetType.Enemy,           1, 8, 4));
        cardLibrary.Add(new Card("Neutral Attack",     CardType.Neutral,   TargetType.Enemy,           1, 1, 0));
        cardLibrary.Add(new Card("Neutral Block",      CardType.Neutral,   TargetType.AllAllies,       1, 5, 0));
        // Add more unique cards as needed
    }

    public void GenerateRewardCards()
    {
        GameController.instance.MoveUIToNextLevel();
        
        // Ensure the card library is initialized
        if (cardLibrary == null || cardLibrary.Count == 0)
        {
            InitializeCardLibrary();
        }

        for (int i = 0; i < 3; i++)
        {
            // Randomly pick a card from the card library
            Card randomCard = cardLibrary[Random.Range(0, cardLibrary.Count)];
            rewardCards.Add(randomCard);
        }

        // Spawn the cards off-screen and then animate them into the target position
        for (int i = 0; i < rewardCards.Count; i++)
        {
            // Instantiate visual card (CardDisplay) at start position below the screen
            Vector3 startPosition = endPositions[i].position - new Vector3(0, 38.4f, 0); // 38.4 units below the target position
            GameObject cardObj = Instantiate(cardPrefab, startPosition, Quaternion.identity);

            // Set card data (display the card)
            CardDisplay cardDisplay = cardObj.GetComponent<CardDisplay>();
            cardDisplay.SetCardData(rewardCards[i], true);
            rewardCardDisplays.Add(cardDisplay);

            // Animate the card moving into position
            StartCoroutine(AnimateCardIntoPosition(cardDisplay, endPositions[i].position));
        }

        Debug.Log("Reward cards generated and moving into position.");
    }

    private IEnumerator AnimateCardIntoPosition(CardDisplay card, Vector3 endPosition)
    {
        float moveSpeed = 50f; // Adjust the speed as necessary
        Vector3 startPosition = card.transform.position;

        float elapsedTime = 0;
        float totalTime = 0.5f; // Time for the animation to complete

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            card.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / totalTime);
            yield return null;
        }

        // Ensure the card ends exactly at the target position
        card.transform.position = endPosition;
    }

    public void AddCardToDeck(Card selectedCard)
    {
        
        deck.Add(selectedCard); // Add the card to the player's deck
        Debug.Log($"Card {selectedCard.cardName} added to the deck!");


        // Loop through the list and destroy each CardDisplay object
        foreach (CardDisplay cardDisplay in rewardCardDisplays)
        {
            if (cardDisplay != null) // Make sure the object exists before destroying it
            {
                Destroy(cardDisplay.gameObject); // Destroy the GameObject attached to the CardDisplay
            }
        }

        // Clear the list after destroying all the objects
        rewardCardDisplays.Clear();
        rewardCards.Clear();

        GameController.instance.PrepareNextLevel();
    }

}
