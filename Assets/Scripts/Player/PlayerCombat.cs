using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class PlayerCombat : MonoBehaviour
{
    public int damage = 10;
    [Tooltip("Attacks per second")]
    public float attackSpeed = 1f;
    public float attackRange = 1.2f;
    [Tooltip("Effect to spawn when hitting a target")]
    public GameObject hitEffectPrefab;

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
        MouseInput.OnRightClickTarget += HandleRightClickOnTarget;
        if (stats != null)
        {
            stats.OnStatsChanged += UpdateStats;
            UpdateStats();
        }
    }

    void OnDisable()
    {
        MouseInput.OnRightClickTarget -= HandleRightClickOnTarget;
        if (stats != null)
            stats.OnStatsChanged -= UpdateStats;
    }

    void UpdateStats()
    {
        if (stats == null) return;
        damage = stats.Attack;
        attackSpeed = stats.GetStat(StatType.AttackSpeed);
    }

    void HandleRightClickOnTarget(IDamagable target)
    {
        if (target == null || target.IsDead) return;

        currentTarget = target;
        // move player to the target and stop at attackRange
        movement.MoveToTarget(target.Transform, attackRange);
        // start attack routine (it will wait until in range)
        if (attackRoutine != null) StopCoroutine(attackRoutine);
        attackRoutine = StartCoroutine(AttackWhenInRange());
    }

    IEnumerator AttackWhenInRange()
    {
        while (currentTarget != null && !currentTarget.IsDead)
        {
            // wait until within range
                 while (currentTarget != null && !currentTarget.IsDead &&
                     Vector3.Distance(transform.position, currentTarget.Transform.position) > attackRange)
            {
                yield return null;
            }

            if (currentTarget == null || currentTarget.IsDead) break;

            // perform attacks at attackSpeed
            float interval = Mathf.Max(0.01f, 1f / Mathf.Max(0.0001f, attackSpeed));

            // Check cooldown to prevent spamming
            if (Time.time < lastAttackTime + interval)
            {
                yield return new WaitForSeconds((lastAttackTime + interval) - Time.time);
                continue;
            }

            lastAttackTime = Time.time;
            
            // // Spawn Hit Effect (3D)
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

            currentTarget.TakeDamage(damage);

            // if target died after hit, break
            if (currentTarget.IsDead) break;

            yield return new WaitForSeconds(interval);
        }

        // cleanup: stop following target and end combat
        movement.StopMoving();
        currentTarget = null;
        attackRoutine = null;
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
}