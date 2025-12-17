using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class InventorySlot
{
    public ItemDefinition itemDefinition; // Reference to the original data
    public int quantity;                  // Current quantity in bag

    public InventorySlot(ItemDefinition item, int amount)
    {
        this.itemDefinition = item;
        this.quantity = amount;
    }

    public void AddAmount(int amount)
    {
        quantity += amount;
    }
}

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [Header("Currency")]
    public int currentGems = 0; // Primary currency (Gem/Spirit Stone)

    [Header("Items")]
    public List<InventorySlot> items = new List<InventorySlot>();

    // Events for UI updates
    public event Action OnInventoryChanged;
    public event Action<int> OnGemChanged; // Returns new gem amount for UI display

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // --- Currency Logic (Gems Only) ---
    public void AddGems(int amount)
    {
        currentGems += amount;
        Debug.Log($"<color=cyan>[Inventory] Received {amount} Gems. Total: {currentGems}</color>");
        
        // Notify UI to update
        OnGemChanged?.Invoke(currentGems);
    }

    public bool SpendGems(int amount)
    {
        if (currentGems >= amount)
        {
            currentGems -= amount;
            OnGemChanged?.Invoke(currentGems);
            return true;
        }
        return false;
    }

    // --- Item Logic ---
    public void AddItem(ItemDefinition item, int amount)
    {
        if (item == null || amount <= 0) return;

        // Check if item already exists in inventory to stack quantity
        InventorySlot existingSlot = items.Find(slot => slot.itemDefinition == item);

        if (existingSlot != null)
        {
            existingSlot.AddAmount(amount);
            Debug.Log($"[Inventory] Stacked item: {item.itemName} (+{amount})");
        }
        else
        {
            // Create new slot if item doesn't exist
            InventorySlot newSlot = new InventorySlot(item, amount);
            items.Add(newSlot);
            Debug.Log($"[Inventory] New item: {item.itemName} (x{amount})");
        }

        // Notify UI to update inventory grid
        OnInventoryChanged?.Invoke();
    }
}