using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;
    public float moveSpeed = 2f;
    public float attackSpeed = 1f;
    public float viewRange = 5f;
    public float attackRange = 1.5f;
    public float returnRange = 7f;
    public float stopDistance = 1f;
    public float stunDuration = 0f;
    public bool hitted = false;

    public Transform leftPatrolPoint;
    public Transform rightPatrolPoint;
    public LayerMask playerLayer;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 patrolTarget;
    private bool chasing = false;
    public bool attacking = false;
    private bool moveable = true;
    public bool isStunned = false;
    private bool canAttack = true;  // Cooldown control
    public float attackCooldownTime = 1.5f; // Adjust cooldown as neede

    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        patrolTarget = rightPatrolPoint.position;
    }

    void Update()
    {
        if (currentHP <= 0 || isStunned) return;
        if (!isStunned) 
        {
            if (chasing)
            {
                ChasePlayer();
            }
            else
            {
                Patrol();
            }
        }
        DetectPlayer();

    }

    void Patrol()
    {
        if (!moveable) return;

        animator.SetBool("isWalking", true);
        transform.position = Vector2.MoveTowards(transform.position, patrolTarget, moveSpeed * Time.deltaTime);

        // Flip the enemy based on direction
        if (transform.position.x > patrolTarget.x)
        {
            // Enemy moving left
            Flip(true);
        }
        else if (transform.position.x < patrolTarget.x)
        {
            // Enemy moving right
            Flip(false);
        }

        if (Vector2.Distance(transform.position, patrolTarget) < 0.2f)
        {
            patrolTarget = (patrolTarget == (Vector2)rightPatrolPoint.position) ? (Vector2)leftPatrolPoint.position : (Vector2)rightPatrolPoint.position;
            Flip(transform.position.x < patrolTarget.x); // Flip based on current position and target
        }
    }

    void DetectPlayer()
    {
        Collider2D playerCheck = Physics2D.OverlapCircle(transform.position, viewRange, playerLayer);
        if (playerCheck != null)
        {
            player = playerCheck.transform;
            chasing = true;
        }
    }
    void Flip(bool isMovingRight)
    {
        // Flip horizontally based on direction
        Vector3 scale = transform.localScale;
        if (isMovingRight && scale.x < 0 || !isMovingRight && scale.x > 0)
        {
            scale.x = -scale.x;  // Flip the enemy sprite
        }
        transform.localScale = scale;
    }
    void ChasePlayer()
    {
        if (player == null || isStunned) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > returnRange)
        {
            chasing = false;
            moveable = true;
            return;
        }

        if (distance <= attackRange && !attacking && canAttack)
        {
            if (player == null) return;
            StartCoroutine(Attack());
        }
        else if (!attacking && distance > stopDistance)
        {
            moveable = true;
            animator.SetBool("isWalking", true);

            // Move towards the player
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

            // Flip the enemy to face the player
            LookAtPlayer();
        }
        else
        {
            moveable = false;
            animator.SetBool("isWalking", false);
        }
    }

    IEnumerator Attack()
    {
        attacking = true;
        moveable = false;
        canAttack = false;  // Prevents immediate re-attacking

        LookAtPlayer();
        animator.SetTrigger("attack");
        animator.SetBool("isAttacking", true); // Play attack animation

        yield return new WaitForSeconds(attackSpeed); // Wait for attack animation to finish

        animator.SetBool("isAttacking", false); // Stop attack animation
        attacking = false;
        moveable = true;

        // Cooldown before next attack
        yield return new WaitForSeconds(attackCooldownTime);
        canAttack = true;  // Allow attacking again
    }

    public void TakeDamage(int damage)
    {
        if (currentHP <= 0) return;
        if (!hitted)
        {
            currentHP -= damage;
            animator.SetTrigger("hit");
        }


        if (currentHP <= 0)
        {
            animator.SetTrigger("dead");
            Die();
        }
    }

    void Die()
    {

        rb.velocity = Vector2.zero;
        this.enabled = false;
        Destroy(gameObject, 1f);
    }

    public void ResetAttackTimer()
    {
        StopAllCoroutines(); // Stops ongoing attack coroutine
        attacking = false;

    }

    public void Stun(float stunfromplayer)
    {
        stunDuration = stunfromplayer;
        StartCoroutine(StunTimer(stunfromplayer));
        if (isStunned) return; // Prevent multiple stuns from overlapping
        gameObject.layer = LayerMask.NameToLayer("Default");
        isStunned = true;
        moveable = false;
        animator.SetBool("isWalking", false);
        // Ensure coroutine starts
    }

    private IEnumerator StunTimer(float stunDuration)
    {

        yield return new WaitForSeconds(stunDuration); // Wait for stun duration
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        hitted = false;
        isStunned = false; // End stun
        moveable = true; // Allow movement again


    }

    private void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void LookAtPlayer()
    {
        if (player != null)
        {
            float direction = player.position.x - transform.position.x;

            // Flip only if the direction changes
            if (Mathf.Abs(direction) > 0.1f)
            {
                if (direction < 0 && transform.localScale.x < 0) // Player is to the right
                {
                    Flip();
                }
                else if (direction > 0 && transform.localScale.x > 0) // Player is to the left
                {
                    Flip();
                }
            }
        }
    }
}