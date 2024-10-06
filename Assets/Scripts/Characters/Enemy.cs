using UnityEngine;
using TMPro; // For using TextMeshPro

// Enemy class inheriting from Character
public class Enemy : Character
{
    public TextMeshPro damageText; // Text component to display the damage
    public GameObject damageMarker; // Reference to the damage marker object

    private Duck targetDuck; // The duck chosen as the target for the enemy's attack

    // Set the enemy's target duck
    public void SetTarget(Duck duck)
    {
        targetDuck = duck;
    }

    // Get the enemy's target duck
    public Duck GetTarget()
    {
        return targetDuck;
    }

    // Initialize the enemy's stats (overrides the base InitializeCharacter method)
    public override void InitializeCharacter(string name, int health, int attack, CharacterType type)
    {
        base.InitializeCharacter(name, health, attack, type);

        // Update the damage display and marker
        UpdateDamageDisplay();
    }

    // Function to update the damage display on spawn
    private void UpdateDamageDisplay()
    {
        if (damageText != null)
        {
            damageText.text = attackDamage.ToString(); // Set the text to the current attack damage
        }

        // Enable the damage marker if damage is greater than 0
        if (damageMarker != null)
        {
            damageMarker.SetActive(attackDamage > 0);
        }
    }

    // Function to handle attack logic (overrides the base Attack method)
    public override void Attack(Character target)
    {
        base.Attack(target); // Call the base class method to handle attack and apply damage

        // Optionally, add any additional logic for enemy-specific attacks here
    }
}
