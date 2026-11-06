using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    // Updated to utilize the stats tied to the Unit
    private Stats stats;

    [Header("Health UI")]
    public Slider healthSlider;

    private PlayerAudio playerAudio;
    private void Start()
    {
        stats = GetComponent<Stats>();
        maxHealth = stats.hp;
        currentHealth = maxHealth;

        healthSlider.maxValue = maxHealth;// connect slider max value to max health value
        healthSlider.value = currentHealth;// connect slider to current health
    }

    public void TakeDamage(int damage)
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

    public void Heal(int amount)
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

        Debug.Log("Enemy has died!");

        //add death logic here - death screen, end screen, etc.
    }
}
