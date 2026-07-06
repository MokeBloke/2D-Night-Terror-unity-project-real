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

    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        if (distance > stoppingDistance)
        {
            Vector3 move = direction.normalized * moveSpeed * Time.deltaTime;
            if (rb != null)
                rb.MovePosition(transform.position + move);
            else
                transform.position += move;
        }

        if (lookAtPlayer && direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }
}