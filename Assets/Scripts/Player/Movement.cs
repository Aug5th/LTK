using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float stopDistance = 0.05f;

    private Rigidbody rb;
    private UnitStats stats;
    private Vector3 targetPosition;
    private bool hasTarget = false;

    // new: optional follow target (e.g., enemy)
    private Transform followTarget;
    private float currentStopDistance;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        stats = GetComponent<UnitStats>();
        targetPosition = rb.position;
        currentStopDistance = stopDistance;
    }

    void OnEnable()
    {
        MouseInput.OnRightClick += HandleMouseRightClick;
        if (stats != null)
        {
            stats.OnStatsChanged += UpdateStats;
            UpdateStats();
        }
    }

    void OnDisable()
    {
        MouseInput.OnRightClick -= HandleMouseRightClick;
        if (stats != null)
            stats.OnStatsChanged -= UpdateStats;
    }

    void UpdateStats()
    {
        if (stats == null) return;
        moveSpeed = stats.MoveSpeed;
    }

    void HandleMouseRightClick(Vector3 worldPosition)
    {
        MoveTo(worldPosition);
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        if (!hasTarget)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        // if following a transform, update targetPosition from it
        if (followTarget != null)
        {
            // if followTarget was destroyed, Unity's == null will be true
            if (followTarget == null)
            {
                followTarget = null;
                hasTarget = false;
                rb.velocity = Vector3.zero;
                return;
            }

            targetPosition = followTarget.position;
        }

        Vector3 currentPos = rb.position;
        Vector3 dir = targetPosition - currentPos;
        float distance = dir.magnitude;

        // If following a moving target, do NOT clear hasTarget when within stop distance.
        // Instead stop movement but keep following so if the target moves away we'll resume.
        if (followTarget != null)
        {
            if (distance <= currentStopDistance)
            {
                rb.velocity = Vector3.zero;
                return; // keep hasTarget true so following continues
            }
        }
        else
        {
            if (distance <= currentStopDistance)
            {
                hasTarget = false;
                rb.velocity = Vector3.zero;
                return;
            }
        }

        Vector3 moveDir = dir.normalized;
        Vector3 newPos = currentPos + moveDir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    public void MoveTo(Vector3 worldPos)
    {
        followTarget = null;
        currentStopDistance = stopDistance;
        targetPosition = worldPos;
        hasTarget = true;
    }

    // new: follow a transform until within stopDistance (used for attacking enemies)
    public void MoveToTarget(Transform target, float stopDist)
    {
        if (target == null) return;
        followTarget = target;
        currentStopDistance = Mathf.Max(0.01f, stopDist);
        hasTarget = true;
    }

    public void StopMoving()
    {
        followTarget = null;
        hasTarget = false;
        rb.velocity = Vector3.zero;
    }
}