using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;

    public Animator camAnim;
    public Slider healthBar;
    private int currentHealth;
    private bool stageTwoTriggered;

    // New: store initial position and allow toggling the behaviour in the inspector
    public bool resetPositionOnAnimationChange = false;
    private Vector3 initialPosition;
    private int prevStateHash = -1;

    // --- NEW for Option B: scene object to reveal on death ---
    [Tooltip("Reference to the Enemy2 GameObject already in the scene. It will be hidden at Start and enabled when this enemy dies.")]
    public GameObject enemy2Object;
    public GameObject enemyObject;

    [Tooltip("If true, Enemy2 will be moved to this enemy's position when enabled.")]
    public bool moveEnemy2ToThisPosition = true;

    // New: triangle object to unhide when this enemy dies
    [Tooltip("Reference to the Triangle GameObject to show when this enemy dies.")]
    public GameObject triangleObject;
    public GameObject enemyDead;



    // New: third object that will be hidden when THIS enemy dies (assign on Enemy2 instance to hide a third object for Enemy 2)
    [Tooltip("Optional: Reference to a GameObject that will be deactivated when this enemy's HP reaches 0.")]
    public GameObject hideOnDeathObject;
    public GameObject door;

    // Fixed: declare the Animator reference actually used by the script
    private Animator anim;

    // Track death so we only run death logic once
    private bool isDead = false;

    // Damage flash settings
    [Header("Damage Flash")]
    public Color damageColor = Color.red;
    public float flashDuration = 0.08f; // single step duration
    public int flashCount = 3; // how many red/normal cycles
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();

        // Capture the spawn/initial position
        initialPosition = transform.position;

        if (anim != null)
            prevStateHash = anim.GetCurrentAnimatorStateInfo(0).shortNameHash;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        // Ensure Enemy2 is hidden until this enemy dies
        if (enemy2Object != null)
            enemy2Object.SetActive(false);

        // Ensure triangle is hidden until death (optional: remove if triangle should start visible)
        if (triangleObject != null)
            triangleObject.SetActive(false);

        // Note: hideOnDeathObject is intentionally not modified here; it should remain in its initial state
        // and will be deactivated when this enemy dies.

        // Find SpriteRenderer on this object or its children
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (anim == null) return;

        // Only reset position when explicitly enabled in the inspector
        if (!resetPositionOnAnimationChange) return;

        // Reset position and timers when the animator's state changes (optional)
        var state = anim.GetCurrentAnimatorStateInfo(0);
        int hash = state.shortNameHash;
        if (hash != prevStateHash)
        {
            ResetPosition();
            prevStateHash = hash;

            // ensure animator is running if it was paused previously
            if (anim.speed == 0f) anim.speed = 1f;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        if (healthBar != null)
            healthBar.value = currentHealth;

        // Play stage two transition if needed
        if (!stageTwoTriggered && currentHealth <= 25)
        {
            if (anim != null)
            {
                anim.SetTrigger("stageTwo");
                if (enemy2Object != null) enemy2Object.SetActive(true);
                if (enemyObject != null) enemyObject.SetActive(false);
            }

            stageTwoTriggered = true;
        }

        // Death: when health reaches 0, destroy enemy2 and unhide triangle
        if (currentHealth == 0 && !isDead)
        {
            isDead = true;

            if (enemy2Object != null)
            {
                Destroy(enemy2Object);
                enemy2Object = null;
                Destroy(door);
                door = null;
            }
            if (gameObject)

            if (triangleObject != null)
                triangleObject.SetActive(true);
            if (enemyDead != null)
                enemyDead.SetActive(true);

            // New: hide the optional third object on death (useful when this script instance is Enemy2)
            if (hideOnDeathObject != null)
                hideOnDeathObject.SetActive(false);

            // Optional: disable this enemy's visuals/logic
            // gameObject.SetActive(false);
        }

        // Start flashing red
        if (spriteRenderer != null)
        {
            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashDamage());
        }
    }

    private IEnumerator FlashDamage()
    {
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
        flashCoroutine = null;
    }

    private void ResetPosition()
    {
        transform.position = initialPosition;
        // If you also want to reset rotation or velocity (Rigidbody), handle that here:
        // transform.rotation = Quaternion.identity;
        // var rb = GetComponent<Rigidbody2D>();
        // if (rb != null) rb.velocity = Vector2.zero;
    }
}
