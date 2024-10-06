using UnityEngine;

// Enemy class inheriting from Character
public class Enemy : Character
{
    private Duck targetDuck; // The duck chosen as the target for the enemy's attack

    // Set the enemy's target duck
    public void SetTarget(Duck duck)
    {
        targetDuck = duck;
    }

    // Get the enemy's target duck
    public Duck GetTarget()
    {
        return targetDuck;
    }
}
