using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Data/Weapon Definition")]
public class WeaponConfig : ScriptableObject
{
    // damage, fireRate, projectileSpeed, range
    public float damage = 10f;
    public float fireRate = 1f;
    public float projectileSpeed = 20f;
    public float range = 15f;
    public float maxLife = 2f;
}