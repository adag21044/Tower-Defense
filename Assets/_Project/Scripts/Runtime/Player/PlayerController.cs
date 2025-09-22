using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerMovement playerMovement;
    public PlayerHealthController healthController;
    [SerializeField] private IFrameController iFrameController;


    private void Update()
    {
        playerMovement.HandleMovement();

        if (Input.GetKeyDown(KeyCode.K))
        {
            healthController.KillPlayer();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with Enemy");
            ToastManager.Instance.ShowToast("Collided with Enemy! Took 10 damage.");
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
        ToastManager.Instance.ShowToast("You have died! Game Over.");
        GameManager.Instance.RetryPanel(); // paneli aç
        // Respawn() çağrısını kaldır
    }

    private void HandleHealthChanged(int currentHealth)
    {
        ToastManager.Instance.ShowToast($"Player Health: {currentHealth}");
        Debug.Log("Health is now: " + currentHealth);
    }

    private void Respawn()
    {
        ToastManager.Instance.ShowToast("You have respawned!");
        Debug.Log("Player has respawned!");
        // Reset health, position etc.
        healthController.ResetHealth();
        playerMovement.ResetPosition();
    }
}