using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour
{
    public FadeInImage fall;
    public Image[] hpIcons;   // Assign HP images in the inspector
    public Sprite normalHP;   // Sprite for full HP
    public Sprite lostHP;     // Sprite for lost HP
    public int maxHealth = 6;
    public int currentHealth;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float secondJumpForce = 8f;
    
    public float runspeed = 7f;
    public float walkspeed = 2f;
    private bool isGrounded;
    private bool isJumping;
    private bool isFalling;
    private bool canDoubleJump;
    private bool isAttacking;
    private bool isDead;
    private bool isHit; // Flag to check if the player is currently hit
    private float knockbackDuration = 0.5f; // Duration of knockback effect
    private Vector2 knockbackDirection;
    public float fallSpeed = 2f;  // Initial fall speed
    private bool canMove = true;   // Flag to control movement

    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D attackCollider;
    private Vector2 moveDirection;
    private SpriteRenderer spriteRenderer;
    private void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        attackCollider = GetComponentInChildren<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        attackCollider.enabled = false; // Disable the collider initially
    }

    private void Update()
    {
        Fade();
        UpdateHPUI();
        if (isDead || isHit || isAttacking) return; // Don't allow player control if dead or hit
        HandleGround();
        // Handle movement input
        HandleMovement();
        // Handle jumping
        HandleJumping();
        // Handle attacking
        HandleAttacking();

       

        // Update animation states based on movement
        UpdateAnimations();
    }
    public void ChangeHP(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure HP stays within bounds
        UpdateHPUI();
    }
    private void UpdateHPUI()
    {
        for (int i = 0; i < maxHealth; i++)
        {
            if (i < currentHealth)
                hpIcons[i].sprite = normalHP; 
            else
                hpIcons[i].sprite = lostHP;    
        }
    }
    private void HandleMovement()
    {
        if (isHit || !canMove ) return; 

        float moveX = Input.GetAxis("Horizontal");

        // Check if running (holding shift)
        if (Input.GetKey(KeyCode.LeftShift) && !isFalling)
        {
            moveSpeed = runspeed; // Running speed
            animator.SetBool("IsRunning", true);
        }
        else if (!isFalling)
        {
            moveSpeed = isFalling ? Mathf.Lerp(moveSpeed, 1f, Time.deltaTime * 2f) : walkspeed; // Slowly decrease speed to 1 while falling
            animator.SetBool("IsRunning", false);
        }
        if (isFalling)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, 1f, Time.deltaTime * 2f);
        }

        moveDirection = new Vector2(moveX * moveSpeed, rb.velocity.y);
        rb.velocity = moveDirection;

        if (moveX > 0)
        {
            spriteRenderer.flipX = false; // Face right
        }
        else if (moveX < 0)
        {
            spriteRenderer.flipX = true; // Face left
        }
    }

    private void HandleGround()
    {
        if (isGrounded)
        {
            animator.SetBool("IsGround", true);
            isFalling = false; // Player is not falling if on the ground
            canMove = true; // Enable movement when touching the ground
        }
        else
        {
            animator.SetBool("IsGround", false);
            if (!isFalling) // Start decreasing speed when player begins to fall
            {
                isFalling = true;
                canMove = false; // Disable movement while falling
                StartCoroutine(GraduallyDecreaseSpeed()); // Start coroutine to decrease speed
            }
        }
    }

    private IEnumerator GraduallyDecreaseSpeed()
    {
        while (rb.velocity.y < 0) // While falling
        {
            // Gradually reduce fall speed until it reaches 1
            moveSpeed = Mathf.Lerp(moveSpeed, 1f, Time.deltaTime * 2f);
            yield return null;
        }
        canMove = true; // Enable movement when player lands on the ground
    }

    private void HandleJumping()
    {
        if (Input.GetButtonDown("Jump") && (isGrounded || canDoubleJump))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                isGrounded = false;
                canDoubleJump = true;
                animator.SetTrigger("Jump");
            }
            else if (canDoubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, secondJumpForce);
                canDoubleJump = false;
                animator.SetTrigger("Jump");
            }
        }
    }

    private void HandleAttacking()
    {
        if (Input.GetKeyDown(KeyCode.D) && !isAttacking && isGrounded)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true; // Set attack flag to true
        animator.SetTrigger("Attack");
        attackCollider.enabled = true; // Enable the attack collider
        yield return new WaitForSeconds(0.3f); // Adjust based on attack animation length
        attackCollider.enabled = false; // Disable the attack collider
        yield return new WaitForSeconds(0.5f); // Delay before player can do other actions
        isAttacking = false; // Reset attack flag
    }

    private void UpdateAnimations()
    {
        if (isGrounded)
        {
            if (Mathf.Abs(rb.velocity.x) > 0)
            {
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsIdle", false);
            }
            else
            {
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsIdle", true);
            }
        }

        if (!isGrounded && rb.velocity.y < 0)
        {
            animator.SetTrigger("Fall");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            canDoubleJump = false; // Reset double jump on ground collision
        }
        else
        {
            isGrounded = false;
        }
    }
    public void Fade()
    {
        if (currentHealth <= 0)
        {
            fall.fadeInTriggered = true;
        }
    }
    // Take damage and apply knockback
    public void TakeDamage(int damage)
    {
        if (isDead || isHit) return;
        
        currentHealth -= damage;
        if (isHit)
        {
            moveSpeed = 0;
        }
        if (currentHealth <= 0)
        {
            Die();
            Fade();
        }
        else
        {
            // Play the hit animation and apply knockback
            animator.SetTrigger("Hit");
            isHit = true;
           
            StartCoroutine(KnockbackCoroutine());
        }
    }
   
    private IEnumerator KnockbackCoroutine()
    {
        float timer = 0f;

        // Apply knockback force
        while (timer < knockbackDuration)
        {
            rb.velocity = knockbackDirection * 5f; // Adjust knockback strength
            timer += Time.deltaTime;
            yield return null;
        }

        // Stop knockback after the duration
        rb.velocity = Vector2.zero;
        isHit = false;
        animator.SetTrigger("Idle"); // Return to idle or any appropriate state after hit
    }


    public void Die()
    {
        isDead = true;
        animator.SetTrigger("Dead");
        Destroy(gameObject, 2.5f);
        


        // Add logic to disable player input, play death animation, etc.
    }
    public void heal(int totalheal)
    {
        currentHealth += totalheal;
    }
    public void SetHP(int fall)
    {
        currentHealth = fall;
    }
}
