using UnityEngine;

public class Deathanim : MonoBehaviour
{
    [Tooltip("Reference to the PlayerHealth component (optional, will auto-find on same GameObject).")]
    public PlayerHealth playerHealth;

    [Tooltip("Animator that contains the death animation. If null, will try GetComponent<Animator>().")]
    public Animator animator;

    [Tooltip("Trigger parameter name in the Animator that starts the death animation.")]
    public string deathTrigger = "Die";

    [Tooltip("Fallback delay before destroying the player GameObject if animation length cannot be determined (seconds).")]
    public float destroyDelay = 1.0f;

    bool hasPlayed = false;

    void Awake()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (playerHealth != null)
            playerHealth.OnDeath += HandleDeath;
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnDeath -= HandleDeath;
    }

    void HandleDeath()
    {
        if (hasPlayed) return;
        hasPlayed = true;

        // Trigger animator if available
        if (animator != null)
        {
            // Try to determine a sensible destroy delay from animation clips named with "death" or "die"
            if (destroyDelay <= 0f)
                destroyDelay = 10f;

            var clips = animator.runtimeAnimatorController != null
                ? animator.runtimeAnimatorController.animationClips
                : null;

            if (clips != null && clips.Length > 0)
            {
                foreach (var clip in clips)
                {
                    var name = clip.name.ToLower();
                    if (name.Contains("death") || name.Contains("die"))
                    {
                        destroyDelay = clip.length;
                        break;
                    }
                }
            }

            animator.SetTrigger(deathTrigger);
        }
        else
        {
            Debug.LogWarning("Deathanim: Animator not assigned or found on " + gameObject.name);
        }

        // Ensure the player GameObject is destroyed after the animation delay
        Destroy(playerHealth != null ? playerHealth.gameObject : gameObject, destroyDelay);
    }
}