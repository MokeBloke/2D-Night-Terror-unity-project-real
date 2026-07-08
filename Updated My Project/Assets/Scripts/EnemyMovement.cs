using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    Transform player;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float stoppingDistance = 1f;

    [Header("Rotation")]
    public bool lookAtPlayer = true;
    public float rotationSpeed = 10f;

    [Header("Tracking")]
    [Range(0f, 1f)]
    public float trackingAccuracy = 0.8f; // 1 = very precise, 0 = maximally imprecise
    public float maxJitterAngle = 15f;    // degrees of possible aim jitter
    public float trackingUpdateInterval = 0.25f; // seconds between recalculating noisy direction

    Rigidbody rb;

    // internal
    Vector3 currentDirection;
    float trackingTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        rb = GetComponent<Rigidbody>();

        // initialize currentDirection
        if (player != null)
            currentDirection = player.position - transform.position;
        else
            currentDirection = transform.forward;
        trackingTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        Vector3 rawDirection = player.position - transform.position;
        float distance = rawDirection.magnitude;

        // Recalculate a "noisy" direction only at the configured interval to reduce precision  
        trackingTimer -= Time.deltaTime;
        if (trackingTimer <= 0f)
        {
            trackingTimer = Mathf.Max(0.01f, trackingUpdateInterval);

            // compute jitter amount based on desired accuracy
            float jitter = (1f - Mathf.Clamp01(trackingAccuracy)) * maxJitterAngle;

            // create a random small rotation around Y (and X if needed) and apply to raw direction
            Quaternion jitterRot = Quaternion.Euler(
                Random.Range(-jitter, jitter),
                Random.Range(-jitter, jitter),
                0f
            );
            Vector3 noisy = jitterRot * rawDirection;

            // blend between perfect direction and noisy direction using trackingAccuracy
            float blend = 1f - Mathf.Clamp01(trackingAccuracy);
            currentDirection = Vector3.Slerp(rawDirection, noisy, blend);
        }

        if (distance > stoppingDistance)
        {
            Vector3 move = currentDirection.normalized * moveSpeed * Time.deltaTime;
            if (rb != null)
                rb.MovePosition(transform.position + move);
            else
                transform.position += move;
        }

        if (lookAtPlayer && currentDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(currentDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }
}