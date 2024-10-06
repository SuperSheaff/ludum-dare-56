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
            }
        }

        // Enemy attacks a random duck here
        AttackRandomDuck();
    }

    private void AttackRandomDuck()
    {
        // Choose a random duck
        Duck[] ducks = { gameController.rogueDuck, gameController.knightDuck, gameController.wizardDuck };
        Duck randomDuck = ducks[Random.Range(0, ducks.Length)];

        // Enemy attacks the chosen duck if the duck is alive
        if (randomDuck.currentHealth > 0)
        {
            gameController.enemy.Attack(randomDuck); // Attack the random duck
            Debug.Log($"Enemy attacked {randomDuck.characterName}!");
        }

        // End the enemy's turn after the attack
        gameController.EndEnemyTurn();
    }

    public override void Exit()
    {        
        Debug.Log("Enemy's turn has ended.");
    }
}
