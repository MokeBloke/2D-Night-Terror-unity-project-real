using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Restart : MonoBehaviour
{
    public Button restartButton;
    public PlayerHealth playerHealth;

    void Awake()
    {
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth != null)
            playerHealth.OnDeath += GameOver;
    }

    void Start()
    {
        if (restartButton != null)
            restartButton.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnDeath -= GameOver;
    }

    public void GameOver()
    {
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
