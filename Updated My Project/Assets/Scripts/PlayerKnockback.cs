using System.Collections;
using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    [Header("Knockback Settings")]
    public float knockbackForceX = 10f;
    public float knockbackForceY = 6f;
    public float knockbackDuration = 0.2f;

    private Rigidbody2D rb;
    private MonoBehaviour movementScript;
    private bool isKnockedBack = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Replace 'PlayerMovement' with the exact name of your movement script
        movementScript = GetComponent<PlayerMovement>();
    }

    public void TriggerKnockback(Transform damageSource)
    {
        if (!isKnockedBack)
        {
            StartCoroutine(KnockbackRoutine(damageSource));
        }
    }

    private IEnumerator KnockbackRoutine(Transform damageSource)
    {
        isKnockedBack = true;

        // 1. Disable normal movement
        movementScript.enabled = false;

        // 2. Calculate direction (-1 if damage source is to the right, 1 if to the left)
        float direction = damageSource.position.x > transform.position.x ? -1f : 1f;

        // 3. Reset velocity before applying force so it's consistent
        rb.linearVelocity = new Vector2(0, 0);

        // 4. Apply knockback force
        Vector2 force = new Vector2(direction * knockbackForceX, knockbackForceY);
        rb.AddForce(force, ForceMode2D.Impulse);

        // 5. Wait for the knockback duration
        yield return new WaitForSeconds(knockbackDuration);

        // 6. Restore normal movement
        movementScript.enabled = true;
        isKnockedBack = false;
    }
}