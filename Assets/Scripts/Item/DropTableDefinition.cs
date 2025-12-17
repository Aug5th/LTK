using System;
using UnityEngine;

[CreateAssetMenu(menuName = "TuTien/Drop Table")]
public class DropTableDefinition : ScriptableObject
{
    [Header("Link")]
    public string dropTableId;  // "DT_LinhThu"
    public string unitId;       // "Enemy_LinhThu"

    [Header("Rewards - Basic")]
    public int expReward = 0;

    [Header("Currency Rewards (Gem)")]
    [Range(0f, 1f)] public float gemDropChance = 1f;
    public int gemMin = 0;
    public int gemMax = 0;

    [Header("Item Rewards")]
    public ItemDropEntry[] itemDrops;
}

[Serializable]
public class ItemDropEntry
{
    public ItemDefinition item;   // ScriptableObject định nghĩa item
    [Range(0f, 1f)] public float dropChance = 1f;
    public int minAmount = 1;
    public int maxAmount = 1;
}