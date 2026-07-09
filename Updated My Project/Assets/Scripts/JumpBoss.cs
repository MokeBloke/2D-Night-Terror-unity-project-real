using System.Collections;
using UnityEngine;

public class JumpBoss : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    private int moveDirection = 1; // 1 = Right, -1 = Left

    [Header("Jump Settings")]
    public float minJumpForce = 5f;
    public float maxJumpForce = 10f;
    public float minTimeBetweenJumps = 1.5f;
    public float maxTimeBetweenJumps = 4f;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Start the infinite loop for randomized jumping
        StartCoroutine(JumpRoutine());
    }

    void Update()
    {
        // Handle constant patrol movement
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);

        // Perform ground check using an overlap circle
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    IEnumerator JumpRoutine()
    {
        while (true)
        {
            // Wait for a randomized interval before trying to jump again
            float randomWaitTime = Random.Range(minTimeBetweenJumps, maxTimeBetweenJumps);
            yield return new WaitForSeconds(randomWaitTime);

            // Only jump if the enemy is safely touching the ground
            if (isGrounded)
            {
                Jump();
            }
        }
    }

    void Jump()
    {
        // Randomize how high/hard the enemy jumps
        float randomForce = Random.Range(minJumpForce, maxJumpForce);

        // Apply immediate impulse force upward
        rb.AddForce(Vector2.up * randomForce, ForceMode2D.Impulse);

        // Optional: Randomly change horizontal direction upon jumping
        if (Random.value > 0.5f)
        {
            FlipDirection();
        }
    }

    void FlipDirection()
    {
        moveDirection *= -1;

        // Flip the sprite visually
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    // Optional visualization for the ground check radius in the Editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}