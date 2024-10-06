using UnityEngine;
// Enum for the different types of ducks

public enum DuckType
{
    Rogue,
    Knight,
    Wizard
}

// Duck class inheriting from Character
public class Duck : Character
{
    public DuckType duckType; // Enum to differentiate duck types

    public SpriteRenderer spriteRenderer;
    public Sprite rogueSprite;
    public Sprite knightSprite;
    public Sprite wizardSprite;

    public void InitializeDuck(DuckType type)
    {
        duckType = type;

        // Assign the correct sprite based on duck type
        switch (duckType)
        {
            case DuckType.Rogue:
                spriteRenderer.sprite = rogueSprite;
                break;
            case DuckType.Knight:
                spriteRenderer.sprite = knightSprite;
                break;
            case DuckType.Wizard:
                spriteRenderer.sprite = wizardSprite;
                break;
        }
    }

    public void Revive(int reviveHealth)
    {
        // Revive the duck with a set amount of health, but don't exceed maxHealth
        currentHealth = Mathf.Min(reviveHealth, maxHealth);

        // Re-enable the health marker and update the health UI
        healthMarker.SetActive(true);
        UpdateStatText();

        // Ensure the sprite is active again if it was disabled upon death
        spriteRenderer.enabled = true;

                // Assign the correct sprite based on duck type
        switch (duckType)
        {
            case DuckType.Rogue:
                spriteRenderer.sprite = rogueSprite;
                break;
            case DuckType.Knight:
                spriteRenderer.sprite = knightSprite;
                break;
            case DuckType.Wizard:
                spriteRenderer.sprite = wizardSprite;
                break;
        }
    }
}