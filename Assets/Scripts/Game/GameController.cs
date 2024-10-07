using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public GameSettings gameSettings;

    public StateMachine<GameController> stateMachine;
    public GameIntroState gameIntroState;
    public GameStartState gameStartState;
    public PlayerTurnState playerTurnState;
    public EnemyTurnState enemyTurnState;
    public ChooseRewardState chooseRewardState;
    public GameWinState gameWinState;

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
    public GameObject gameWinScreen;
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
    public float cardSpacing    = 2.0f; // Adjust this for more or less spread
    public float cardYOffset    = -3.0f; // Adjust this to move the cards lower on the screen

    public List<Enemy> Enemies; // List of all enemy characters
    public List<Duck> DuckParty;  // List of all ally characters
    public GameObject chooseTargetText;     // Text for "Choose Target"

    private Card currentCard;               // The card currently being played
    private CardDisplay currentCardDisplay; // The CardDisplay of the current card

    public int startingMana     = 5;
    public int baseMana         = 5;
    public int currentMana      = 5;    // Example mana value, this can be dynamic
    public TextMeshPro manaText;        // Reference to the TextMeshPro component for displaying health
    public GameObject chooseNewCardText;        // Reference to the TextMeshPro component for displaying health

    public GameObject TilesParent; // Parent for background tiles
    public GameObject dungeonTilePrefab;
    public GameObject wallTilePrefab;

    private List<GameObject> backgroundTiles = new List<GameObject>();

    public float backgroundWidth    = 38.4f;  // Width of each background tile
    private int currentRoomIndex    = 0;    // Keep track of which room the player is currently in

    public int baseEnemyHealth      = 1;  // Starting health for enemies
    public int baseEnemyDamage      = 5;   // Starting damage for enemies
    public float healthMultiplier   = 1.23f;  // Scaling factor for health
    public float damageMultiplier   = 1.23f;  // Scaling factor for damage
    
    // Add a random variation factor
    public float randomHealthVariation = 0.23f;  // 10% variation in health scaling
    public float randomDamageVariation = 0.23f;  // 10% variation in damage scaling

    public int currentLevel = 1;        // Track the current level
    public TextMeshPro levelText;        // Reference to the TextMeshPro component for displaying health
    public TextMeshPro drawPileText;        // Reference to the TextMeshPro component for displaying health
    public TextMeshPro discardPileText;        // Reference to the TextMeshPro component for displaying health

    // Predefined enemy health and damage for each level
    public List<int> enemyHealthPerLevel = new List<int>() { 10, 20, 30, 40 }; // Example values
    public List<int> enemyDamagePerLevel = new List<int>() { 5, 10, 15, 20 };  // Example values

    public GameObject startButtonObject;
    public GameObject introLogoObject;
    public GameObject introScreen;
    public TextMeshPro introAuthors;

    public TextMeshPro introText1;
    public TextMeshPro introText2;
    public TextMeshPro introText3;

    public bool CanStartGame = false;

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
        InitializeStateMachine();
    }

    private void Update()
    {
        stateMachine.Update();

        UpdateUI();

        if (CanStartGame)
        {
            // Check if the left mouse button is clicked (0 is the left button, 1 is the right button, 2 is the middle button)
            if (Input.GetMouseButtonDown(0))
            {
                StartIntroThirdPart(); // Call your StartGame function when the mouse button is clicked
                CanStartGame = false;
            }
        }
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    private void InitializeStateMachine()
    {
        stateMachine        = new StateMachine<GameController>(true);

        gameIntroState      = new GameIntroState(this);
        gameStartState      = new GameStartState(this);
        playerTurnState     = new PlayerTurnState(this);
        enemyTurnState      = new EnemyTurnState(this);
        chooseRewardState   = new ChooseRewardState(this);
        gameWinState        = new GameWinState(this);

        stateMachine.Initialize(gameIntroState);
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
        rogueDuck.InitializeCharacter("Rogue Duck", 10, 10, CharacterType.Rogue, rogueSpawnPoint);
        rogueDuck.InitializeDuck(DuckType.Rogue);
        allCharacters.Add(rogueDuck); // Add Rogue Duck to available targets
        DuckParty.Add(rogueDuck); // Add Rogue Duck to available targets

        knightDuck.InitializeCharacter("Knight Duck", 15, 8, CharacterType.Knight, knightSpawnPoint);
        knightDuck.InitializeDuck(DuckType.Knight);
        allCharacters.Add(knightDuck); // Add Knight Duck to available targets
        DuckParty.Add(knightDuck); // Add Knight Duck to available targets

        wizardDuck.InitializeCharacter("Wizard Duck", 8, 12, CharacterType.Wizard, wizardSpawnPoint);
        wizardDuck.InitializeDuck(DuckType.Wizard);
        allCharacters.Add(wizardDuck); // Add Wizard Duck to available targets
        DuckParty.Add(wizardDuck); // Add Wizard Duck to available targets

        // Initialize enemy with health and attack values
        enemy.InitializeCharacter("Enemy", enemyHealthPerLevel[0], enemyDamagePerLevel[0], CharacterType.Enemy, enemySpawnPoint);
        enemy.InitializeEnemy(EnemyType.Peasant);
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
    public void PlayCard(Character target, Card playedCard)
    {
        StartCoroutine(IE_PlayCard(target, playedCard));
    }

    // Apply the card's effect to the target
    private IEnumerator IE_PlayCard(Character target, Card playedCard)
    {

        // Hide all markers after selection
        HideAllMarkers();



        // Move the card data to the discard pile
        CardController.instance.DiscardCard(playedCard);

        // Hide "Choose Target" text
        chooseTargetText.gameObject.SetActive(false);
        unselectButton.SetActive(false);
        endTurnButton.SetActive(true);

        yield return new WaitForSeconds(0.2f);


        // Change color based on card type
        switch (playedCard.cardName)
        {

            // Apply evade chance to all ally ducks (50% chance to evade for one turn)
            case "Smoke Bomb":
                foreach (Character character in allCharacters)
                {
                    if (character.IsAlly()) // Ensure it's a duck
                    {
                        character.ApplyEvadeChance(0.5f, playedCard.primaryAmount); // 50% evade chance for 1 turn
                    }
                }
                yield return new WaitForSeconds(0.3f);
                SoundManager.instance.PlaySound("Quack1", this.transform, true);
                yield return new WaitForSeconds(0.3f);
                PlayCharacterAnimation(CharacterType.Rogue, "attack");
                break;

            // Apply poison for primaryAmount damage over secondaryAmount turns
            case "Poison":
                yield return new WaitForSeconds(0.3f);
                SoundManager.instance.PlaySound("Quack1", this.transform, true);
                yield return new WaitForSeconds(0.3f);
                PlayCharacterAnimation(CharacterType.Rogue, "attack");
                target.ApplyPoison(playedCard.primaryAmount, playedCard.secondaryAmount); 

                break;

            // Apply heal for primaryAmount to target
            case "Heal":
                yield return new WaitForSeconds(0.3f);
                SoundManager.instance.PlaySound("Quack1", this.transform, true);
                yield return new WaitForSeconds(0.3f);
                PlayCharacterAnimation(CharacterType.Wizard, "attack");
                target.Heal(playedCard.primaryAmount);
                break;

            // Apply damage for primaryAmount to target
            case "Fireball":
                yield return new WaitForSeconds(0.3f);
                SoundManager.instance.PlaySound("Quack1", this.transform, true);
                yield return new WaitForSeconds(0.3f);
                PlayCharacterAnimation(CharacterType.Wizard, "attack");
                target.TakeDamage(playedCard.primaryAmount);
                break;

            // Apply damage for primaryAmount to target
            case "Shiv":
                yield return new WaitForSeconds(0.3f);
                SoundManager.instance.PlaySound("Quack1", this.transform, true);
                yield return new WaitForSeconds(0.3f);
                PlayCharacterAnimation(CharacterType.Rogue, "attack");
                target.TakeDamage(playedCard.primaryAmount);
                break;

            // Apply damage for primaryAmount to target
            case "Renew":
                yield return new WaitForSeconds(0.3f);
                SoundManager.instance.PlaySound("Quack1", this.transform, true);
                yield return new WaitForSeconds(0.3f);
                PlayCharacterAnimation(CharacterType.Wizard, "attack");
                currentMana++;
                break;

            // Apply damage for primaryAmount to target
            case "Shield Bash":
                yield return new WaitForSeconds(0.3f);
                SoundManager.instance.PlaySound("Quack1", this.transform, true);
                yield return new WaitForSeconds(0.3f);
                PlayCharacterAnimation(CharacterType.Knight, "attack");
                target.TakeDamage(playedCard.primaryAmount);
                yield return new WaitForSeconds(0.3f);
                SoundManager.instance.PlaySound("Quack1", this.transform, true);
                yield return new WaitForSeconds(0.3f);
                knightDuck.GainBlock(playedCard.secondaryAmount);
                break;

            // Set the enemy's intent to the Knight Duck
            case "Taunt":
                yield return new WaitForSeconds(0.3f);
                SoundManager.instance.PlaySound("Quack1", this.transform, true);
                yield return new WaitForSeconds(0.3f);
                SoundManager.instance.PlaySound("Quack1", this.transform, true);
                yield return new WaitForSeconds(0.3f);
                PlayCharacterAnimation(CharacterType.Knight, "attack");
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
                yield return new WaitForSeconds(0.3f);
                SoundManager.instance.PlaySound("Quack1", this.transform, true);
                yield return new WaitForSeconds(0.3f);
                PlayCharacterAnimation(CharacterType.Knight, "attack");
                yield return new WaitForSeconds(0.3f);
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
                yield return new WaitForSeconds(0.3f);
                SoundManager.instance.PlaySound("Quack1", this.transform, true);
                yield return new WaitForSeconds(0.3f);
                PlayCharacterAnimation(CharacterType.Knight, "attack");
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

        yield return new WaitForSeconds(0.5f);
    }

    public void PlayCharacterAnimation(CharacterType characterType, string animationName, bool random = false)
    {
        Character targetCharacter = null;

        if (random)
        {
            // Create a list of alive ducks
            List<Character> aliveDucks = new List<Character>();

            if (rogueDuck.currentHealth > 0)
            {
                aliveDucks.Add(rogueDuck);
            }
            if (knightDuck.currentHealth > 0)
            {
                aliveDucks.Add(knightDuck);
            }
            if (wizardDuck.currentHealth > 0)
            {
                aliveDucks.Add(wizardDuck);
            }

            // If there are any alive ducks, choose one randomly
            if (aliveDucks.Count > 0)
            {
                targetCharacter = aliveDucks[Random.Range(0, aliveDucks.Count)];
                characterType = targetCharacter.characterType;
            }
            else
            {
                Debug.LogError("No alive ducks found!");
            }
        }


        // Find the correct character based on character type
        switch (characterType)
        {
            case CharacterType.Rogue:
                targetCharacter = rogueDuck;
                break;
            case CharacterType.Knight:
                targetCharacter = knightDuck;
                break;
            case CharacterType.Wizard:
                targetCharacter = wizardDuck;
                break;
            case CharacterType.Enemy:
                targetCharacter = enemy;
                break;
            default:
                Debug.LogError("Invalid character type specified!");
                return;
        }

        if (targetCharacter != null)
        {
            // Use a switch statement to define different animations based on the animation name
            switch (animationName.ToLower())
            {
                case "attack":
                    targetCharacter.PlayAttackAnimation();
                    break;

                case "block":
                    Debug.Log($"{targetCharacter.characterName} is playing the Block animation.");
                    break;

                default:
                    Debug.LogError($"Animation {animationName} not recognized for {targetCharacter.characterName}.");
                    break;
            }
        
        }
        else
        {
            Debug.LogError("Target character is null!");
        }
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
            if (currentLevel < 8)
            {
                // Change state
                stateMachine.ChangeState(new ChooseRewardState(this));
            }
            else
            {
                stateMachine.ChangeState(gameWinState);
            }
        }
        else
        {
            CardController.instance.DisableCardsByType(characterType);
        }
    }

    // Method to trigger the game-over screen
    public void TriggerGameOver()
    {
        startGameOverProcess();
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
        yield return new WaitForSeconds(0.3f);

        float duration = 2f; // Adjust the time it takes to move
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

        // Clear the previous enemy
        if (enemy != null)
        {
            allCharacters.Remove(enemy);  // Remove enemy from allCharacters list
            Enemies.Remove(enemy);        // Remove enemy from Enemies list
            Destroy(enemy.gameObject);    // Destroy the enemy GameObject
        }

        // Define the enemy type for this level (you can choose how to assign enemy types based on the level)
       EnemyType enemyType;
        if (currentLevel < 5) 
        {
            enemyType = EnemyType.Peasant;
        }
        else if (currentLevel >= 5 && currentLevel < 8) 
        {
            enemyType = EnemyType.Knight;
        }
        else 
        {
            enemyType = EnemyType.King;
        }


        // Get health and damage based on the current level
        int enemyHealth = enemyHealthPerLevel[currentLevel - 1];
        int enemyDamage = enemyDamagePerLevel[currentLevel - 1];

        // Spawn a new enemy based on the type
        Vector3 enemySpawnPosition = enemySpawnPoint.position; 
        enemy = Instantiate(enemyPrefab, enemySpawnPosition, Quaternion.identity).GetComponent<Enemy>();
        enemy.InitializeCharacter("Enemy", enemyHealth, enemyDamage, CharacterType.Enemy, enemySpawnPoint);
        enemy.InitializeEnemy(enemyType); // Initialize enemy based on type

        allCharacters.Add(enemy); // Add Enemy to available targets
        Enemies.Add(enemy); // Add Enemy to available targets


        // Optionally: Reset player status effects, health, or block here
        foreach (Duck duck in DuckParty)
        {
            duck.block = 0;
            duck.ResetEvadeChance();
            duck.UpdateStatText();
            duck.SetPositionAtHome();

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

    public void RestartGame()
    {
        StartCoroutine(IE_RestartGame());
    }

    public IEnumerator IE_RestartGame()
    {
        Destroy(enemy.gameObject); 

        // Step 1: Clear all characters and enemies
        foreach (var character in allCharacters)
        {
            Destroy(character.gameObject); // Destroy each character's GameObject
        }
        // Step 1: Clear all characters and enemies
        foreach (var character in Enemies)
        {
            Destroy(character.gameObject); // Destroy each character's GameObject
        }


        allCharacters.Clear();  // Clear the list of all characters
        DuckParty.Clear();      // Clear the list of ducks (allies)
        Enemies.Clear();        // Clear the list of enemies

        enemy       = null;
        rogueDuck   = null;
        wizardDuck  = null;
        knightDuck  = null;

        // yield return new WaitForSeconds(2f);

        // Step 2: Reset the camera to its original position (assuming the camera starts at zero)
        CameraController.instance.transform.position    = new Vector3(0, 0, -500);
        UIContainer.transform.position                  = new Vector3(0, 0, 0);
        ducksParent.transform.position                  = new Vector3(0, 0, 0);

        // Step 3: Clear background tiles
        foreach (var tile in backgroundTiles)
        {
            Destroy(tile.gameObject); // Destroy each tile GameObject
        }
        backgroundTiles.Clear();  // Clear the list of background tiles

        // Step 8: Reset mana, level, and UI
        currentMana = baseMana;
        currentLevel = 1;
        UpdateUI();  // Update UI elements like mana, level, draw pile, etc.


        // Step 4: Reset the current room index
        currentRoomIndex = 0;

        // Step 6: Generate the initial background tiles again
        GenerateBackgroundTiles();

        // Step 7: Reset the card controller (new function you'll add)
        yield return new WaitForSeconds(0.1f);

        CardController.instance.ResetCards(); // Resets the deck, draw pile, discard pile, etc.

        // Step 9: Reset state machine back to the game start state
        stateMachine.ChangeState(gameStartState);

        // Step 10: Hide any game over or other screens
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false); // Hide game-over screen
            gameWinScreen.SetActive(false); // Hide game-over screen
        }
      // Step 5: Reinitialize characters
        InitializeCharacters(); // Recreate the ducks and enemies with initial values

        enemy.transform.position = enemySpawnPoint.transform.position;
        Debug.Log("Game restarted successfully.");
    }

    public void startGameWinProcess()
    {
        SoundManager.instance.FadeOutSound("GameMusic", 1f);

        CardController.instance.ClearAllCards();
        StartCoroutine(gameWinProcess());
    }

    private IEnumerator gameWinProcess()
    {
        SoundManager.instance.PlaySound("GameWin", this.transform, false);
        yield return new WaitForSeconds(3f);
        StartCoroutine(SlideInObject(gameWinScreen, new Vector3(CameraController.instance.transform.position.x, CameraController.instance.transform.position.y, 50f), 1.6f, 1f));
        yield return new WaitForSeconds(1f);
        SoundManager.instance.PlaySound("Quack1", this.transform, false);
    }

    public void startGameOverProcess()
    {
        SoundManager.instance.FadeOutSound("GameMusic", 1f);
        CardController.instance.ClearAllCards();
        StartCoroutine(gameOverProcess());
    }

    private IEnumerator gameOverProcess()
    {
        yield return new WaitForSeconds(3f);
        SoundManager.instance.PlaySound("GameOver", this.transform, false);
        StartCoroutine(SlideInObject(gameOverScreen, UIContainer.transform.position, 1.6f, 1f));
        yield return new WaitForSeconds(1f);
        SoundManager.instance.PlaySound("Quack1", this.transform, false);
    }


    private IEnumerator SlideInObject(GameObject obj, Vector3 targetPosition, float overshootDistance, float duration)
    {
        // Move the object to a starting position above the screen
        Vector3 startPosition = targetPosition + new Vector3(0, 50f, 0); // Well above the screen, adjust 1000f as needed
        obj.transform.position = startPosition;
        obj.SetActive(true); // Activate the object before the slide animation

        float elapsedTime = 0f;
        Vector3 overshootPosition = targetPosition - new Vector3(0, overshootDistance, 0); // Position slightly below the target

        // Slide from above to the overshoot position
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            obj.transform.position = Vector3.Lerp(startPosition, overshootPosition, elapsedTime / duration);
            yield return null;
        }

        // Snap to overshoot position to ensure accuracy
        obj.transform.position = overshootPosition;

        // Bounce back to the final position (center) with a slight delay
        elapsedTime = 0f;
        while (elapsedTime < duration * 0.5f) // Bounce-back time is half of the slide duration
        {
            elapsedTime += Time.deltaTime;
            obj.transform.position = Vector3.Lerp(overshootPosition, targetPosition, elapsedTime / (duration * 0.5f));
            yield return null;
        }

        // Ensure it snaps exactly to the center position
        obj.transform.position = targetPosition;
    }


    public void StartIntro()
    {
        StartCoroutine(IE_StartIntro());
    }

    private IEnumerator IE_StartIntro()
    {
        // Step 1: Start playing the music
        SoundManager.instance.PlaySound("IntroMusic", this.transform);

        // Step 2: Fade in the logo
        yield return StartCoroutine(FadeInSprite(introLogoObject, 2f)); // 2 seconds to fade in

        // Step 3: Wait for a short duration before showing the button
        yield return new WaitForSeconds(1f); // Optional wait time before showing the button

        // Step 4: Fade in the button
        yield return StartCoroutine(FadeInText(introAuthors, 1f)); // 1 second to fade in the button
        
        // Step 3: Wait for a short duration before showing the button
        yield return new WaitForSeconds(1f); // Optional wait time before showing the button

        // Step 4: Fade in the button
        yield return StartCoroutine(FadeInSprite(startButtonObject, 1f)); // 1 second to fade in the button
    }

    // Function to fade in a GameObject with a SpriteRenderer
    private IEnumerator FadeInSprite(GameObject uiElement, float duration)
    {
        SpriteRenderer spriteRenderer = uiElement.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on " + uiElement.name);
            yield break;
        }

        float elapsedTime = 0f;
        Color initialColor = spriteRenderer.color;
        initialColor.a = 0; // Start at 0 opacity
        spriteRenderer.color = initialColor; // Set the starting color

        uiElement.SetActive(true); // Ensure the UI element is active

        // Gradually increase the alpha over the duration
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        // Ensure the final alpha is set to 1
        spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, 1);
    }

    // Function to fade out a GameObject with a SpriteRenderer
    private IEnumerator FadeOutSprite(GameObject uiElement, float duration)
    {
        SpriteRenderer spriteRenderer = uiElement.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on " + uiElement.name);
            yield break;
        }

        float elapsedTime = 0f;
        Color initialColor = spriteRenderer.color;
        initialColor.a = 1; // Start at full opacity
        spriteRenderer.color = initialColor; // Set the starting color

        // Gradually decrease the alpha over the duration
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsedTime / duration)); // Alpha decreases from 1 to 0
            spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        // Ensure the final alpha is set to 0
        spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0);

        uiElement.SetActive(false); // Optionally deactivate the GameObject after fading out
    }

    public void StartIntroSecondPart()
    {
        StartCoroutine(IE_StartIntroSecondPart());
    }

    private IEnumerator IE_StartIntroSecondPart()
    {
        // Step 2: Fade in the logo
        StartCoroutine(FadeOutSprite(introLogoObject, 1f)); // 2 seconds to fade in

        StartCoroutine(FadeOutText(introAuthors, 1f)); // 1 second to fade in the button

        // Step 4: Fade in the button
        StartCoroutine(FadeOutSprite(startButtonObject, 1f)); // 1 second to fade in the button

        // Step 3: Wait for a short duration before showing the button
        yield return new WaitForSeconds(1f); // Optional wait time before showing the button

        yield return StartCoroutine(FadeInText(introText1, 1f)); // 1 second to fade in the button

        yield return new WaitForSeconds(1f); // Optional wait time before showing the button

        yield return StartCoroutine(FadeInText(introText2, 1f)); // 1 second to fade in the button

        yield return new WaitForSeconds(1f); // Optional wait time before showing the button

        CanStartGame = true;

        yield return StartCoroutine(FadeInText(introText3, 1f)); // 1 second to fade in the button
    }


    // Function to fade in a TextMeshPro object
    private IEnumerator FadeInText(TextMeshPro textElement, float duration)
    {
        if (textElement == null)
        {
            Debug.LogError("TextMeshProUGUI not found!");
            yield break;
        }

        float elapsedTime = 0f;
        Color initialColor = textElement.color;
        initialColor.a = 0; // Start at 0 opacity
        textElement.color = initialColor; // Set the starting color

        // Gradually increase the alpha over the duration
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            textElement.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        // Ensure the final alpha is set to 1
        textElement.color = new Color(initialColor.r, initialColor.g, initialColor.b, 1);
    }

    // Function to fade out a TextMeshPro object
    private IEnumerator FadeOutText(TextMeshPro textElement, float duration)
    {
        if (textElement == null)
        {
            Debug.LogError("TextMeshProUGUI not found!");
            yield break;
        }

        float elapsedTime = 0f;
        Color initialColor = textElement.color;
        initialColor.a = 1; // Start at full opacity
        textElement.color = initialColor; // Set the starting color

        // Gradually decrease the alpha over the duration
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsedTime / duration)); // Alpha decreases from 1 to 0
            textElement.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        // Ensure the final alpha is set to 0
        textElement.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
    }


    public void StartIntroThirdPart()
    {
        StartCoroutine(IE_StartIntroThirdPart());
    }

    private IEnumerator IE_StartIntroThirdPart()
    {
        // Step 1: Start playing the music
        SoundManager.instance.FadeOutSound("IntroMusic", 1f);
        // Step 3: Wait for a short duration before showing the button
        yield return new WaitForSeconds(1f); // Optional wait time before showing the button
        
        InitializeCharacters();
        GenerateBackgroundTiles();

        StartCoroutine(FadeOutText(introText1, 1f)); // 1 second to fade in the button
        StartCoroutine(FadeOutText(introText2, 1f)); // 1 second to fade in the button
        StartCoroutine(FadeOutText(introText3, 1f)); // 1 second to fade in the button
        yield return new WaitForSeconds(1f); // Optional wait time before showing the button
        yield return StartCoroutine(FadeOutSprite(introScreen, 1f)); // 1 second to fade in the button


        // Initialize the deck in CardController
        CardController.instance.InitializeDeck();

        // Shuffle the deck and fill the draw pile
        CardController.instance.ShuffleDeckIntoDrawPile();

        stateMachine.ChangeState(gameStartState);
    }
}

