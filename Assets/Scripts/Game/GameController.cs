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
    public List<GameObject> availableTargets; // The list of available targets in the scene


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

    public GameObject endTurnButton; // Button to end the player's turn

    // Reference to the main camera
    public Camera mainCamera;

    // Card-related values
    public CardController cardController;

    // Positioning values for cards
    public float cardSpacing = 2.0f; // Adjust this for more or less spread
    public float cardYOffset = -3.0f; // Adjust this to move the cards lower on the screen

    public List<Character> enemyCharacters; // List of all enemy characters
    public List<Character> allyCharacters;  // List of all ally characters
    public Transform playCardPosition;      // Position where cards move to when played
    public GameObject chooseTargetText;     // Text for "Choose Target"

    private Card currentCard;               // The card currently being played
    private CardDisplay currentCardDisplay; // The CardDisplay of the current card

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
        availableTargets = new List<GameObject>(); // Initialize the availableTargets list

        // Instantiate the same duck prefab for each type
        rogueDuck = Instantiate(duckPrefab, rogueSpawnPoint.position, Quaternion.identity).GetComponent<Duck>();
        knightDuck = Instantiate(duckPrefab, knightSpawnPoint.position, Quaternion.identity).GetComponent<Duck>();
        wizardDuck = Instantiate(duckPrefab, wizardSpawnPoint.position, Quaternion.identity).GetComponent<Duck>();

        enemy = Instantiate(enemyPrefab, enemySpawnPoint.position, Quaternion.identity).GetComponent<Enemy>();

        // Initialize ducks with health, attack values, and types
        rogueDuck.InitializeCharacter("Rogue Duck", 100, 10, CharacterType.Rogue);
        rogueDuck.InitializeDuck(DuckType.Rogue);
        availableTargets.Add(rogueDuck.gameObject); // Add Rogue Duck to available targets

        knightDuck.InitializeCharacter("Knight Duck", 150, 8, CharacterType.Knight);
        knightDuck.InitializeDuck(DuckType.Knight);
        availableTargets.Add(knightDuck.gameObject); // Add Knight Duck to available targets

        wizardDuck.InitializeCharacter("Wizard Duck", 80, 12, CharacterType.Wizard);
        wizardDuck.InitializeDuck(DuckType.Wizard);
        availableTargets.Add(wizardDuck.gameObject); // Add Wizard Duck to available targets

        // Initialize enemy with health and attack values
        enemy.InitializeCharacter("Enemy", 200, 15, CharacterType.Enemy);
        availableTargets.Add(enemy.gameObject); // Add Enemy to available targets
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
    public void OnCardPlayed(CardDisplay cardDisplay)
    {
        currentCard         = cardDisplay.cardData;     // Store the logical card data
        currentCardDisplay  = cardDisplay;              // Store the visual card display

        // Move the card to the "play card" position
        cardDisplay.SetTargetPosition(playCardPosition.position);

        // Show "Choose Target" text
        chooseTargetText.SetActive(true);

        // Show markers on valid targets based on the card's target type
        ShowMarkersForValidTargets();
    }

    // Show markers on valid targets
    private void ShowMarkersForValidTargets()
    {
        List<GameObject> validTargets = new List<GameObject>();

        switch (currentCard.targetType)
        {
            case TargetType.AllAllies:
                foreach (GameObject target in availableTargets)
                {
                    Character targetChar = target.GetComponent<Character>();
                    if (targetChar.IsAlly())
                    {
                        validTargets.Add(target);
                    }
                }
                break;

            case TargetType.Knight:
                foreach (GameObject target in availableTargets)
                {
                    Character targetChar = target.GetComponent<Character>();
                    if (targetChar.characterType == CharacterType.Knight) // Filter specific class
                    {
                        validTargets.Add(target);
                    }
                }
                break;

            case TargetType.Wizard:
                foreach (GameObject target in availableTargets)
                {
                    Character targetChar = target.GetComponent<Character>();
                    if (targetChar.characterType == CharacterType.Wizard) // Filter specific class
                    {
                        validTargets.Add(target);
                    }
                }
                break;

            case TargetType.Rogue:
                foreach (GameObject target in availableTargets)
                {
                    Character targetChar = target.GetComponent<Character>();
                    if (targetChar.characterType == CharacterType.Rogue) // Filter specific class
                    {
                        validTargets.Add(target);
                    }
                }
                break;

            case TargetType.Enemy:
                foreach (GameObject target in availableTargets)
                {
                    Character targetChar = target.GetComponent<Character>();
                    if (targetChar.IsEnemy())
                    {
                        validTargets.Add(target);
                    }
                }
                break;

            case TargetType.Any:
                validTargets.AddRange(availableTargets);  // All targets can be selected
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
            ApplyCardEffect(targetCharacter); // Apply card effects

            // Hide all markers after selection
            HideAllMarkers();

            // Hide "Choose Target" text
            chooseTargetText.gameObject.SetActive(false);
        }
    }

    // Hide markers on all characters
    private void HideAllMarkers()
    {
        foreach (Character enemy in enemyCharacters)
        {
            enemy.HideMarker();
        }

        foreach (Character ally in allyCharacters)
        {
            ally.HideMarker();
        }
    }

    // Apply the card's effect to the target
    private void ApplyCardEffect(Character target)
    {
        // Example effect: If the card is an attack card, damage the target
        if (currentCard.attackPower > 0)
        {
            target.TakeDamage(currentCard.attackPower);
        }
        else if (currentCard.blockPower > 0)
        {
            target.GainBlock(currentCard.blockPower);
        }

        // After applying the card, you may want to discard or remove the card from hand, etc.
    }
}
