using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Image gameOverImage;
    public Button restartButton;

    public int health;
    public int maxHealth = 5;

    public SpriteRenderer playerSr;
    public PlayerMovement playerMovement;

    // Event fired once when player dies
    public event Action OnDeath;

    bool isDead = false;

    void Start()
    {
        health = maxHealth;
        if (gameOverImage != null) gameOverImage.gameObject.SetActive(false);
        if (restartButton != null) restartButton.gameObject.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;
        if (health <= 0)
        {
            isDead = true;

            // Stop player input / movement so death animation can play
            if (playerMovement != null) playerMovement.enabled = false;

            // Notify listeners (Deathanim) so they can play animation and handle destruction/timing
            OnDeath?.Invoke();

            // Show game over UI (kept so UI still appears)
            GameOver();

            // Do NOT disable sprite or immediately destroy here — let Deathanim play the animation first.
        }
    }

    public void GameOver()
    {
        if (gameOverImage != null)
            gameOverImage.gameObject.SetActive(true);
        else
            Debug.LogWarning("gameOverImage not assigned on " + name);

        if (restartButton != null)
            restartButton.gameObject.SetActive(true);
        else
            Debug.LogWarning("restartButton not assigned on " + name);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
