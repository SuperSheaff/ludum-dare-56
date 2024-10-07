using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

public class ChooseRewardState : GameState
{
    public ChooseRewardState(GameController gameController) : base(gameController) { }

    public override void Enter()
    {
        Debug.Log("Player's turn has started.");

        gameController.endTurnButton.SetActive(false);
        gameController.unselectButton.SetActive(false);

        // Move ducks to the next dungeon tile
        CardController.instance.ClearAllCards();

        // Move ducks to the next dungeon tile
        gameController.MoveDucksToNextRoom();

        CameraController.instance.MoveCameraToNextTile(gameController.GenerateNextRoomLocation(), 1f, true);

        // Increase the level
        gameController.currentLevel++;

        gameController.GenerateBackgroundTiles();

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
