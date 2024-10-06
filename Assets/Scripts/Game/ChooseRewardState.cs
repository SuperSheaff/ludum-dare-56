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

        // Move ducks to the next dungeon tile
        gameController.MoveDucksToNextRoom();

        // Move the camera to the center of the next tile
        CameraController.instance.MoveCamera(new Vector3(gameController.nextRoomX, 0, -10), () =>
        {
            // Camera movement is complete, now generate the reward cards
            gameController.GenerateRewards();
        });
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
