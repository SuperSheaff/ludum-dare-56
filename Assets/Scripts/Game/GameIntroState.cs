using UnityEngine;

public class GameIntroState : GameState
{
    public GameIntroState(GameController gameController) : base(gameController) { }

    public override void Enter()
    {
        gameController.StartIntro();
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}
