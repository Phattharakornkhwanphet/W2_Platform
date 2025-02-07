using UnityEngine;
using System.Collections;
public class EnemyAttack : MonoBehaviour
{
    public int attackDamage = 20; // Damage dealt by the enemy's attack
    public float attackCooldown = 1f; // Time between attacks
    private bool canAttack = true; // Flag to check if the enemy can attack

    // The area of the enemy's attack zone (can be a child collider or something else)
    public Collider2D attackZone;

    private void Start()
    {
        // Make sure the attack zone is not active initially
        if (attackZone != null)
        {
            attackZone.enabled = false;
        }
    }

    private void Update()
    {
        // Check if the enemy can attack and if it's time to perform an attack
        if (canAttack)
        {
            // You can trigger the attack using any condition. For simplicity, let's use a timer or animation event.
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        canAttack = false;

        // Enable attack zone collider to detect player collision
        if (attackZone != null)
        {
            attackZone.enabled = true;
        }

        // Wait for attack cooldown (time before next attack)
        yield return new WaitForSeconds(attackCooldown);

        // Disable attack zone collider after attack
        if (attackZone != null)
        {
            attackZone.enabled = false;
        }

        canAttack = true;
    }

    // This method is called when an object enters the attack zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered the attack zone is the player
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>(); // Ensure the player has a PlayerHitbox script
            if (player != null)
            {
               
                player.TakeDamage(attackDamage); // Call the player's TakeDamage method
                Debug.Log("Player hit by enemy!");
            }
        }
    }
}
