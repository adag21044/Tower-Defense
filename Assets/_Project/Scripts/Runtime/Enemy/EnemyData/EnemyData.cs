using UnityEngine;

[CreateAssetMenu(fileName= "EnemyData", menuName = "Data/Enemy Definition")]
public class EnemyData : ScriptableObject
{
    public float moveSpeed = 2f;
    public Color color = Color.white;
    public int maxHealth = 100;
    public int damage = 10;
}