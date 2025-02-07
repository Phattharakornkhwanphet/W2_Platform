using UnityEngine;

public class Heal : MonoBehaviour
{
    public int healAmount = 1;  

    void Start()
    {
    }

    void OnTriggerEnter2D (Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (player.currentHealth < player.maxHealth)
            {
                player.heal(healAmount);
                Destroy(gameObject);
            }
            else
            {
                // Optionally: Debug or notify that player can't heal (full health)
                Debug.Log("Player's health is already full.");
            }
        }
    }
}
