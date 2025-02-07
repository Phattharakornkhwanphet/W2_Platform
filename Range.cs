using UnityEngine;
using System.Collections;

public class EnemyShooter : MonoBehaviour
{
    [Header("Stats")]
    public int HP = 2;
    public int currentHP;
    public float moveSpeed = 3f;
    public float shootCooldown = 2f;

    [Header("Zones")]
    public float tooCloseZone = 4f;
    public float shootZoneMin = 4f;
    public float shootZoneMax = 5f;
    public float chaseZoneMin = 6f;
    public float chaseZoneMax = 9f;
    

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private bool canShoot = true;
    private bool isDead = false;
    private bool isFacingRight = true;
    private bool isRunningAway = false;
    private bool isStunned = false;
    public bool hitted = false;

    private void Start()
    {
        currentHP = HP;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the Player has the 'Player' tag.");
        }
    }

    private void Update()
    {
        if (isDead || isStunned) return;
         LookAtPlayer();
        HandleZones();
    }

    private void HandleZones()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < tooCloseZone)
        {
            RunAwayFromPlayer();  // Too close → Run away
        }
        else if (distanceToPlayer >= shootZoneMin && distanceToPlayer <= shootZoneMax)
        {
            StopMoving();
            LookAtPlayer(); // Always look at the player in the shoot zone
            if (canShoot) StartCoroutine(ShootRoutine()); // Shoot if in range
        }
        else if (distanceToPlayer >= chaseZoneMin && distanceToPlayer <= chaseZoneMax)
        {
            MoveTowardsPlayer(); // Chase player
        }
        else
        {
            StopMoving(); // Idle if too far
        }
    }


    private void MoveTowardsPlayer()
    {
        animator.SetBool("isWalking", true);

        Vector2 direction = (player.position - transform.position).normalized;

        // Adjust movement direction based on facing direction
        if (!isFacingRight)
        {
            direction.x = Mathf.Abs(direction.x); // Always move left when facing left
        }
        else
        {
            direction.x = -Mathf.Abs(direction.x); // Always move right when facing right
        }

        rb.velocity = direction * moveSpeed;

    }

    private void StopMoving()
    {
        animator.SetBool("isWalking", false);
        rb.velocity = Vector2.zero;
    }



    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }


    private void RunAwayFromPlayer()
    {
        isRunningAway = true;
        animator.SetBool("isWalking", true);

        Vector2 directionAwayFromPlayer = (transform.position - player.position).normalized;
        rb.velocity = directionAwayFromPlayer * moveSpeed * 1.5f; // Run away faster
    }

    private IEnumerator ShootRoutine()
    {
        canShoot = false;
        animator.SetTrigger("Shoot");

        Vector2 direction = (player.position - bulletSpawnPoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate bullet to face player (since default bullet faces -x, add 180 degrees)
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.Euler(0, 0, angle + 180));

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.velocity = direction * 7f; // Adjust speed as needed
        }

        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }
    private void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Dead");
        Destroy(gameObject, 1f);
    }
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;
        animator.SetTrigger("Hit");

        if (currentHP <= 0)
        {
            Die();
        }
    }
    private void LookAtPlayer()
    {
        if (player == null) return;

        float enemyX = transform.position.x;
        float playerX = player.position.x;

        if (enemyX >= -11f && enemyX <= -3.1f) // Enemy is in range on -X side
        {
            if (spriteRenderer.flipX) // Only update if necessary
            {
                spriteRenderer.flipX = false;
                isFacingRight = true;
            }
        }
        else if (enemyX >= 3.1f && enemyX <= 11f) // Enemy is in range on +X side
        {
            if (playerX > enemyX && isFacingRight) // Player is on the left, enemy faces right
            {
                spriteRenderer.flipX = true;
                isFacingRight = false;
            }
            else if (playerX < enemyX && !isFacingRight) // Player is on the right, enemy faces left
            {
                spriteRenderer.flipX = false;
                isFacingRight = true;
            }
        }
    }
    public void Stun(float duration)
    {
        if (isStunned) return;

        isStunned = true;
        StopMoving();
        rb.velocity = Vector2.zero;
        StartCoroutine(StunTimer(duration));
    }

    private IEnumerator StunTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        isStunned = false;
        hitted = false;
        Debug.Log("Enemy stun ended. Resuming movement.");
    }
}
