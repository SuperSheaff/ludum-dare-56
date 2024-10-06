using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

public class ChooseRewardState : GameState
{
    public ChooseRewardState(GameController gameController) : base(gameController) { }

    public override void Enter()
    {
        Debug.Log("Player's turn has started.");

        
        // Revive ducks
        foreach (Duck duck in gameController.DuckParty)
        {
            if (duck.currentHealth <= 0)
            {
                duck.Revive(3); // Revive with 3 health
            }
        }

        gameController.endTurnButton.SetActive(false);
        gameController.unselectButton.SetActive(false);
        gameController.nextRoomButton.SetActive(true);

        // Move ducks to the next dungeon tile
        CardController.instance.ClearAllCards();

        // Move ducks to the next dungeon tile
        gameController.MoveDucksToNextRoom();

        CameraController.instance.MoveCameraToNextTile(gameController.GenerateNextRoomLocation(), 1f);

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
