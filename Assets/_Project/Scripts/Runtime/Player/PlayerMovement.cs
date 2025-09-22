using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private float lookSpead = 2f;

    private float rotationX = 0f;
    private float rotationY = 0f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ParticleSystem dustEffect;

    private void Awake()
    {
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Y ekseninde hareketi kitliyoruz
        rb.constraints |= RigidbodyConstraints.FreezePositionY;
    }

    public void HandleMovement()
    {
        // Player movement
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        transform.Translate(moveX, 0, moveZ);

        transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0f);

        if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f)
        {
            if (!dustEffect.isPlaying)
                dustEffect.Play();
        }
        else
        {
            if (dustEffect.isPlaying)
                dustEffect.Stop();
        }
    }

    public void ResetPosition()
    {
        ToastManager.Instance.ShowToast("You have respawned!");
        Debug.Log("Resetting Player Position");
        
        transform.position = new Vector3(0f, -2f, -4f);
        rotationX = 0f;
        rotationY = 0f;
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }
}
