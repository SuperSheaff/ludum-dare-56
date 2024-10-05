using UnityEngine;
using TMPro;

public enum CardType { Rogue, Knight, Wizard, Neutral }

// This class holds only card data, not the MonoBehaviour
public class Card
{
    public string cardName;
    public CardType cardType;
    public int manaCost;
    public int attackPower;
    public int blockPower;

    public Card(string name, CardType type, int mana, int attack, int block)
    {
        cardName = name;
        cardType = type;
        manaCost = mana;
        attackPower = attack;
        blockPower = block;
    }
}
