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

    public TextMeshPro healthText; // Reference to the TextMeshPro component for displaying health
    public GameObject marker; // Reference to the marker object
    private SpriteRenderer markerRenderer; // Reference to the marker's SpriteRenderer to change its color

    public Color defaultMarkerColor = Color.white; // Default marker color
    public Color hoverMarkerColor = Color.yellow; // Marker color when hovered
    public CharacterType characterType; // Add this to define the class of the character

    private void Start()
    {
        // Initialize marker visibility and color
        marker.SetActive(false);
        markerRenderer = marker.GetComponent<SpriteRenderer>();
        markerRenderer.color = defaultMarkerColor;

        // Update the health text at the start
        UpdateHealthText();
    }

    // Initialize character stats and health text
    public void InitializeCharacter(string name, int health, int attack, CharacterType type)
    {
        characterName = name;
        maxHealth = health;
        currentHealth = health;
        attackDamage = attack;
        characterType = type;

        UpdateHealthText();
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
    }

    // Method to take damage, factoring in block
    public virtual void TakeDamage(int damage)
    {
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
        UpdateHealthText();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method to perform an attack
    public virtual void Attack(Character target)
    {
        target.TakeDamage(attackDamage);
        Debug.Log($"{characterName} attacked {target.characterName} for {attackDamage} damage.");
    }

    // Function to update the health text display
    public void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = currentHealth.ToString();
        }
    }

    // Function when character dies
    protected virtual void Die()
    {
        Debug.Log(characterName + " has died!");
        // Add death logic (e.g., disable the character, etc.)
    }

    // Function to show the marker above the character
    public void ShowMarker()
    {
        marker.SetActive(true); // Make marker visible
        markerRenderer.color = defaultMarkerColor; // Reset to default color
    }

    // Function to hide the marker
    public void HideMarker()
    {
        marker.SetActive(false); // Hide marker
    }

    // Function to change the marker's color when hovered over
    private void OnMouseEnter()
    {
        if (marker.activeInHierarchy)
        {
            markerRenderer.color = hoverMarkerColor; // Change color on hover
        }
    }

    private void OnMouseExit()
    {
        if (marker.activeInHierarchy)
        {
            markerRenderer.color = defaultMarkerColor; // Revert color when hover ends
        }
    }

    // Handle clicking on the character to select them as a target
    private void OnMouseDown()
    {
        if (marker.activeInHierarchy)
        {
            GameController.instance.OnTargetChosen(this.gameObject); // Notify GameController that this target was chosen
        }
    }
}
