using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private HealthController healthController;

    // Events
    public static event Action OnPlayerDeath;

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
        }
    }
}