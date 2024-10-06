using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public GameSettings gameSettings;

    public StateMachine<GameController> stateMachine;
    public GameStartState gameStartState;
    public PlayerTurnState playerTurnState;
    public EnemyTurnState enemyTurnState;
    public ChooseRewardState chooseRewardState;

    // Prefabs
    public GameObject ducksParent;
    public float duckMoveSpeed = 50f;
    public GameObject duckPrefab;
    public GameObject enemyPrefab;
    public GameObject cardPrefab;

    // Position where cards will be displayed
    public List<Character> allCharacters; // The list of available targets in the scene

    // Reference to the game-over screen (assigned via the inspector)
    public GameObject gameOverScreen;
    public GameObject UIContainer; // Reference to the UIContainer object

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

    public List<Enemy> Enemies; // List of all enemy characters
    public List<Duck> DuckParty;  // List of all ally characters
    public GameObject chooseTargetText;     // Text for "Choose Target"

    private Card currentCard;               // The card currently being played
    private CardDisplay currentCardDisplay; // The CardDisplay of the current card

    public int startingMana     = 5;
    public int baseMana         = 5;
    public int currentMana      = 5;    // Example mana value, this can be dynamic
    public TextMeshPro manaText;        // Reference to the TextMeshPro component for displaying health

    public GameObject TilesParent; // Parent for background tiles
    public GameObject dungeonTilePrefab;
    public GameObject wallTilePrefab;

    private List<GameObject> backgroundTiles = new List<GameObject>();

    public float backgroundWidth = 38.4f;  // Width of each background tile
    private int currentRoomIndex = 0;    // Keep track of which room the player is currently in

    public int baseEnemyHealth      = 10;  // Starting health for enemies
    public int baseEnemyDamage      = 5;   // Starting damage for enemies
    public float healthMultiplier   = 1.23f;  // Scaling factor for health
    public float damageMultiplier   = 1.23f;  // Scaling factor for damage
    
    // Add a random variation factor
    public float randomHealthVariation = 0.23f;  // 10% variation in health scaling
    public float randomDamageVariation = 0.23f;  // 10% variation in damage scaling

    public int currentLevel = 1;  // Track the current level
    public TextMeshPro levelText;        // Reference to the TextMeshPro component for displaying health
    public TextMeshPro drawPileText;        // Reference to the TextMeshPro component for displaying health
    public TextMeshPro discardPileText;        // Reference to the TextMeshPro component for displaying health

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
        GenerateBackgroundTiles();

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
        chooseRewardState   = new ChooseRewardState(this);

        stateMachine.Initialize(gameStartState);
    }

    private void InitializeCharacters()
    {
        allCharacters = new List<Character>(); // Initialize the allCharacters list

        // Instantiate the same duck prefab for each type
        rogueDuck = Instantiate(duckPrefab, rogueSpawnPoint.position, Quaternion.identity, ducksParent.transform).GetComponent<Duck>();
        knightDuck = Instantiate(duckPrefab, knightSpawnPoint.position, Quaternion.identity, ducksParent.transform).GetComponent<Duck>();
        wizardDuck = Instantiate(duckPrefab, wizardSpawnPoint.position, Quaternion.identity, ducksParent.transform).GetComponent<Duck>();

        enemy = Instantiate(enemyPrefab, enemySpawnPoint.position, Quaternion.identity).GetComponent<Enemy>();

        // Initialize ducks with health, attack values, and types
        rogueDuck.InitializeCharacter("Rogue Duck", 10, 10, CharacterType.Rogue);
        rogueDuck.InitializeDuck(DuckType.Rogue);
        allCharacters.Add(rogueDuck); // Add Rogue Duck to available targets
        DuckParty.Add(rogueDuck); // Add Rogue Duck to available targets

        knightDuck.InitializeCharacter("Knight Duck", 15, 8, CharacterType.Knight);
        knightDuck.InitializeDuck(DuckType.Knight);
        allCharacters.Add(knightDuck); // Add Knight Duck to available targets
        DuckParty.Add(knightDuck); // Add Knight Duck to available targets

        wizardDuck.InitializeCharacter("Wizard Duck", 8, 12, CharacterType.Wizard);
        wizardDuck.InitializeDuck(DuckType.Wizard);
        allCharacters.Add(wizardDuck); // Add Wizard Duck to available targets
        DuckParty.Add(wizardDuck); // Add Wizard Duck to available targets

        // Initialize enemy with health and attack values
        enemy.InitializeCharacter("Enemy", baseEnemyHealth, baseEnemyDamage, CharacterType.Enemy);
        allCharacters.Add(enemy); // Add Enemy to available targets
        Enemies.Add(enemy); // Add Enemy to available targets
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
        if (currentMana >= currentCard.manaCost)
        {
            chooseTargetText.GetComponent<TextMeshPro>().text = "Choose Target";
            chooseTargetText.SetActive(true);
            ShowMarkersForValidTargets();
        }
        else
        {
            chooseTargetText.GetComponent<TextMeshPro>().text = "Not enough Mana";
            chooseTargetText.SetActive(true);
        }

        // Show "Unselect" button to cancel the action
        unselectButton.SetActive(true);
        endTurnButton.SetActive(false);
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
            endTurnButton.SetActive(true);

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
                    if (target.IsAlly() && target.currentHealth > 0)
                    {
                        validTargets.Add(target);
                    }
                }
                break;

            case TargetType.Knight:
                foreach (Character target in allCharacters)
                {
                    if (target.characterType == CharacterType.Knight && target.currentHealth > 0) // Filter specific class
                    {
                        validTargets.Add(target);
                    }
                }
                break;

            case TargetType.Wizard:
                foreach (Character target in allCharacters)
                {
                    if (target.characterType == CharacterType.Wizard && target.currentHealth > 0) // Filter specific class
                    {
                        validTargets.Add(target);
                    }
                }
                break;

            case TargetType.Rogue:
                foreach (Character target in allCharacters)
                {
                    if (target.characterType == CharacterType.Rogue && target.currentHealth > 0) // Filter specific class
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

        unselectButton.SetActive(false);

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

        // Move the card data to the discard pile
        CardController.instance.DiscardCard(playedCard);

        // Hide "Choose Target" text
        chooseTargetText.gameObject.SetActive(false);
        unselectButton.SetActive(false);
        endTurnButton.SetActive(true);

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

            // Set the enemy's intent to the Knight Duck
            case "Taunt":
                foreach (Character character in allCharacters)
                {
                    if (character.IsEnemy())
                    {
                        character.GetComponent<Enemy>().SetTarget(knightDuck);
                    }
                }
                
                // Recheck all markers to show that the knight is now the target
                UpdateIntentMarkers();

                Debug.Log("Taunt: Enemy's intent changed to attack the Knight Duck.");
                break;

            // Deal primary damage to the target and Knight Duck takes secondary damage
            case "Reckless":
                target.TakeDamage(playedCard.primaryAmount);

                if (knightDuck != null)
                {
                    knightDuck.TakeDamage(playedCard.secondaryAmount);
                    Debug.Log($"Knight Duck takes {playedCard.secondaryAmount} damage from the reckless attack!");

                    // Check if the Knight Duck dies from this attack
                    if (knightDuck.currentHealth <= 0)
                    {
                        Debug.Log("Knight Duck has died!");

                        // Check if the enemy's current intent was to attack the Knight Duck
                        if (enemy.GetTarget() == knightDuck)
                        {
                            Debug.Log("Knight Duck was the enemy's target, recalculating intent...");

                            // Recalculate the intent for the enemy
                            playerTurnState.AssignEnemyIntent();
                            knightDuck.HideIntentMarker();
                        }
                    }
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
        manaText.text           = currentMana.ToString();
        levelText.text          = "Level " + currentLevel.ToString();
        drawPileText.text       = CardController.instance.drawPile.Count.ToString();
        discardPileText.text    = CardController.instance.discardPile.Count.ToString();
    }

    public void UpdateIntentMarkers()
    {
        // First, hide the intent marker on all ducks
        rogueDuck.HideIntentMarker();
        knightDuck.HideIntentMarker();
        wizardDuck.HideIntentMarker();

        // Get the current enemy's target
        Duck currentTarget = enemy.GetTarget();

        // Show the intent marker on the current target (which should now be the Knight Duck)
        if (currentTarget != null)
        {
            currentTarget.ShowIntentMarker();
            Debug.Log($"{currentTarget.characterName} is now the target.");
        }
    }

    public void OnCharacterDeath(CharacterType characterType)
    {
        // Disable the class-specific cards in the hand
        if (characterType == CharacterType.Enemy)
        {
            // Change state
            stateMachine.ChangeState(new ChooseRewardState(this));
        }
        else
        {
            CardController.instance.DisableCardsByType(characterType);
        }
    }

    // Method to trigger the game-over screen
    public void TriggerGameOver()
    {
        // Show the game-over screen (if assigned in the inspector)
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

        // Log the game-over event
        Debug.Log("Game Over!");

        // Disable any further gameplay actions (if needed)
        DisableGameActions();
    }

    // Method to disable further game actions (e.g., disable buttons, stop input, etc.)
    private void DisableGameActions()
    {
        // Example: Disable player input, buttons, or anything else
        // You can add any code here that prevents further game interactions
        endTurnButton.SetActive(false);
        // Additional logic to freeze gameplay can go here
    }

    public void GenerateBackgroundTiles()
    {
        // Create a parent object to hold the tiles (if it doesn't exist)
        if (TilesParent == null)
        {
            TilesParent = new GameObject("TilesParent"); // Create a parent object for background tiles
        }

        // Calculate the x-position for the new tiles based on the number of existing tiles
        float startPositionX = backgroundTiles.Count * backgroundWidth;

        // Tile 1 - Dungeon (at startPositionX)
        GameObject tile1 = Instantiate(dungeonTilePrefab, new Vector3(startPositionX, 0, 0), Quaternion.identity, TilesParent.transform);
        backgroundTiles.Add(tile1);

        // Tile 2 - Wall (at startPositionX + backgroundWidth)
        GameObject tile2 = Instantiate(wallTilePrefab, new Vector3(startPositionX + backgroundWidth, 0, 0), Quaternion.identity, TilesParent.transform);
        backgroundTiles.Add(tile2);
    }

    public void MoveDucksToNextRoom()
    {
        // Calculate the target position based on the current position of ducksParent
        Vector3 targetPosition = ducksParent.transform.position + new Vector3(2 * backgroundWidth, 0, 0); 

        // Move the ducks' parent GameObject
        StartCoroutine(MoveDucksCoroutine(targetPosition));
    }

    private IEnumerator MoveDucksCoroutine(Vector3 targetPosition)
    {
        float duration = 1.5f; // Adjust the time it takes to move
        float elapsedTime = 0f;
        Vector3 startingPosition = ducksParent.transform.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            ducksParent.transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            yield return null;
        }

        ducksParent.transform.position = targetPosition; // Ensure it reaches the final position
    }

    // Function to calculate and return the next room's location
    public Vector3 GenerateNextRoomLocation()
    {
        // Increment the room index to move to the next room
        currentRoomIndex++;

        // Calculate the x-position of the next room based on the index and background width
        float nextRoomX = currentRoomIndex * backgroundWidth;

        // Return the new room's location (on the x-axis, keeping y and z consistent)
        return new Vector3(nextRoomX, 0, -500f); // Keeping y = 0, z = -10 (default camera z)
    }

    public void PrepareNextLevel()
    {

        StartCoroutine(IE_PrepareNextLevel());
    }

    public IEnumerator IE_PrepareNextLevel()
    {
        // Increase the level
        currentLevel++;

        GenerateBackgroundTiles();

        // Clear the previous enemy
        if (enemy != null)
        {
            allCharacters.Remove(enemy);  // Remove enemy from allCharacters list
            Enemies.Remove(enemy);        // Remove enemy from Enemies list
            Destroy(enemy.gameObject);    // Destroy the enemy GameObject
        }

        // Calculate scaled health and damage with added spice (rng)
        float healthRNG = Random.Range(1f - randomHealthVariation, 1f + randomHealthVariation);
        float damageRNG = Random.Range(1f - randomDamageVariation, 1f + randomDamageVariation);

        int scaledHealth = Mathf.RoundToInt(baseEnemyHealth * Mathf.Pow(healthMultiplier, currentLevel - 1) * healthRNG);
        int scaledDamage = Mathf.RoundToInt(baseEnemyDamage * Mathf.Pow(damageMultiplier, currentLevel - 1) * damageRNG);

        // Spawn a new enemy with scaled health and damage
        Vector3 enemySpawnPosition = enemySpawnPoint.position; 
        enemy = Instantiate(enemyPrefab, enemySpawnPosition, Quaternion.identity).GetComponent<Enemy>();
        enemy.InitializeCharacter("New Enemy", scaledHealth, scaledDamage, CharacterType.Enemy);

        allCharacters.Add(enemy); // Add Enemy to available targets
        Enemies.Add(enemy); // Add Enemy to available targets

        // Optionally: Reset player status effects, health, or block here
        foreach (Duck duck in DuckParty)
        {
            duck.block = 0;
            duck.ResetEvadeChance();
            duck.UpdateStatText();

            if (duck.currentHealth <= 0)
            {
                duck.Revive(3);
            }
        }

        // Move the camera to the next room
        CameraController.instance.MoveCameraToNextTile(GenerateNextRoomLocation(), 0, false);

        // Wait for 1 second before transitioning back to player turn state
        yield return new WaitForSeconds(2f);

        // Shuffle the deck and fill the draw pile
        CardController.instance.ShuffleDeckIntoDrawPile();

        // Transition back to player turn state
        stateMachine.ChangeState(playerTurnState);
    }

    // Method to move the UI to the next level
    public void MoveUIToNextLevel()
    {
        // Get the current position of the UIContainer
        Vector3 currentPosition = UIContainer.transform.position;

        // Calculate the new position (move it 2x background width to the right)
        Vector3 nextPosition = new Vector3(currentPosition.x + 2 * backgroundWidth, currentPosition.y, currentPosition.z);

        // Set the UIContainer's position to the new position
        UIContainer.transform.position = nextPosition;
    }
}
