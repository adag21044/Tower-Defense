using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    public static int ActiveCount { get; private set; }

    private void OnEnable()
    {
        ActiveCount = Mathf.Max(0, ActiveCount + 1);
    }

    private void OnDisable()
    {
        ActiveCount = Mathf.Max(0, ActiveCount - 1);
    }

    public static void Reset()
    {
        ActiveCount = 0;
    }
}
