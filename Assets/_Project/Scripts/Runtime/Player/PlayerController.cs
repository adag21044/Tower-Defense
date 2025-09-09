using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;

    private void Update()
    {
        playerMovement.HandleMovement();
    }
}