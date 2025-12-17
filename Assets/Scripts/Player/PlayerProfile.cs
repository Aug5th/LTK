using UnityEngine;
using System;

public class PlayerProfile : MonoBehaviour
{
    public static PlayerProfile Instance { get; private set; }

    [Header("Level Stats")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int maxXP = 100; // XP required to reach the next level

    // Events for UI updates
    public event Action<float> OnXpChanged; // Returns progress percentage (0f -> 1f)
    public event Action<int> OnLevelUp;     // Returns new level

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        CalculateMaxXP(); // Calculate initial MaxXP
    }

    public void AddExperience(int amount)
    {
        currentXP += amount;
        Debug.Log($"<color=green>[Profile] Received {amount} XP.</color>");

        // Check for level up (Used 'while' in case multiple levels are gained at once)
        while (currentXP >= maxXP)
        {
            LevelUp();
        }

        // Notify UI
        float xpProgress = (float)currentXP / maxXP;
        OnXpChanged?.Invoke(xpProgress);
    }

    private void LevelUp()
    {
        currentXP -= maxXP;
        currentLevel++;
        
        CalculateMaxXP(); // Recalculate XP needed for the new level
        
        Debug.Log($"<color=red>[Profile] LEVEL UP! Current Level: {currentLevel}</color>");
        
        // TODO: Increase player stats (HP, Atk) here if needed
        OnLevelUp?.Invoke(currentLevel);
    }

    // XP Calculation Formula: Each level requires 20% more XP than the previous one
    private void CalculateMaxXP()
    {
        // Example: Lvl 1=100, Lvl 2=120, Lvl 3=150...
        maxXP = Mathf.RoundToInt(maxXP * 1.2f); 
    }
}