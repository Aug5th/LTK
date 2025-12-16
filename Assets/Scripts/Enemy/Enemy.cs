using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Health))]
public class Enemy : MonoBehaviour
{
    public int attackDamage = 5;
    Health health;

    void Awake()
    {
        health = GetComponent<Health>();
    }

    void Start()
    {
        health.SetMaxHealthFromStats(100); // example value , will be replaced by EnemyStats
    }
}