using UnityEngine;
using TMPro; // For using TextMeshPro

public enum EnemyType
{
    Peasant,
    Knight,
    King
}

// Enemy class inheriting from Character
public class Enemy : Character
{
    public TextMeshPro damageText; // Text component to display the damage
    public GameObject damageMarker; // Reference to the damage marker object
    public SpriteRenderer spriteRenderer;

    public Sprite PeasantSprite;
    public Sprite KnightSprite;
    public Sprite KingSprite;

    public EnemyType enemyType;

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
    public override void InitializeCharacter(string name, int health, int attack, CharacterType type, Transform homeTransform)
    {
        base.InitializeCharacter(name, health, attack, type, homeTransform);

        // Update the damage display and marker
        UpdateDamageDisplay();
    }

    // Function to initialize the enemy based on its type
    public void InitializeEnemy(EnemyType type)
    {
        enemyType = type;

        // Assign sprite and stats based on enemy type
        switch (enemyType)
        {
            case EnemyType.Peasant:
                spriteRenderer.sprite = PeasantSprite;
                break;
            case EnemyType.Knight:
                spriteRenderer.sprite = KnightSprite;
                break;
            case EnemyType.King:
                spriteRenderer.sprite = KingSprite;
                break;
        }

        UpdateDamageDisplay();
    }

        // Initialize the enemy's stats (overrides the base InitializeCharacter method)
    public override void UpdateStatText()
    {
        base.UpdateStatText();

        // Update the damage display and marker
        UpdateDamageDisplay();
    }

    // Function to update the damage display on spawn
    private void UpdateDamageDisplay()
    {
        // If block is greater than 0, display the block text and marker
        if (currentHealth > 0)
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
        else
        {
            damageMarker.SetActive(false); // Hide the block marker when block is 0
        }
    }

    // Function to handle attack logic (overrides the base Attack method)
    public override void Attack(Character target)
    {
        base.Attack(target); // Call the base class method to handle attack and apply damage

        // Optionally, add any additional logic for enemy-specific attacks here
    }
}
