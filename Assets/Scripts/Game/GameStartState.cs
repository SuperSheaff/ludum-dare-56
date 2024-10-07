using UnityEngine;

public class GameStartState : GameState
{
    public GameStartState(GameController gameController) : base(gameController) { }

    public override void Enter()
    {
        SoundManager.instance.PlaySound("GameMusic", gameController.transform);
    }

    public override void Update()
    {
        gameController.stateMachine.ChangeState(gameController.playerTurnState);
    }

    public override void Exit()
    {
    }
}
