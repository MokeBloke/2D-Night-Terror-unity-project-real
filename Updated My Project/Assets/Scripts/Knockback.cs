using UnityEngine;

public class Knockback : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object we hit is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerKnockback knockback = collision.gameObject.GetComponent<PlayerKnockback>();
            if (knockback != null)
            {
                // Apply the knockback, passing the hazard's transform for direction
                knockback.TriggerKnockback(transform);
            }
        }
    }
}