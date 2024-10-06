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

        // Tick
        foreach (Character character in gameController.allCharacters)
        {
            if (character.IsAlly())
            {
                character.StartTurn();
                character.HideIntentMarker();
            }
        }

        // Assign enemy intent by selecting a random duck as the target
        AssignEnemyIntent();
    }

    public override void Update()
    {
        // Handle player's card interaction, etc.
    }

    public override void Exit()
    {
        Debug.Log("Player's turn has ended.");
    }

    private void AssignEnemyIntent()
    {
        // Create an array of ducks
        Duck[] ducks = { gameController.rogueDuck, gameController.knightDuck, gameController.wizardDuck };

        // Filter out only the ducks that are alive
        Duck[] aliveDucks = System.Array.FindAll(ducks, duck => duck.currentHealth > 0);

        // If no ducks are alive, show the game over screen
        if (aliveDucks.Length == 0)
        {
            Debug.LogWarning("All ducks are dead! Game Over.");
            gameController.TriggerGameOver(); // Trigger game-over
            return; // Exit if no valid target is found
        }

        // Choose a random duck from the alive ducks
        Duck randomDuck = aliveDucks[Random.Range(0, aliveDucks.Length)];

        // Set the enemy's target to the valid duck
        gameController.enemy.SetTarget(randomDuck);

        // Show intent marker on the chosen duck
        randomDuck.ShowIntentMarker();

        Debug.Log($"Enemy intends to attack {randomDuck.characterName}.");
    }

}
