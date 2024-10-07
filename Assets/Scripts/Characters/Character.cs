using UnityEngine;
using TMPro; // Import the TextMeshPro namespace
using System.Collections;
using System.Collections.Generic;

public enum CharacterType
{
    Knight,
    Wizard,
    Rogue,
    Enemy
}

public class Character : MonoBehaviour
{
    public string characterName;
    public int maxHealth;
    public int currentHealth;
    public int attackDamage;
    public int block; // Amount of block (temporary defense)

    public float evadeChance = 0f; // Evade chance (0.0 to 1.0)
    private int evadeChanceTurnsRemaining;

    public float attackMoveDistance = 1.5f;  // How far the character moves forward during an attack
    public float attackMoveSpeed = 5.0f;     // Speed of the attack movement

    // Particle systems for block and health effects
    public ParticleSystem blockParticle;
    public ParticleSystem missParticle;
    public ParticleSystem healParticle;
    public ParticleSystem healthParticle;
    public ParticleSystem deathParticle;

    public TextMeshPro healthText; // Reference to the TextMeshPro component for displaying health
    public TextMeshPro blockText; // Reference to the TextMeshPro component for displaying health
    public GameObject blockMarker; // Reference to the block marker object (blockText)
    public GameObject healthMarker; // Reference to the block marker object (blockText)
    public GameObject marker; // Reference to the marker object
    public GameObject intentMarker; // Reference to the intent marker object
    private SpriteRenderer markerRenderer; // Reference to the marker's SpriteRenderer to change its color

    public Sprite deathSprite;

    public Color defaultMarkerColor = Color.white; // Default marker color
    public Color hoverMarkerColor = Color.yellow; // Marker color when hovered
    public CharacterType characterType; // Add this to define the class of the character

    public bool markerIsActive;

   public Transform homePosition;

    // Poison-related properties
    private int poisonDamage;
    private int poisonTurnsRemaining;

    private void Start()
    {
        // Initialize marker visibility and color
        marker.SetActive(false);
        markerIsActive = false;
        markerRenderer = marker.GetComponent<SpriteRenderer>();
        markerRenderer.color = defaultMarkerColor;

        // Update the health text at the start
        UpdateStatText();
    }

    // Initialize character stats and health text
    public virtual void InitializeCharacter(string name, int health, int attack, CharacterType type, Transform homeTransform)
    {
        characterName   = name;
        maxHealth       = health;
        currentHealth   = health;
        attackDamage    = attack;
        characterType   = type;
        homePosition    = homeTransform;

        HideIntentMarker();
        UpdateStatText();
    }

    public bool IsAlly()
    {
        return characterType == CharacterType.Rogue || characterType == CharacterType.Knight || characterType == CharacterType.Wizard;
    }

    public bool IsEnemy()
    {
        return characterType == CharacterType.Enemy;
    }

    // Method to gain block
    public void GainBlock(int amount)
    {
        SoundManager.instance.PlaySound("GainBlock", this.transform, true);

        block += amount;
        Debug.Log($"{characterName} gained {amount} block. Current block: {block}");

        UpdateStatText(); // Update UI after healing
    }

    // Method to reset block
    public void ResetBlock()
    {
        block = 0;
        UpdateStatText(); // Update UI after healing
    }

    // Method to heal the character
    public void Heal(int amount)
    {

        SoundManager.instance.PlaySound("Heal", this.transform, true);
        healParticle.Play();

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Ensure health doesn't exceed maxHealth

        Debug.Log($"{characterName} healed for {amount}. Current health: {currentHealth}");

        UpdateStatText(); // Update UI after healing
    }

