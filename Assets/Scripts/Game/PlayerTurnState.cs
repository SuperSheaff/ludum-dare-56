using UnityEngine;

public class PlayerTurnState : GameState
{
    public PlayerTurnState(GameController gameController) : base(gameController) { }

    public override void Enter()
    {
        Debug.Log("Player's turn has started.");
        
        // Draw cards at the start of the player's turn
        gameController.ResetMana();
        CardController.instance.CheckDrawPile(); 
        CardController.instance.DrawHand(); 
        gameController.endTurnButton.SetActive(true); // Show the End Turn button
    }

    public override void Update()
    {
        // Handle player's card interaction, etc.
    }

    public override void Exit()
    {
        Debug.Log("Player's turn has ended.");
    }
}
