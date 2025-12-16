using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatsContainer
{
    [SerializeField]
    private List<StatValue> baseStats = new List<StatValue>();

    private Dictionary<StatType, float> _statDict;

    void EnsureDict()
    {
        if (_statDict != null) return;
        _statDict = new Dictionary<StatType, float>();
        foreach (var s in baseStats)
        {
            if (_statDict.ContainsKey(s.statType))
                _statDict[s.statType] += s.value;
            else
                _statDict[s.statType] = s.value;
        }
    }

    public float GetStat(StatType type)
    {
        EnsureDict();
        if (_statDict.TryGetValue(type, out float v))
            return v;
        return 0f;
    }

    public void AddToStat(StatType type, float delta)
    {
        EnsureDict();
        float current = GetStat(type);
        _statDict[type] = current + delta;

        bool found = false;
        for (int i = 0; i < baseStats.Count; i++)
        {
            if (baseStats[i].statType == type)
            {
                baseStats[i] = new StatValue { statType = type, value = _statDict[type] };
                found = true;
                break;
            }
        }
        if (!found)
        {
            baseStats.Add(new StatValue { statType = type, value = _statDict[type] });
        }
    }

    public void AddFrom(StatsContainer other)
    {
        if (other == null) return;
        foreach (StatType type in Enum.GetValues(typeof(StatType)))
        {
            float v = other.GetStat(type);
            if (Mathf.Abs(v) > 0.0001f)
                AddToStat(type, v);
        }
    }

    public IReadOnlyList<StatValue> GetAllRawStats() => baseStats;
}