    // Update this method to consider evade
    public virtual void TakeDamage(int damage)
    {
        bool blockWasReduced = false;
        bool healthWasReduced = false;

        // Check for evade
        if (TryEvade())
        {
            SoundManager.instance.PlaySound("NearMiss", this.transform, true);
            missParticle.Play();

            Debug.Log($"{characterName} evaded the attack!");
            return; // No damage taken
        }

        if (block > 0)
        {
            // If the character has block, reduce damage from block first
            int remainingDamage = Mathf.Max(damage - block, 0);
            block = Mathf.Max(block - damage, 0);
            damage = remainingDamage;

            blockWasReduced = true;
        }

        // Apply any remaining damage to health
        if (damage > 0)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth); // Ensure health doesn't go below 0
            healthWasReduced = true;
        }

        Debug.Log($"{characterName} took {damage} damage. Current health: {currentHealth}");

        // Play particle effects for block and health reduction
        if (blockWasReduced && blockParticle != null)
        {
            blockParticle.Play();
        }

        if (healthWasReduced && healthParticle != null)
        {
            healthParticle.Play();
        }

        PlayDamageAnimation();

        // Update the health text
        UpdateStatText();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Poison-related methods
    public void ApplyPoison(int damagePerTurn, int turns)
    {
        poisonDamage = damagePerTurn;
        poisonTurnsRemaining += turns;
        Debug.Log($"{characterName} has been poisoned! Takes {damagePerTurn} damage for {turns} turns.");
    }

    // Method to check poison damage at the start of each turn
    public void CheckPoisonDamage()
    {
        if (poisonTurnsRemaining > 0)
        {
            TakeDamage(poisonDamage);
            poisonTurnsRemaining--;
            Debug.Log($"{characterName} took {poisonDamage} poison damage. {poisonTurnsRemaining} turns remaining.");
        }
    }


    // Method to apply evade chance for a turn~
    public void ApplyEvadeChance(float chance, int turns)
    {
        evadeChance = chance;
        evadeChanceTurnsRemaining += turns;
        Debug.Log($"{characterName} gained {evadeChance * 100}% evade chance!");
    }

    public void CheckEvade()
    {
        if (evadeChanceTurnsRemaining > 0)
        {
            evadeChanceTurnsRemaining--;
        }
        else 
        {
            ResetEvadeChance();
        }
    }

    // Method to determine if an attack is evaded
    public bool TryEvade()
    {
        if (evadeChanceTurnsRemaining > 0)
        {
            return Random.value < evadeChance; // Returns true if evaded
        }
        else
        {
            return false;
        }
    }
    
    // Function to reset evade chance (e.g., after a turn)
    public void ResetEvadeChance()
    {
        evadeChance = 0f;
    }

    // Method to perform an attack
    public virtual void Attack(Character target)
    {
        target.TakeDamage(attackDamage);
        Debug.Log($"{characterName} attacked {target.characterName} for {attackDamage} damage.");
    }

    // Function to update the health and block text display
    public virtual void UpdateStatText()
    {
        if (currentHealth > 0)
        {
            healthText.text = currentHealth.ToString();
            healthMarker.SetActive(true); // Show the block marker
        }
        else
        {
            healthMarker.SetActive(false); // Hide the block marker when block is 0

        }

        // If block is greater than 0, display the block text and marker
        if (block > 0)
        {
            blockText.text = block.ToString();
            blockMarker.SetActive(true); // Show the block marker
        }
        else
        {
            blockMarker.SetActive(false); // Hide the block marker when block is 0
        }
    }

    // Function to show the marker above the character
    public void ShowMarker()
    {
        marker.SetActive(true); // Make marker visible
        marker.GetComponent<Marker>().SetEnabled(true);
        marker.GetComponent<Marker>().SetHover(false);
        markerIsActive = true;
        markerRenderer.color = defaultMarkerColor; // Reset to default color
    }

    // Function to hide the marker
    public void HideMarker()
    {
        marker.SetActive(false); // Hide marker
        markerIsActive = false;
    }

    // Function to change the marker's color when hovered over
    private void OnMouseEnter()
    {
        if (markerIsActive)
        {
            marker.GetComponent<Marker>().SetEnabled(false);
            marker.GetComponent<Marker>().SetHover(true);
            markerRenderer.color = hoverMarkerColor; // Change color on hover
            SoundManager.instance.PlaySound("ButtonHover", this.transform, true);
        }
    }

    private void OnMouseExit()
    {
        if (markerIsActive)
        {
            marker.GetComponent<Marker>().SetEnabled(true);
            marker.GetComponent<Marker>().SetHover(false);
            markerRenderer.color = defaultMarkerColor; // Revert color when hover ends
        }
    }

    // Handle clicking on the character to select them as a target
    private void OnMouseDown()
    {
        if (markerIsActive)
        {
            GameController.instance.OnTargetChosen(this.gameObject); // Notify GameController that this target was chosen
            SoundManager.instance.PlaySound("CharacterSelect", this.transform, true);
        }
    }

    // Call this at the start of each turn for all characters
    public void StartTurn()
    {
        // Apply poison damage at the start of the turn if the character is poisoned
        CheckPoisonDamage();
        CheckEvade();
        
        // Other turn-related logic...
    }

    // Function to show the intent marker
    public void ShowIntentMarker()
    {
        if (intentMarker != null)
        {
            intentMarker.SetActive(true); // Show the marker
        }
    }

    // Function to hide the intent marker
    public void HideIntentMarker()
    {
        if (intentMarker != null)
        {
            intentMarker.SetActive(false); // Hide the marker
        }
    }
    // Attack Animation Coroutine using the homePosition
    public void PlayAttackAnimation()
    {
        StartCoroutine(AttackAnimation());
    }

    public IEnumerator AttackAnimation()
    {
        Vector3 targetPosition;

        if (IsAlly())  // Ducks (move right)
        {
            targetPosition = new Vector3(homePosition.position.x + attackMoveDistance, homePosition.position.y, homePosition.position.z);
        }
        else if (IsEnemy())  // Enemies (move left)
        {
            targetPosition = new Vector3(homePosition.position.x - attackMoveDistance, homePosition.position.y, homePosition.position.z);
        }
        else
        {
            yield break;
        }

        // Move the character forward
        yield return StartCoroutine(MoveToPosition(targetPosition));

        // Add a slight delay
        yield return new WaitForSeconds(0.1f);

        // Move the character back to the home position
        yield return StartCoroutine(MoveToPosition(homePosition.position));
    }

    // Damage Animation Coroutine using the homePosition
    public void PlayDamageAnimation()
    {
        StartCoroutine(DamageAnimation());
    }

    public IEnumerator DamageAnimation()
    {
        
        SoundManager.instance.PlaySound("Hurt", this.transform, true);

        yield return new WaitForSeconds(0.1f);


        Vector3 targetPosition;

        if (IsAlly())  // Ducks (flinch left)
        {
            targetPosition = new Vector3(homePosition.position.x - attackMoveDistance, homePosition.position.y, homePosition.position.z);
        }
        else if (IsEnemy())  // Enemies (flinch right)
        {
            targetPosition = new Vector3(homePosition.position.x + attackMoveDistance, homePosition.position.y, homePosition.position.z);
        }
        else
        {
            yield break;
        }

        // Move the character backward (flinch)
        yield return StartCoroutine(MoveToPosition(targetPosition));

        // Add a slight delay
        // yield return new WaitForSeconds(0.1f);

        // Move the character back to the home position
        yield return StartCoroutine(MoveToPosition(homePosition.position));
    }

    // Coroutine to move the character to a target position smoothly with a timeout
    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        float timeout = 0.3f;  // Set the timeout duration
        float elapsedTime = 0f;  // Track the elapsed time

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f && elapsedTime < timeout)
        {
            // Move towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, attackMoveSpeed * Time.deltaTime);

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // After timeout or reaching the position, set the final position
        transform.position = targetPosition;
    }

    public void Die()
    {
        StartCoroutine(IE_Die());
    }

    // Function when character dies
    private IEnumerator IE_Die()
    {
        Debug.Log(characterName + " has died!");

        SoundManager.instance.PlaySound("RetroDeath", this.transform, true);

        // Step 1: Shake the character
        yield return StartCoroutine(ShakeCharacter(0.6f));  // Shake for 0.6 seconds

        if (characterType != CharacterType.Enemy)
        {
            yield return new WaitForSeconds(0.3f);
            SoundManager.instance.PlaySound("Quack1", this.transform, true);
            yield return new WaitForSeconds(0.6f);
        }

        SoundManager.instance.PlaySound("RetroExplosion", this.transform, true);

        yield return new WaitForSeconds(0.05f);



        // Step 2: Play death particle effect (if assigned)
        if (deathParticle != null)
        {
            deathParticle.Play();  // You can use healthParticle as the death particle effect
        }

        // Wait a moment after the death particles (optional)
        yield return new WaitForSeconds(0.1f);


        // Hide health and block markers
        if (healthMarker != null)
        {
            healthMarker.SetActive(false);
        }
        if (blockMarker != null)
        {
            blockMarker.SetActive(false);
        }

        // Show death sprite
        if (deathSprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = deathSprite;
        }

        yield return new WaitForSeconds(0.3f);

        // Notify the GameController to disable cards of this duck's class
        GameController.instance.OnCharacterDeath(characterType);
    }

    // Coroutine to shake the character for a specified duration
    private IEnumerator ShakeCharacter(float duration)
    {
        Vector3 originalPosition = homePosition.position;  // Store the original position
        float elapsed = 0f;

        // Continue shaking for the specified duration
        while (elapsed < duration)
        {
            float xOffset = Random.Range(-0.1f, 0.1f);  // Random horizontal movement
            float yOffset = Random.Range(-0.1f, 0.1f);  // Random vertical movement

            // Apply the random offset to the character's position
            transform.position = new Vector3(originalPosition.x + xOffset, originalPosition.y + yOffset, originalPosition.z);

            // Wait for the next frame
            yield return null;

            elapsed += Time.deltaTime;
        }

        // Reset the position back to the original at the end
        transform.position = homePosition.position;
    }

}
