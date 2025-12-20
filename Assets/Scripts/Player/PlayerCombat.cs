using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class PlayerCombat : MonoBehaviour
{
    public int damage = 10;
    [Tooltip("Attacks per second")]
    public float attackSpeed = 1f;
    public float attackRange = 1.5f; // Recommended to be slightly higher than Movement's stopDistance
    
    [Header("Auto Attack")]
    [Tooltip("Radius to auto-acquire enemies when idle")]
    public float autoAttackRadius = 5f;
    [Tooltip("Layers considered as valid attack targets")]
    public LayerMask attackableLayers = ~0;

    private IDamagable currentTarget;
    private Movement movement;
    private UnitStats stats;
    private Coroutine attackRoutine;
    private float lastAttackTime = -100f; // make sure we can attack immediately

    void Awake()
    {
        movement = GetComponent<Movement>();
        stats = GetComponent<UnitStats>();
    }

    void OnEnable()
    {
        // Subscribe to input events
        MouseInput.OnRightClickTarget += HandleRightClickOnTarget;
        if (stats != null)
        {
            stats.OnStatsChanged += UpdateStats;
            UpdateStats();
        }
    }

    void OnDisable()
    {
        // Unsubscribe from input events
        MouseInput.OnRightClickTarget -= HandleRightClickOnTarget;
        if (stats != null)
            stats.OnStatsChanged -= UpdateStats;
    }

    void UpdateStats()
    {
        if (stats == null) return;
        damage = stats.Attack;
        // Ensure a sane minimum attacks-per-second
        attackSpeed = Mathf.Max(0.1f, stats.GetStat(StatType.AttackSpeed));
    }

    void Update()
    {
        if (!movement.IsPlayerMoving())
        {
            TryAutoAcquireTarget();
        }
    }

    void HandleRightClickOnTarget(IDamagable target)
    {
        if (target == null || target.IsDead) return;
        
        // Only attack targets on valid layers
        if (((1 << target.Transform.gameObject.layer) & attackableLayers) == 0) return;

        currentTarget = target;

        float moveStopDistance = Mathf.Max(0.5f, attackRange - 0.1f);
        movement.MoveToTarget(target.Transform, moveStopDistance);

        if (attackRoutine != null) StopCoroutine(attackRoutine);
        attackRoutine = StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop()
    {
        // Main combat loop
        while (currentTarget != null && !currentTarget.IsDead)
        {
            float dist = Vector2.Distance((Vector2)transform.position, (Vector2)currentTarget.Transform.position);

            // If out of range, just wait for Movement script to bring us closer
            if (dist > attackRange)
            {
                yield return null; 
                continue;
            }

            // Optional: Manually rotate towards the target while attacking (overrides Movement script's rotation when stopped)
            Vector2 dir = (Vector2)(currentTarget.Transform.position - transform.position);
            if (dir != Vector2.zero) 
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                angle -= 90f; // Adjust for sprite orientation
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * 20f);
            }

            // Check Cooldown
            float cooldown = 1f / attackSpeed;
            if (Time.time >= lastAttackTime + cooldown)
            {
                // Perform the attack
                PerformAttack();
                lastAttackTime = Time.time;
            }

            // Wait for the next frame to check again
            yield return null;
        }

        // Target died or combat ended -> Reset
        StopCombat();
    }

    void PerformAttack()
    {
        if (currentTarget == null) return;
        currentTarget.TakeDamage(damage);
    }

    void TryAutoAcquireTarget()
    {
        // Use OverlapCircle to find nearby targets in 2D
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, autoAttackRadius, attackableLayers);
        
        if(hits.Length == 0) return;
        
        IDamagable nearestTarget = null;
        float bestDist = float.MaxValue;

        foreach (var col in hits)
        {
            // Skip self
            if (col.attachedRigidbody != null && col.attachedRigidbody.gameObject == gameObject) continue;

            // Find IDamagable component
            IDamagable target = col.GetComponent<IDamagable>();
            if (target == null && col.attachedRigidbody != null) 
                target = col.attachedRigidbody.GetComponent<IDamagable>();

            if (target != null && !target.IsDead)
            {
                float d = Vector2.Distance((Vector2)transform.position, (Vector2)target.Transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    nearestTarget = target;
                }
            }
        }

        if (nearestTarget != null)
        {
            // Start combat with the nearest acquired target
            HandleRightClickOnTarget(nearestTarget);
        }
    }

    public void StopCombat()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
        currentTarget = null;
        movement.StopMoving();
    }

    // VISUALIZATION LOGIC
    void OnDrawGizmos()
    {
        // 1. Draw Attack Range (Red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 2. Draw Auto Attack Radius (Green)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, autoAttackRadius);
    }
}