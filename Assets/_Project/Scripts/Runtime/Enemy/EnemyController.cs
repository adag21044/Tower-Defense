using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyHealthController enemyHealthController;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private EnemyMover enemyMover;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (enemyData != null)
        {
            enemyHealthController = GetComponent<EnemyHealthController>();
            enemyMover = GetComponent<EnemyMover>();

            enemyHealthController = GetComponent<EnemyHealthController>();
            enemyMover = GetComponent<EnemyMover>();

            if (enemyHealthController != null)
            {
                enemyHealthController = GetComponent<EnemyHealthController>();
            }

            if (enemyMover != null)
            {
                enemyMover = GetComponent<EnemyMover>();
            }

            // Apply data to components
            if (enemyHealthController != null)
            {
                enemyHealthController = GetComponent<EnemyHealthController>();
            }

            if (enemyMover != null)
            {
                enemyMover = GetComponent<EnemyMover>();
                enemyMover.MoveSpeed = enemyData.moveSpeed;
            }

            if (spriteRenderer != null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.color = enemyData.color;
            }
        }
        else
        {
            Debug.LogError("EnemyData is not assigned in EnemyController.");
        }
    }

    private void Update()
    {
        enemyMover.MoveAlongPath();
    }
}