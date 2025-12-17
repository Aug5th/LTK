using UnityEngine;

[RequireComponent(typeof(Health))]
public class LootDropper : MonoBehaviour
{
    [Header("Drop Table")]
    public DropTableDefinition dropTable;

    private Health health;

    void Awake()
    {
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        if (health != null)
            health.OnDeath += HandleDeath;
    }

    void OnDisable()
    {
        if (health != null)
            health.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (dropTable == null) return;

        DropLoot();
    }

    public void DropLoot()
    {
        // 1. EXP
        if (dropTable.expReward > 0)
        {
            Debug.Log($"[LootDropper] Dropped {dropTable.expReward} EXP from {name}");
            // TODO: Add logic to give EXP to player
        }

        // 2. Gems
        if (UnityEngine.Random.value <= dropTable.gemDropChance)
        {
            int amount = UnityEngine.Random.Range(dropTable.gemMin, dropTable.gemMax + 1);
            if (amount > 0)
            {
                Debug.Log($"[LootDropper] Dropped {amount} Gems from {name}");
                // TODO: Spawn Gem prefab
            }
        }

        // 3. Items
        if (dropTable.itemDrops != null)
        {
            foreach (var entry in dropTable.itemDrops)
            {
                if (entry.item == null) continue;

                if (UnityEngine.Random.value <= entry.dropChance)
                {
                    int amount = UnityEngine.Random.Range(entry.minAmount, entry.maxAmount + 1);
                    if (amount > 0)
                    {
                        Debug.Log($"[LootDropper] Dropped {amount} x {entry.item.itemName} from {name}");
                        // TODO: Spawn Item prefab
                    }
                }
            }
        }
    }
}
