public enum CardType
{
    Rogue, Knight, Wizard, Neutral
}

public enum TargetType
{
    AllAllies,      // All allies
    Knight,         // A specific class of ally (e.g., Knight, Wizard)
    Wizard,         // A specific class of ally (e.g., Knight, Wizard)
    Rogue,          // A specific class of ally (e.g., Knight, Wizard)
    Enemy,          // Any enemy
    Any             // Any target (ally or enemy)
}

public class Card
{
    public string cardName;
    public CardType cardType;
    public TargetType targetType;   // Define which target this card can affect
    public int manaCost;
    public int primaryAmount;       // Generic value (e.g., for attack, heal, etc.)
    public int secondaryAmount;     // Generic value (e.g., for block, buff, etc.)

    // New field to track if the card is disabled
    public bool isDisabled;

    public Card(string name, CardType type, TargetType target, int mana, int primary, int secondary)
    {
        cardName            = name;
        cardType            = type;
        targetType          = target;
        manaCost            = mana;
        primaryAmount       = primary;
        secondaryAmount     = secondary;
        isDisabled          = false;
    }
}
