using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float stopDistance = 0.1f; // Slightly increased to make stopping easier
    public float rotateSpeed = 10f;   // Rotation speed

    private Rigidbody2D rb;
    private UnitStats stats;
    private Vector3 targetPosition;
    private bool hasTarget = false;

    private Transform followTarget;
    private float currentStopDistance;
    
    [SerializeField] private bool isMoving;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<UnitStats>();
        targetPosition = rb.position;
        currentStopDistance = stopDistance;
        
        // Freeze rotation in 2D
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
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
        if (!hasTarget) return;

        // Update target position
        if (followTarget != null)
        {
            if (followTarget == null) { StopMoving(); return; }
            targetPosition = followTarget.position;
        }

        Vector2 currentPos = rb.position;
        Vector2 targetPos2D = new Vector2(targetPosition.x, targetPosition.y);
        Vector2 dir = targetPos2D - currentPos;
        float distance = dir.magnitude;

        if (distance <= currentStopDistance)
        {
            if (isMoving)
            {
                rb.velocity = Vector2.zero;
                isMoving = false;
            }

            // If following a target (Combat), still rotate towards the target
            if (followTarget != null)
            {
                RotateTowards(dir); 
            }
            else
            {
                StopMoving(); // If it is normal mouse movement, cancel the target immediately
            }
            return;
        }

        // Movement step
        isMoving = true;
        Vector2 moveDir = dir.normalized;
        
        // 1) Rotate towards movement direction
        RotateTowards(moveDir);

        // 2) Compute this frame's step
        float step = moveSpeed * Time.fixedDeltaTime;

        // 3) Prevent overshoot: if the step exceeds remaining distance, snap to the target
        if (step >= distance)
        {
             // Snap to target to avoid jitter
             rb.MovePosition(targetPos2D);
        }
        else
        {
             // Move normally
             rb.MovePosition(currentPos + moveDir * step);
        }
    }
    
    // Helper to rotate smoothly in 2D (Z-axis rotation)
    void RotateTowards(Vector2 dir)
    {
        if (dir == Vector2.zero) return;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // Adjust for sprite orientation (assuming sprite faces right by default)
        angle -= 90f; // Adjust if sprite faces up by default, remove if sprite faces right
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime));
    }

    public void MoveTo(Vector3 worldPos)
    {
        followTarget = null;
        currentStopDistance = stopDistance;
        targetPosition = worldPos;
        hasTarget = true;
        isMoving = true;
    }

    public void MoveToTarget(Transform target, float stopDist)
    {
        if (target == null) return;
        
        // Check immediately: If already standing next to it, do not activate IsMoving anymore
        // To avoid 1 frame jitter
        float dist = Vector2.Distance((Vector2)transform.position, (Vector2)target.position);
        if (dist <= stopDist)
        {
             // Still set target to rotate face, but do not set isMoving = true
             followTarget = target;
             currentStopDistance = stopDist;
             hasTarget = true;
             isMoving = false; 
             return;
        }

        followTarget = target;
        currentStopDistance = Mathf.Max(0.1f, stopDist);
        hasTarget = true;
        isMoving = true;
    }

    public bool IsPlayerMoving()
    {
        return isMoving;
    }

    public void StopMoving()
    {
        followTarget = null;
        hasTarget = false;
        
        // Fully reset velocities to prevent drift
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f; 
        isMoving = false;
    }
}