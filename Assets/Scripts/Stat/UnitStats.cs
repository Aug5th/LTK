using UnityEngine;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health))]
public class UnitStats : MonoBehaviour
{
    [Header("Base Definition (ScriptableObject)")]
    public UnitStatsDefinition baseDefinition;

    [Header("Runtime Final Stats (read-only)")]
    [SerializeField] private StatsContainer finalStats = new StatsContainer();

    // Event triggered when stats are recalculated
    public event Action OnStatsChanged;

    void Start()
    {
        RecalculateFinalStats();
    }

    /// <summary>
    /// Call when stats need to be recalculated(equirements, buff...). 
    /// </summary>
    public void RecalculateFinalStats(
        StatsContainer equipmentBonus = null,
        StatsContainer buffBonus      = null)
    {
        finalStats = new StatsContainer();

        // 1. Base from ScriptableObject
        if (baseDefinition != null && baseDefinition.baseStats != null)
            finalStats.AddFrom(baseDefinition.baseStats);

        // 2. Bonus from equipments
        if (equipmentBonus != null)
            finalStats.AddFrom(equipmentBonus);

        // 3. Bonus from buffs
        if (buffBonus != null)
            finalStats.AddFrom(buffBonus);

        // Notify listeners
        OnStatsChanged?.Invoke();
    }

    public float GetStat(StatType type) => finalStats.GetStat(type);

    // convenience properties
    public int Attack      => Mathf.RoundToInt(GetStat(StatType.Attack));
    public float MaxHP     => GetStat(StatType.MaxHP);
    public float MaxMP     => GetStat(StatType.MaxMP);
    public float MoveSpeed => GetStat(StatType.MoveSpeed);
}