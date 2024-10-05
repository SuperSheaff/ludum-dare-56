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
}