using UnityEngine;
using TMPro; // Import the TextMeshPro namespace

public enum CharacterType
{
    Knight,
    Wizard,
    Rogue,
    Enemy
}

public class Character : MonoBehaviour
{
    public string characterName;
    public int maxHealth;
    public int currentHealth;
    public int attackDamage;
    public int block; // Amount of block (temporary defense)

    public float evadeChance = 0f; // Evade chance (0.0 to 1.0)
    private int evadeChanceTurnsRemaining;

    public TextMeshPro healthText; // Reference to the TextMeshPro component for displaying health
    public TextMeshPro blockText; // Reference to the TextMeshPro component for displaying health
    public GameObject blockMarker; // Reference to the block marker object (blockText)
    public GameObject healthMarker; // Reference to the block marker object (blockText)
    public GameObject marker; // Reference to the marker object
    public GameObject intentMarker; // Reference to the intent marker object
    private SpriteRenderer markerRenderer; // Reference to the marker's SpriteRenderer to change its color

    public Sprite deathSprite;

    public Color defaultMarkerColor = Color.white; // Default marker color
    public Color hoverMarkerColor = Color.yellow; // Marker color when hovered
    public CharacterType characterType; // Add this to define the class of the character

    public bool markerIsActive;


    // Poison-related properties
    private int poisonDamage;
    private int poisonTurnsRemaining;

    private void Start()
    {
        // Initialize marker visibility and color
        marker.SetActive(false);
        markerIsActive = false;
        markerRenderer = marker.GetComponent<SpriteRenderer>();
        markerRenderer.color = defaultMarkerColor;

        // Update the health text at the start
        UpdateStatText();
    }

    // Initialize character stats and health text
    public void InitializeCharacter(string name, int health, int attack, CharacterType type)
    {
        characterName   = name;
        maxHealth       = health;
        currentHealth   = health;
        attackDamage    = attack;
        characterType   = type;

        HideIntentMarker();
        UpdateStatText();
    }

    public bool IsAlly()
    {
        return characterType == CharacterType.Rogue || characterType == CharacterType.Knight || characterType == CharacterType.Wizard;
    }

    public bool IsEnemy()
    {
        return characterType == CharacterType.Enemy;
    }

    // Method to gain block
    public void GainBlock(int amount)
    {
        block += amount;
        Debug.Log($"{characterName} gained {amount} block. Current block: {block}");

        UpdateStatText(); // Update UI after healing
    }

    // Method to heal the character
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Ensure health doesn't exceed maxHealth

        Debug.Log($"{characterName} healed for {amount}. Current health: {currentHealth}");

