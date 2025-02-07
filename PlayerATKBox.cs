using UnityEngine;
using System.Collections;

public class PlayerAttackBox : MonoBehaviour
{
    public string hitAnimationParameter = "hit";
    public float stuntime = 1.5f;
    public int dmg = 1;
    private BoxCollider2D boxCollider;
    private float rightOffset = 3.5f;
    private float leftOffset = -3.5f;
    private bool canChangeDirection = true; // Controls if direction can change
    public float directionLockTime = 0.5f; // Time to prevent direction change after pressing D

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError("PlayerAttackBox is missing a BoxCollider2D component!");
        }
    }

    void Update()
    {
        HandleDirectionLock();

        if (canChangeDirection)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SetColliderOffset(leftOffset);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SetColliderOffset(rightOffset);
            }
        }
    }

    private void HandleDirectionLock()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(LockDirectionChange());
        }
    }

    private void SetColliderOffset(float xOffset)
    {
        if (boxCollider != null)
        {
            boxCollider.offset = new Vector2(xOffset, boxCollider.offset.y);
        }
    }

    private IEnumerator LockDirectionChange()
    {
        canChangeDirection = false;
        yield return new WaitForSeconds(directionLockTime);
        canChangeDirection = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            HandleEnemyHit(collision);
        }
    }

    private void HandleEnemyHit(Collider2D collision)
    {
        Animator enemyAnimator = collision.GetComponent<Animator>();
        if (enemyAnimator != null)
        {
            EnemyAI enemyAI = collision.GetComponent<EnemyAI>();
            EnemyShooter enemyShooter = collision.GetComponent<EnemyShooter>();
            EnemyBullet deflect = collision.GetComponent<EnemyBullet>();

            if (enemyAI != null)
            {
                enemyAI.TakeDamage(dmg);
                enemyAnimator.SetTrigger(hitAnimationParameter);
                enemyAI.hitted = true;
                enemyAI.ResetAttackTimer();
                enemyAI.Stun(stuntime);
            }

            if (enemyShooter != null && enemyShooter.hitted == false)
            {
                enemyShooter.TakeDamage(dmg);
                enemyAnimator.SetTrigger(hitAnimationParameter);
                enemyShooter.hitted = true;
                enemyShooter.Stun(stuntime);
            }
           if (deflect != null)
            {
                deflect.parry();
            }
        }
    }
}
