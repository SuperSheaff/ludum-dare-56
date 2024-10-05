using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardController : MonoBehaviour
{
    public static CardController instance;

    public List<Card> deck = new List<Card>(); // Logical card data
    public List<Card> drawPile = new List<Card>(); // Draw pile, shuffled from the deck
    public List<Card> discardPile = new List<Card>(); // Discarded cards
    public List<CardDisplay> hand = new List<CardDisplay>(); // Cards in the player's hand

    public GameObject cardPrefab; // Prefab containing the CardDisplay script
    public Transform cardSpawningPoint;
    public Transform spreadStartPoint;
    public Transform spreadEndPoint;

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
        deck.Add(new Card("Rogue Slash", CardType.Rogue, 1, 10, 0));
        deck.Add(new Card("Knight Shield", CardType.Knight, 2, 0, 15));
        deck.Add(new Card("Wizard Fireball", CardType.Wizard, 3, 20, 0));
        deck.Add(new Card("Neutral Strike", CardType.Neutral, 1, 5, 0));
        deck.Add(new Card("Neutral Block", CardType.Neutral, 1, 0, 5));
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
}
