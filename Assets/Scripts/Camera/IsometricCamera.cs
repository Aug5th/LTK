using UnityEngine;

[ExecuteInEditMode]
public class IsometricCamera : MonoBehaviour
{
    [Header("Edge Scrolling (Mouse)")]
    public float panBorderThickness = 24f;
    [Tooltip("Max camera speed (units/sec)")]
    public float maxPanSpeed = 18f;
    [Tooltip("Acceleration toward target speed (units/sec^2)")]
    public float acceleration = 90f;
    [Tooltip("Deceleration when stopping (units/sec^2)")]
    public float deceleration = 120f;
    [Tooltip("Curve mapping edge proximity [0..1] to speed factor [0..1]")]
    public AnimationCurve edgeSpeedCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("World Limits (XY)")]
    public Vector2 limitX = new Vector2(-100f, 100f);
    public Vector2 limitY = new Vector2(-100f, 100f);

    [Header("Setup")]
    public bool autoSetup = true;

    // Top-down 2D rotation: X = 90 (looking straight down), Y = 0
    private readonly Vector3 TOPDOWN_ROTATION = new Vector3(90f, 0f, 0f);

    // internal velocity for inertia
    private Vector2 _velocity; // 2D velocity in XY plane

    void Start()
    {
        SetupTopDown();
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            HandleEdgeScrollingInertia();
        }

        if (autoSetup)
        {
            SetupTopDown();
        }
    }

    void HandleEdgeScrollingInertia()
    {
        // Mouse proximity to edges -> directional target speed
        Vector2 edgeFactor = GetEdgeFactors(); // x: left(-)/right(+), y: down(-)/up(+)

        // Compute desired velocity in 2D (XY plane)
        Vector2 desiredVel = edgeFactor * maxPanSpeed;

        // Choose accel or decel based on magnitude
        float dt = Time.deltaTime;
        if (desiredVel.sqrMagnitude > 0.0001f)
        {
            // accelerate toward target velocity
            _velocity = Vector2.MoveTowards(_velocity, desiredVel, acceleration * dt);
        }
        else
        {
            // decelerate to zero
            if (_velocity.sqrMagnitude > 0.0001f)
                _velocity = Vector2.MoveTowards(_velocity, Vector2.zero, deceleration * dt);
            else
                _velocity = Vector2.zero;
        }

        // integrate position
        if (_velocity.sqrMagnitude > 0f)
        {
            Vector3 pos = transform.position;
            pos.x += _velocity.x * dt;
            pos.y += _velocity.y * dt;
            pos.x = Mathf.Clamp(pos.x, limitX.x, limitX.y);
            pos.y = Mathf.Clamp(pos.y, limitY.x, limitY.y);
            transform.position = pos;
        }
    }

    // Returns a vector2 where X is horizontal edge factor (-1..1), Y is vertical (-1..1)
    // Uses proximity to edges and maps via curve for smooth Diablo-like acceleration.
    Vector2 GetEdgeFactors()
    {
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;
        float w = Screen.width;
        float h = Screen.height;

        float fLeft  = Mathf.Clamp01(1f - (x / Mathf.Max(1f, panBorderThickness)));
        float fRight = Mathf.Clamp01(1f - ((w - x) / Mathf.Max(1f, panBorderThickness)));
        float fDown  = Mathf.Clamp01(1f - (y / Mathf.Max(1f, panBorderThickness)));
        float fUp    = Mathf.Clamp01(1f - ((h - y) / Mathf.Max(1f, panBorderThickness)));

        // map with curve for smoother start
        fLeft  = edgeSpeedCurve.Evaluate(fLeft);
        fRight = edgeSpeedCurve.Evaluate(fRight);
        fDown  = edgeSpeedCurve.Evaluate(fDown);
        fUp    = edgeSpeedCurve.Evaluate(fUp);

        // combine into signed factors (right-positive, up-positive)
        float fx = Mathf.Clamp(fRight - fLeft, -1f, 1f);
        float fy = Mathf.Clamp(fUp - fDown, -1f, 1f);

        // Normalize diagonal so speed is capped at maxPanSpeed
        Vector2 v = new Vector2(fx, fy);
        if (v.sqrMagnitude > 1f) v.Normalize();
        return v;
    }

    public void SetupTopDown()
    {
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographic = true;
            if (cam.orthographicSize < 1f) cam.orthographicSize = 7f;
        }

        transform.rotation = Quaternion.Euler(TOPDOWN_ROTATION);
    }
}
