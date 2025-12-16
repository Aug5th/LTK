using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Unit Stats Definition")]
public class UnitStatsDefinition : ScriptableObject
{
    [Header("Info")]
    public string unitId;      // "Player_KiemTu", "Enemy_LinhThu", "Boss_HoYeu", ...
    public string displayName;

    [Header("Base Stats")]
    public StatsContainer baseStats;

    [Header("Notes")]
    [TextArea]
    public string description;
}