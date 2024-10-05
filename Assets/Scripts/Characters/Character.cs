using UnityEngine;
using TMPro; // Import the TextMeshPro namespace

public class Character : MonoBehaviour
{
    public string characterName;
    public int maxHealth;
    public int currentHealth;
    public int attackDamage;
    public int block;         // Amount of block (temporary defense)

    public TextMeshPro healthText; // Reference to the TextMeshPro component for displaying health

    // Initialize character stats and health text
    public void InitializeCharacter(string name, int health, int attack)
    {
        characterName = name;
        maxHealth = health;
        currentHealth = health;
        attackDamage = attack;
        block = 0; // Initialize with 0 block

        // Update the health text at the start
        UpdateHealthText();
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
}
