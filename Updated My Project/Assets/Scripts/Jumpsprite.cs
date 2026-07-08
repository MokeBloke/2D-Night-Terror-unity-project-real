using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Jumpsprite : MonoBehaviour
{
    [Header("Ground Check")]
    public bool use2D = true;                 // set false for 3D physics
    public Transform groundCheck;             // empty child at feet
    public float checkRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Animator")]
    public Animator animator;
    public string airborneBoolName = "IsAirborne"; // Animator Bool parameter name

    bool isGrounded;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (groundCheck == null) return;

        // check grounded
        if (use2D)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer) != null;
        else
            isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, groundLayer);

        // set Animator bool: true when airborne (not grounded)
        if (animator != null && !string.IsNullOrEmpty(airborneBoolName))
            animator.SetBool(airborneBoolName, !isGrounded);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}
