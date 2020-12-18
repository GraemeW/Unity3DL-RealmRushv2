using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerProperties : MonoBehaviour
{
    // Tunables
    [SerializeField] float baseHealth = 5f; 
    [SerializeField] TextMeshProUGUI gameOverText = null;
    [SerializeField] TextMeshProUGUI scoreText = null;
    [SerializeField] TextMeshProUGUI healthText = null;

    // State
    float currentHealth = 5f;
    float currentScore = 0f;
    bool isAlive = true;

    private void Start()
    {
        CalculatePlayerHealth();
        SetInitialScore();
    }

    private void CalculatePlayerHealth()
    {
        currentHealth = baseHealth;
        FactoryBase[] factories = FindObjectsOfType<FactoryBase>();
        foreach (FactoryBase factory in factories)
        {
            if (factory.GetFriendlyBase())
            {
                currentHealth += factory.GetHealthAdder();
            }
        }
        healthText.text = currentHealth.ToString();
    }

    private void SetInitialScore()
    {
        currentScore = 0f;
        scoreText.text = currentScore.ToString();
    }


    public void DecrementHealth()
    {
        if (!isAlive) { return; }
        currentHealth--;
        healthText.text = currentHealth.ToString();
        if (currentHealth <= 0)
        {
            gameOverText.gameObject.SetActive(true);
            isAlive = false;
        }
    }

    public void IncrementScore()
    {
        if (!isAlive) { return; }
        currentScore++;
        scoreText.text = currentScore.ToString();
    }

    public bool GetAliveStatus()
    {
        return isAlive;
    }
}
