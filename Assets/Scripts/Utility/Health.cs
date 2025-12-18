using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable
{
    [Header("Health (can be overridden by Stats)")]
    [Tooltip("Default max health if no stats component overrides it.")]
    public int maxHealth = 50;
    public int currentHealth = 0;

    public bool IsDead { get; private set; } = false;
    public Transform Transform => this.transform;

    /// <summary>0..1 health percent changed</summary>
    public event Action<float> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnDamageTaken;

    private UnitStats unitStats;

    void OnEnable()
    {
        if(unitStats != null)
        {
            unitStats.OnStatsChanged += UpdateHealthFromStats;
            UpdateHealthFromStats();
        }
    }

    private void UpdateHealthFromStats()
    {
        if(unitStats == null)   
        {
            return;
        }
        int newMaxHP = Mathf.Max(1, Mathf.RoundToInt(unitStats.GetStat(StatType.MaxHP)));
        SetMaxHealthFromStats(newMaxHP);
    }

    void OnDisable()
    {
        if(unitStats != null)
        {
            unitStats.OnStatsChanged -= UpdateHealthFromStats;
        }
    }

    void Awake()
    {
        unitStats = GetComponent<UnitStats>();
        if (maxHealth <= 0) maxHealth = 1;

        if (currentHealth <= 0)
            currentHealth = maxHealth;

        OnHealthChanged?.Invoke((float)currentHealth / maxHealth);
    }

    /// <summary>
    /// Used by PlayerStats / EnemyStats to sync MaxHP from stats.
    /// keepPercent = true: keep current health percent when MaxHp changes.
    /// </summary>
    public void SetMaxHealthFromStats(int newMaxHealth, bool keepPercent = true)
    {
        if (newMaxHealth <= 0) newMaxHealth = 1;

        float pct = (maxHealth > 0) ? (float)currentHealth / maxHealth : 1f;

        maxHealth = newMaxHealth;

        if (keepPercent)
            currentHealth = Mathf.RoundToInt(maxHealth * pct);
        else
            currentHealth = maxHealth;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke((float)currentHealth / maxHealth);
    }

    public void TakeDamage(int amount)
    {
        OnDamageTaken?.Invoke();
        if (IsDead) return;
        if (amount <= 0) return;

        FloatingText.Create(transform.position, "-" + amount.ToString(), Color.red, 1.0f, 0.35f);

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        float pct = (float)currentHealth / maxHealth;
        
        OnHealthChanged?.Invoke(pct);

        if (currentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        if (amount <= 0) return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        OnHealthChanged?.Invoke((float)currentHealth / maxHealth);
    }

    void Die()
    {
        if (IsDead) return;
        IsDead = true;
        
        OnHealthChanged?.Invoke(0f);
        OnDeath?.Invoke();
        
        Destroy(gameObject);
    }
}