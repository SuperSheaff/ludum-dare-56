using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public GameSettings gameSettings;

    public StateMachine<GameController> stateMachine;
    public GameStartState gameStartState;
    public PlayerTurnState playerTurnState;
    public EnemyTurnState enemyTurnState;

    // Prefabs
    public GameObject duckPrefab;
    public GameObject enemyPrefab;
    public GameObject cardPrefab;

    // Position where cards will be displayed
    public List<Character> allCharacters; // The list of available targets in the scene


    // References to spawned ducks and enemy
    public Duck rogueDuck;
    public Duck knightDuck;
    public Duck wizardDuck;
    public Enemy enemy;

    // Spawn points for ducks and enemy
    public Transform rogueSpawnPoint;
    public Transform knightSpawnPoint;
    public Transform wizardSpawnPoint;
    public Transform enemySpawnPoint;

    public GameObject endTurnButton;        // Button to end the player's turn
    public GameObject unselectButton;       // Button to unselect the card

    // Reference to the main camera
    public Camera mainCamera;

    // Card-related values
    public CardController cardController;

    // Positioning values for cards
    public float cardSpacing = 2.0f; // Adjust this for more or less spread
    public float cardYOffset = -3.0f; // Adjust this to move the cards lower on the screen

    public List<Character> enemyCharacters; // List of all enemy characters
    public List<Character> allyCharacters;  // List of all ally characters
    public GameObject chooseTargetText;     // Text for "Choose Target"

    private Card currentCard;               // The card currently being played
    private CardDisplay currentCardDisplay; // The CardDisplay of the current card

    public int startingMana     = 5;
    public int baseMana         = 5;
    public int currentMana      = 5;    // Example mana value, this can be dynamic
    public TextMeshPro manaText;        // Reference to the TextMeshPro component for displaying health

    private void Awake()
    {
        // Singleton pattern implementation
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

    private void Start()
    {
        InitializeCharacters();
        InitializeStateMachine();

        // Initialize the deck in CardController
        CardController.instance.InitializeDeck();

        // Shuffle the deck and fill the draw pile
        CardController.instance.ShuffleDeckIntoDrawPile();
    }

    private void Update()
    {
        stateMachine.Update();

        UpdateUI();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Simulate the enemy attacking the rogue duck for testing purposes
            // rogueDuck.TakeDamage(10);
        }
        
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    private void InitializeStateMachine()
    {
        stateMachine        = new StateMachine<GameController>(true);

        gameStartState      = new GameStartState(this);
        playerTurnState     = new PlayerTurnState(this);
        enemyTurnState      = new EnemyTurnState(this);

        stateMachine.Initialize(gameStartState);
    }

    private void InitializeCharacters()
    {
        allCharacters = new List<Character>(); // Initialize the allCharacters list

        // Instantiate the same duck prefab for each type
        rogueDuck = Instantiate(duckPrefab, rogueSpawnPoint.position, Quaternion.identity).GetComponent<Duck>();
        knightDuck = Instantiate(duckPrefab, knightSpawnPoint.position, Quaternion.identity).GetComponent<Duck>();
        wizardDuck = Instantiate(duckPrefab, wizardSpawnPoint.position, Quaternion.identity).GetComponent<Duck>();

        enemy = Instantiate(enemyPrefab, enemySpawnPoint.position, Quaternion.identity).GetComponent<Enemy>();

        // Initialize ducks with health, attack values, and types
        rogueDuck.InitializeCharacter("Rogue Duck", 10, 10, CharacterType.Rogue);
        rogueDuck.InitializeDuck(DuckType.Rogue);
        allCharacters.Add(rogueDuck); // Add Rogue Duck to available targets

        knightDuck.InitializeCharacter("Knight Duck", 15, 8, CharacterType.Knight);
        knightDuck.InitializeDuck(DuckType.Knight);
        allCharacters.Add(knightDuck); // Add Knight Duck to available targets

        wizardDuck.InitializeCharacter("Wizard Duck", 8, 12, CharacterType.Wizard);
        wizardDuck.InitializeDuck(DuckType.Wizard);
        allCharacters.Add(wizardDuck); // Add Wizard Duck to available targets

        // Initialize enemy with health and attack values
        enemy.InitializeCharacter("Enemy", 10, 15, CharacterType.Enemy);
        allCharacters.Add(enemy); // Add Enemy to available targets
    }

    // Function to end the player's turn (called by the End Turn button)
    public void EndPlayerTurn()
    {
        stateMachine.ChangeState(enemyTurnState);
    }

    // Function to end the enemy's turn and start the player's turn
    public void EndEnemyTurn()
    {
        stateMachine.ChangeState(playerTurnState);
    }

    // Method to play a card (triggered when clicking on a card)
    public void UpdateUICardSelected(CardDisplay cardDisplay)
    {
        currentCard         = cardDisplay.cardData;     // Store the logical card data
        currentCardDisplay  = cardDisplay;              // Store the visual card display

        // Show "Choose Target" text
        chooseTargetText.SetActive(true);

        // Show "Unselect" button to cancel the action
        unselectButton.SetActive(true);

        // Show markers on valid targets based on the card's target type
        ShowMarkersForValidTargets();
    }

    // Method to handle the unselect card button click
    public void OnUnselectCardButton()
    {
        if (currentCardDisplay != null)
        {
            // Call the CardController to return the card to the hand
            CardController.instance.ReturnCardToHand();

            // Reset UI elements
            chooseTargetText.SetActive(false);
            unselectButton.SetActive(false);

            // Clear the current card and display reference
            currentCard = null;
            currentCardDisplay = null;

            HideAllMarkers();
        }
    }

    // Show markers on valid targets
    private void ShowMarkersForValidTargets()
    {
        List<Character> validTargets = new List<Character>();

        switch (currentCard.targetType)
        {
            case TargetType.AllAllies:
                foreach (Character target in allCharacters)
                {
                    if (target.IsAlly())
                    {
                        validTargets.Add(target);
                    }
                }
                break;

            case TargetType.Knight:
                foreach (Character target in allCharacters)
                {
                    if (target.characterType == CharacterType.Knight) // Filter specific class
                    {
                        validTargets.Add(target);
                    }
                }
                break;

            case TargetType.Wizard:
                foreach (Character target in allCharacters)
                {
                    if (target.characterType == CharacterType.Wizard) // Filter specific class
                    {
                        validTargets.Add(target);
                    }
                }
                break;

            case TargetType.Rogue:
                foreach (Character target in allCharacters)
                {
                    if (target.characterType == CharacterType.Rogue) // Filter specific class
                    {
                        validTargets.Add(target);
                    }
                }
                break;

            case TargetType.Enemy:
                foreach (Character target in allCharacters)
                {
                    if (target.IsEnemy())
                    {
                        validTargets.Add(target);
                    }
                }
                break;

            case TargetType.Any:
                validTargets.AddRange(allCharacters);  // All targets can be selected
                break;
        }

        foreach (var target in validTargets)
        {
            target.GetComponent<Character>().ShowMarker();  // Show marker on valid targets
        }
    }


    // Handle when a target is chosen
    public void OnTargetChosen(GameObject target)
    {
        Character targetCharacter = target.GetComponent<Character>();

        if (targetCharacter != null)
        {
            if (HasEnoughMana(currentCard))
            {
                PlayCard(targetCharacter, currentCard); // Apply card effects
            }
            else
            {
                Debug.Log("NOT ENOUGH MANA");
            }
        }
    }

    // Hide markers on all characters
    private void HideAllMarkers()
    {
        foreach (Character character in allCharacters)
        {
            character.GetComponent<Character>().HideMarker();
        }
    }

    // Apply the card's effect to the target
    private void PlayCard(Character target, Card playedCard)
    {

        // Hide all markers after selection
        HideAllMarkers();

        // Hide "Choose Target" text
        chooseTargetText.gameObject.SetActive(false);

        // Change color based on card type
        switch (playedCard.cardName)
        {

            // Apply evade chance to all ally ducks (50% chance to evade for one turn)
            case "Smoke Bomb":
                foreach (Character character in allCharacters)
                {
                    if (character.IsAlly()) // Ensure it's a duck
                    {
                        character.ApplyEvadeChance(0.5f, 1); // 50% evade chance for 1 turn
                    }
                }
                break;


            // Apply poison for primaryAmount damage over secondaryAmount turns
            case "Poison":
                target.ApplyPoison(playedCard.primaryAmount, playedCard.secondaryAmount); 
                break;

            // Apply heal for primaryAmount to target
            case "Heal":
                target.Heal(playedCard.primaryAmount);
                break;

            // Apply damage for primaryAmount to target
            case "Fireball":
                target.TakeDamage(playedCard.primaryAmount);
                break;

            // Change intent to knight duck
            case "Taunt":
                target.TakeDamage(playedCard.primaryAmount);
                break;

            // Deal primary damage to the target and Knight Duck takes secondary damage
            case "Reckless":
                target.TakeDamage(playedCard.primaryAmount);

                // Find the knight duck and deal secondary damage to it
                Character knightDuck = GameController.instance.knightDuck;

                if (knightDuck != null)
                {
                    knightDuck.TakeDamage(playedCard.secondaryAmount);
                    Debug.Log($"Knight Duck takes {playedCard.secondaryAmount} damage from the reckless attack!");
                }
                else
                {
                    Debug.Log("Knight Duck not found.");
                }
                break;

            // Apply damage for primaryAmount to target
            case "Neutral Attack":
                target.TakeDamage(playedCard.primaryAmount);
                break;

            // Apply block for primaryAmount to target
            case "Neutral Block":
                target.GainBlock(playedCard.primaryAmount);
                break;
        }

        // Deduct mana
        GameController.instance.UseMana(playedCard.manaCost);

        // Move the card data to the discard pile
        CardController.instance.DiscardCard(playedCard);

        // Destroy the visual card (CardDisplay)
        CardController.instance.DeleteSelectedCard();
        CardController.instance.EnableHoverOnAllCards();

        // Update the hand positions
        CardController.instance.UpdateHandCardPositions();
    }

    // Function to check if there's enough mana for the selected card
    public bool HasEnoughMana(Card currentCard)
    {
        return currentMana >= currentCard.manaCost;
    }

    // Reduce mana when a card is played
    public void UseMana(int manaCost)
    {
        currentMana -= manaCost;
        Debug.Log("Mana used: " + manaCost + ". Current mana: " + currentMana);
    }

    public void ResetMana()
    {
        if (currentMana < baseMana)
        {
            currentMana = baseMana;
        }
    }

    public void UpdateUI()
    {
        manaText.text = currentMana.ToString();
    }
}
