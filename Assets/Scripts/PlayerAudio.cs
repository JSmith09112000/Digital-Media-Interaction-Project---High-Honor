using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip collisionSound;
    public AudioClip damageSound;
    public AudioClip deathSound;
    public AudioClip healingSound;

    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float volume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.volume = volume;
    }

    // Plays when the player collides with something
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlaySound(collisionSound);
    }

    // Plays when the player takes damage
    public void PlayDamageSound()
    {
        PlaySound(damageSound);
    }

    // Plays when the player heals
    public void PlayHealingSound()
    {
        PlaySound(healingSound);
    }

    // Plays when the player dies
    public void PlayDeathSound()
    {
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(
                deathSound,
                transform.position,
                volume
            );
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}