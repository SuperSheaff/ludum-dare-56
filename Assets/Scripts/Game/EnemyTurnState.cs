using UnityEngine;

public class EnemyTurnState : GameState
{
    public EnemyTurnState(GameController gameController) : base(gameController) { }

    public override void Enter()
    {
        Debug.Log("Enemy's turn has started.");

        // Tick
        foreach (Character character in gameController.allCharacters)
        {
            if (character.IsEnemy())
            {
                character.StartTurn();
                character.HideIntentMarker();
            }
        }

        // Attack the duck that was chosen as the target during the player's turn
        AttackChosenTarget();
    }

    private void AttackChosenTarget()
    {
        Duck targetDuck = gameController.enemy.GetTarget(); // Get the enemy's chosen target

        gameController.PlayCharacterAnimation(CharacterType.Enemy, "attack");

        // If the target is still alive, attack
        if (targetDuck != null && targetDuck.currentHealth > 0)
        {
            gameController.enemy.Attack(targetDuck);
            Debug.Log($"Enemy attacked {targetDuck.characterName}.");
        }
        else
        {
            Debug.Log("The chosen target is dead or invalid. Enemy skips attack.");
        }

        // End the enemy's turn after the attack
        gameController.EndEnemyTurn();
    }

    public override void Exit()
    {        
        Debug.Log("Enemy's turn has ended.");
    }
}
