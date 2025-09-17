using UnityEngine;

public class IFrameController : MonoBehaviour
{
    [SerializeField] private float iFrameDuration = 2f;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;

    public void EnableIFrames()
    {
        StartCoroutine(IFrameRoutine());
    }

    private System.Collections.IEnumerator IFrameRoutine()
    {
        DisableCollider();
        yield return new WaitForSeconds(iFrameDuration);
        EnableCollider();
    }

    private void DisableCollider()
    {
        if (playerCollider != null)
            playerCollider.enabled = false;

        Debug.Log("Collider disabled for I-Frames.");
        ToastManager.Instance.ShowToast("You are invincible for a short time!");

        SetAlpha(0.5f, playerSpriteRenderer); // Yarı saydam yap
        
    }

    private void EnableCollider()
    {
        if (playerCollider != null)
            playerCollider.enabled = true;
        
        Debug.Log("Collider enabled after I-Frames.");
        ToastManager.Instance.ShowToast("You are vulnerable again!");
        SetAlpha(1f, playerSpriteRenderer); // Tamamen görünür yap
    }
    
    public void SetAlpha(float alpha, SpriteRenderer playerSpriteRenderer)
    {
        if (playerSpriteRenderer != null)
        {
            Color c = playerSpriteRenderer.color;
            c.a = Mathf.Clamp01(alpha); // 0 = tamamen görünmez, 1 = tamamen görünür
            playerSpriteRenderer.color = c;
        }
    }
}
