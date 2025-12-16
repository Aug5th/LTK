using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float stopDistance = 0.1f; // Slightly increased to make stopping easier
    public float rotateSpeed = 10f;   // Rotation speed

    private Rigidbody rb;
    private UnitStats stats;
    private Vector3 targetPosition;
    private bool hasTarget = false;

    private Transform followTarget;
    private float currentStopDistance;
    
    [SerializeField] private bool isMoving;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        stats = GetComponent<UnitStats>();
        targetPosition = rb.position;
        currentStopDistance = stopDistance;
        
        // Prevent tipping: freeze X and Z rotations
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
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

        // Update target position if following a target
        if (followTarget != null)
        {
            if (followTarget == null)
            {
                StopMoving();
                return;
            }
            targetPosition = followTarget.position;
        }

        // Compute distance
        // Use a flat vector (ignore Y) to avoid pushing into the ground
        Vector3 currentPos = rb.position;
        Vector3 targetPosFlat = new Vector3(targetPosition.x, currentPos.y, targetPosition.z);
        Vector3 dir = targetPosFlat - currentPos;
        float distance = dir.magnitude;

        // Stopping logic
        if (distance <= currentStopDistance)
        {
            // If following a target (e.g., enemy): keep hasTarget = true but stop moving
            if (followTarget != null)
            {
                rb.velocity = Vector3.zero;
                isMoving = false;
                // Keep rotating to face the target while idle (combat style)
                RotateTowards(dir); 
            }
            else
            {
                // Move-to click destination reached -> fully stop
                StopMoving();
            }
            return;
        }

        // Movement step
        isMoving = true;
        Vector3 moveDir = dir.normalized;
        
        // 1) Rotate towards movement direction
        RotateTowards(moveDir);

        // 2) Compute this frame's step
        float step = moveSpeed * Time.fixedDeltaTime;

        // 3) Prevent overshoot: if the step exceeds remaining distance, snap to the target
        if (step >= distance)
        {
             // Snap to target to avoid jitter
             rb.MovePosition(targetPosFlat);
        }
        else
        {
             // Move normally
             rb.MovePosition(currentPos + moveDir * step);
        }
    }
    
    // Helper to rotate smoothly
    void RotateTowards(Vector3 dir)
    {
        if (dir == Vector3.zero) return;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        // Use MoveRotation for physics compatibility
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRot, rotateSpeed * Time.fixedDeltaTime));
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
        followTarget = target;
        currentStopDistance = Mathf.Max(0.1f, stopDist); // Ensure not too small
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
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero; 
        isMoving = false;
    }
}