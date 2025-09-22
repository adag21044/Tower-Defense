using System.Collections;
using UnityEngine;
using TMPro;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance { get; private set; }
    [SerializeField] private GameObject toastPrefab;
    [SerializeField] private Transform parentCanvas; // Canvas ref
    [SerializeField] private float duration = 2f;    // duration of toast

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowToast(string message)
    {
        GameObject toast = Instantiate(toastPrefab, parentCanvas);
        TMP_Text text = toast.GetComponentInChildren<TMP_Text>();
        text.text = message;

        StartCoroutine(DestroyAfterDelay(toast));
    }

    private IEnumerator DestroyAfterDelay(GameObject toast)
    {
        yield return new WaitForSeconds(duration);
        Destroy(toast);
    }
}
