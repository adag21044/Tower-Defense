using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyHealthController enemyHealthController;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private EnemyMover enemyMover;

    private void Update()
    {
        enemyMover.MoveAlongPath();
    }
}