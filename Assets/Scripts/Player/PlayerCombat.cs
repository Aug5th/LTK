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
    [Tooltip("Layers considered for auto-attack search (optional)")]
    public LayerMask autoAttackLayers = ~0;

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
        // 1. Check if the current target is still valid
        if (currentTarget != null)
        {
            bool isDead = currentTarget.IsDead;
            // If target is dead or the component was destroyed (null check for Unity Objects) -> Stop combat
            if (isDead || (currentTarget as Component) == null) 
            {
                StopCombat();
                return;
            }
        }

        // 2. Auto Acquire Logic (Only look for a new target when idle)
        if (!movement.IsPlayerMoving())
        {
            TryAutoAcquireTarget();
        }
    }

    void HandleRightClickOnTarget(IDamagable target)
    {
        if (target == null || target.IsDead) return;

        currentTarget = target;

        // FIX: Set MoveToTarget's stop distance slightly less than attackRange (e.g., 80%)
        // This prevents the player from standing exactly on the edge and jittering/missing attacks.
        float moveStopDistance = attackRange * 0.8f; 
        movement.MoveToTarget(target.Transform, moveStopDistance);

        // Start the attack loop
        if (attackRoutine != null) StopCoroutine(attackRoutine);
        attackRoutine = StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop()
    {
        // Main combat loop
        while (currentTarget != null && !currentTarget.IsDead)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.Transform.position);

            // If out of range, just wait for Movement script to bring us closer
            if (dist > attackRange)
            {
                yield return null; 
                continue;
            }

            // Optional: Manually rotate towards the target while attacking (overrides Movement script's rotation when stopped)
            Vector3 dir = currentTarget.Transform.position - transform.position;
            dir.y = 0;
            if (dir != Vector3.zero) 
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 20f);

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
        
        // Trigger attack animation here (if applicable)
        // anim.SetTrigger("Attack");

        currentTarget.TakeDamage(damage);
        
        // Spawn Hit Effect (3D) - Un-commented for reference
        // if (hitEffectPrefab != null)
        // {
        //     Vector3 hitPos = currentTarget.Transform.position;
        //     Collider col = currentTarget.Transform.GetComponent<Collider>();
        //     if (col != null)
        //     {
        //         hitPos = col.ClosestPoint(transform.position);
        //     }
        //     GameObject impact = Instantiate(hitEffectPrefab, hitPos, Quaternion.identity);
        //     Destroy(impact, 0.5f);
        // }
    }

    void TryAutoAcquireTarget()
    {
        // Use OverlapSphere to find nearby targets
        Collider[] hits = Physics.OverlapSphere(transform.position, autoAttackRadius, autoAttackLayers, QueryTriggerInteraction.Ignore);
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
                float d = Vector3.Distance(transform.position, target.Transform.position);
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