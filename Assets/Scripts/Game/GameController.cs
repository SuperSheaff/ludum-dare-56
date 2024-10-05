using UnityEngine;
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
    public Transform cardHandPosition;

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

    private CardDisplay currentCardBeingPlayed; // The card that's currently being played

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
        // Instantiate the same duck prefab for each type
        rogueDuck = Instantiate(duckPrefab, rogueSpawnPoint.position, Quaternion.identity).GetComponent<Duck>();
        knightDuck = Instantiate(duckPrefab, knightSpawnPoint.position, Quaternion.identity).GetComponent<Duck>();
        wizardDuck = Instantiate(duckPrefab, wizardSpawnPoint.position, Quaternion.identity).GetComponent<Duck>();

        enemy = Instantiate(enemyPrefab, enemySpawnPoint.position, Quaternion.identity).GetComponent<Enemy>();

        // Initialize ducks with health, attack values, and types
        rogueDuck.InitializeCharacter("Rogue Duck", 100, 10);
        rogueDuck.InitializeDuck(DuckType.Rogue);

        knightDuck.InitializeCharacter("Knight Duck", 150, 8);
        knightDuck.InitializeDuck(DuckType.Knight);

        wizardDuck.InitializeCharacter("Wizard Duck", 80, 12);
        wizardDuck.InitializeDuck(DuckType.Wizard);

        // Initialize enemy with health and attack values
        enemy.InitializeCharacter("Enemy", 200, 15);
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

        public void OnCardPlayed(CardDisplay card)
    {
        // Set the current card being played
        currentCardBeingPlayed = card;

        // Show the "Choose target" text
        chooseTargetText.gameObject.SetActive(true);

        // Show UI markers on all available targets
        foreach (var target in availableTargets)
        {
            target.GetComponent<TargetMarker>().ShowMarker();
        }
    }

    // Called when a target is clicked
    public void OnTargetChosen(GameObject target)
    {
        if (currentCardBeingPlayed != null)
        {
            // Apply card effect to the target (for now, just log it)
            Debug.Log($"{currentCardBeingPlayed.cardNameText.text} played on {target.name}!");

            // Hide the "Choose target" text
            chooseTargetText.gameObject.SetActive(false);

            // Hide UI markers on all available targets
            foreach (var availableTarget in availableTargets)
            {
                availableTarget.GetComponent<TargetMarker>().HideMarker();
            }

            // End card play sequence
            currentCardBeingPlayed = null;
        }
    }
}
