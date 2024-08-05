using UnityEngine;
using System.Collections;

public class SkeletonEnemy : MonoBehaviour
{
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float moveSpeed = 3f;
    public int attackDamage = 10;

    public GameObject deathEffect; // Particle system or visual effect
    public AudioClip deathSound;    // Sound effect for death

    private Transform player;
    private Animator animator;
    private bool isAttacking = false;
    private bool isInAttackRange = false;
    private float lastAttackTime;
    private int currentHealth;
    public int maxHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer <= attackRange)
            {
                isInAttackRange = true;
                if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
                {
                    StartCoroutine(PerformAttack());
                }
                else
                {
                    // Stop movement and play attack animation
                    if (!isAttacking)
                    {
                        animator.SetInteger("animState", 2); // Set to attack animation
                        StopMoving();
                    }
                }
            }
            else
            {
                isInAttackRange = false;
                ChasePlayer();
            }
        }
        else
        {
            // Idle if out of detection range
            animator.SetInteger("animState", 0);
            StopMoving();
        }
    }

    void ChasePlayer()
    {
        if (isInAttackRange) return;

        animator.SetInteger("animState", 1); // Set to walk animation
        MoveTowards(player.position, moveSpeed);
    }

    void StopMoving()
    {
        animator.SetInteger("animState", 0); // Set to idle animation
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        animator.SetInteger("animState", 2); // Set to attack animation

        // Wait for the attack animation to finish
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;
        float elapsedTime = 0f;

        while (elapsedTime < animationLength)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        lastAttackTime = Time.time;
        isAttacking = false;
    }

    public void ApplyDamage()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            FindObjectOfType<PlayerHealth>().TakeDamage(attackDamage);
        }
    }

    void MoveTowards(Vector3 targetPosition, float speed)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        transform.LookAt(targetPosition);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            StartCoroutine(Death());
        }
    }

  IEnumerator Death()
{
    // Disable the animator
    if (animator != null)
    {
        animator.enabled = false;
    }

    // Play the death sound if assigned
    if (deathSound != null)
    {
        AudioSource.PlayClipAtPoint(deathSound, transform.position);
    }

    // Instantiate the death effect at a higher position
    if (deathEffect != null)
    {
        // Instantiate the effect slightly higher
        Vector3 effectPosition = transform.position + new Vector3(0, 2f, 0); // Adjust the 1f value as needed
        GameObject effect = Instantiate(deathEffect, effectPosition, Quaternion.identity);

        // Optionally, you can set the parent of the effect to be null to ensure it doesn't move with the enemy
        effect.transform.parent = null;

        // Destroy the effect after its duration or a bit longer
        Destroy(effect, 3f);  // Adjust this duration if needed
    }

    // Wait for a short delay to ensure the effect has time to be visible
    yield return new WaitForSeconds(0.3f);  // Increased time to ensure visibility

    // Destroy the game object
    Destroy(gameObject);
}
}