        UpdateStatText(); // Update UI after healing
    }

    // Update this method to consider evade
    public virtual void TakeDamage(int damage)
    {
        // Check for evade
        if (TryEvade())
        {
            Debug.Log($"{characterName} evaded the attack!");
            return; // No damage taken
        }

        if (block > 0)
        {
            // If the character has block, reduce damage from block first
            int remainingDamage = Mathf.Max(damage - block, 0);
            block = Mathf.Max(block - damage, 0);
            damage = remainingDamage;
        }

        // Apply any remaining damage to health
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Ensure health doesn't go below 0

        Debug.Log($"{characterName} took {damage} damage. Current health: {currentHealth}");

        // Update the health text
        UpdateStatText();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Poison-related methods
    public void ApplyPoison(int damagePerTurn, int turns)
    {
        poisonDamage = damagePerTurn;
        poisonTurnsRemaining += turns;
        Debug.Log($"{characterName} has been poisoned! Takes {damagePerTurn} damage for {turns} turns.");
    }

    // Method to check poison damage at the start of each turn
    public void CheckPoisonDamage()
    {
        if (poisonTurnsRemaining > 0)
        {
            TakeDamage(poisonDamage);
            poisonTurnsRemaining--;
            Debug.Log($"{characterName} took {poisonDamage} poison damage. {poisonTurnsRemaining} turns remaining.");
        }
    }


    // Method to apply evade chance for a turn~
    public void ApplyEvadeChance(float chance, int turns)
    {
        evadeChance = chance;
        evadeChanceTurnsRemaining += turns;
        Debug.Log($"{characterName} gained {evadeChance * 100}% evade chance!");
    }

    public void CheckEvade()
    {
        if (evadeChanceTurnsRemaining > 0)
        {
            evadeChanceTurnsRemaining--;
        }
        else 
        {
            ResetEvadeChance();
        }
    }

    // Method to determine if an attack is evaded
    public bool TryEvade()
    {
        if (evadeChanceTurnsRemaining > 0)
        {
            return Random.value < evadeChance; // Returns true if evaded
        }
        else
        {
            return false;
        }
    }
    
    // Function to reset evade chance (e.g., after a turn)
    public void ResetEvadeChance()
    {
        evadeChance = 0f;
    }

    // Method to perform an attack
    public virtual void Attack(Character target)
    {
        target.TakeDamage(attackDamage);
        Debug.Log($"{characterName} attacked {target.characterName} for {attackDamage} damage.");
    }

    // Function to update the health and block text display
    public void UpdateStatText()
    {
        if (healthText != null)
        {
            healthText.text = currentHealth.ToString();
        }

        // If block is greater than 0, display the block text and marker
        if (block > 0)
        {
            blockText.text = block.ToString();
            blockMarker.SetActive(true); // Show the block marker
        }
        else
        {
            blockMarker.SetActive(false); // Hide the block marker when block is 0
        }
    }

    // Function when character dies
    public void Die()
    {
        Debug.Log(characterName + " has died!");

        // Hide health and block markers
        if (healthMarker != null)
        {
            healthMarker.SetActive(false);
        }
        if (blockMarker != null)
        {
            blockMarker.SetActive(false);
        }

        // Show death sprite
        if (deathSprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = deathSprite;
        }

        // Notify the GameController to disable cards of this duck's class
        GameController.instance.OnCharacterDeath(characterType);
    }

    // Function to show the marker above the character
    public void ShowMarker()
    {
        marker.SetActive(true); // Make marker visible
        marker.GetComponent<Marker>().SetEnabled(true);
        marker.GetComponent<Marker>().SetHover(false);
        markerIsActive = true;
        markerRenderer.color = defaultMarkerColor; // Reset to default color
    }

    // Function to hide the marker
    public void HideMarker()
    {
        marker.SetActive(false); // Hide marker
        markerIsActive = false;
    }

    // Function to change the marker's color when hovered over
    private void OnMouseEnter()
    {
        if (markerIsActive)
        {
            marker.GetComponent<Marker>().SetEnabled(false);
            marker.GetComponent<Marker>().SetHover(true);
            markerRenderer.color = hoverMarkerColor; // Change color on hover
        }
    }

    private void OnMouseExit()
    {
        if (markerIsActive)
        {
            marker.GetComponent<Marker>().SetEnabled(true);
            marker.GetComponent<Marker>().SetHover(false);
            markerRenderer.color = defaultMarkerColor; // Revert color when hover ends
        }
    }

    // Handle clicking on the character to select them as a target
    private void OnMouseDown()
    {
        if (markerIsActive)
        {
            GameController.instance.OnTargetChosen(this.gameObject); // Notify GameController that this target was chosen
        }
    }

    // Call this at the start of each turn for all characters
    public void StartTurn()
    {
        // Apply poison damage at the start of the turn if the character is poisoned
        CheckPoisonDamage();
        CheckEvade();
        
        // Other turn-related logic...
    }

    // Function to show the intent marker
    public void ShowIntentMarker()
    {
        if (intentMarker != null)
        {
            intentMarker.SetActive(true); // Show the marker
        }
    }

    // Function to hide the intent marker
    public void HideIntentMarker()
    {
        if (intentMarker != null)
        {
            intentMarker.SetActive(false); // Hide the marker
        }
    }
}
