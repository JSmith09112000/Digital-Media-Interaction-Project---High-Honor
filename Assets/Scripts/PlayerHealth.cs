using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Health UI")]
    public Slider healthSlider;

    private PlayerAudio playerAudio;
    private void Start()
    {
        currentHealth = maxHealth;

        healthSlider.maxValue = maxHealth;// connect slider max value to max health value
        healthSlider.value = currentHealth;// connect slider to current health
        playerAudio = GetComponent<PlayerAudio>(); 
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            playerAudio.PlayDamageSound();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        playerAudio.PlayHealingSound();
    }

    private void UpdateHealthUI()
    {
        healthSlider.value = currentHealth;
    }

    private void Die()
    {
        playerAudio.PlayDeathSound();

        Debug.Log("Player has died!");

        //add death logic here - death screen, end screen, etc.
    }
}