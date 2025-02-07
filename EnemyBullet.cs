using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 1;
    public float lifetime = 5f;

    private Vector2 moveDirection;

    public void SetDirection(Vector2 targetPosition)
    {
        // Calculate direction to the player
        moveDirection = (targetPosition - (Vector2)transform.position).normalized;

        // Rotate bullet so its -X faces the player
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move forward in its local -X direction
        transform.position += transform.right * -speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall") || collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
    public void parry()
    {
        Destroy(gameObject) ;
    }
}
