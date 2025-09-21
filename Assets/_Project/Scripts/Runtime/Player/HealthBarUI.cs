using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private float tweenDuration = 0.3f;

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        float targetFill = Mathf.Clamp01((float)currentHealth / maxHealth);

        // Kill previous tween before starting a new one
        healthFill.DOKill();
        healthFill.DOFillAmount(targetFill, tweenDuration).SetEase(Ease.OutCubic);
    }
}
