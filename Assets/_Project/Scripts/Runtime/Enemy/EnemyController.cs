using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyHealthController enemyHealthController;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private EnemyMover enemyMover;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // Cache components
        if (!enemyHealthController) enemyHealthController = GetComponent<EnemyHealthController>();
        if (!enemyMover) enemyMover = GetComponent<EnemyMover>();
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (enemyData != null)
        {
            Apply(enemyData);
        }
        else
        {
            Debug.LogError("EnemyData is not assigned in EnemyController.");
        }
    }

    private void Update()
    {
        if (enemyMover != null)
            enemyMover.MoveAlongPath();
    }

    public void SetData(EnemyData data)
    {
        enemyData = data;

        if (enemyMover) enemyMover.MoveSpeed = data.moveSpeed;
        if (enemyHealthController) enemyHealthController.SetMaxHealth(data.maxHealth);
        if (spriteRenderer) spriteRenderer.color = data.color;

        if (enemyMover && data.pathData != null && data.pathData.waypoints.Length > 0)
            enemyMover.SetWaypoints(data.pathData.waypoints);
    }

    private void Reset()
    {
        if (!enemyHealthController) enemyHealthController = GetComponent<EnemyHealthController>();
        if (!enemyMover) enemyMover = GetComponent<EnemyMover>();
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Called right after Spawn()
    public void Apply(EnemyData newData)
    {
        enemyData = newData;

        if (enemyMover)
        {
            enemyMover.MoveSpeed = enemyData.moveSpeed;

            if (enemyData.pathData != null && enemyData.pathData.waypoints.Length > 0)
                enemyMover.SetWaypoints(enemyData.pathData.waypoints);
        }

        if (enemyHealthController)
            enemyHealthController.SetMaxHealth(enemyData.maxHealth);

        if (spriteRenderer)
            spriteRenderer.color = enemyData.color;
    }
}
