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
    public TargetType targetType; // Define which target this card can affect
    public int manaCost;
    public int attackPower;
    public int blockPower;

    public Card(string name, CardType type, TargetType target, int mana, int attack, int block)
    {
        cardName = name;
        cardType = type;
        targetType = target;
        manaCost = mana;
        attackPower = attack;
        blockPower = block;
    }
}
