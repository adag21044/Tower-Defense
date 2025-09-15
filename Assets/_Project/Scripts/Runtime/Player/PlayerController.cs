using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerHealthController healthController;
        [SerializeField] private IFrameController iFrameController;


    private void Update()
    {
        playerMovement.HandleMovement();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with Enemy");
            healthController.TakeDamage(10);
            iFrameController.EnableIFrames();
        }
    }

    private void OnEnable()
    {
        healthController.OnDeath += HandlePlayerDeath;
        healthController.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        healthController.OnDeath -= HandlePlayerDeath;
        healthController.OnHealthChanged -= HandleHealthChanged;
    }

    private void HandlePlayerDeath()
    {
        Debug.Log("Player has died!");
        // Disable movement, play anim etc.

        Respawn();
    }

    private void HandleHealthChanged(int currentHealth)
    {
        Debug.Log("Health is now: " + currentHealth);
    }

    private void Respawn()
    {
        Debug.Log("Player has respawned!");
        // Reset health, position etc.
        healthController.ResetHealth();
        playerMovement.ResetPosition();
    }
}