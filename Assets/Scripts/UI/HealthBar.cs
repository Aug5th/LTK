using UnityEngine;

[ExecuteAlways]
public class HealthBar : MonoBehaviour
{
    [Tooltip("Assign a simple 1x1 white sprite (SpriteRenderer-compatible)")]
    public Sprite barSprite;
    public Vector3 offset = new Vector3(0f, 0.7f, 0f);
    public float width = 1.0f;
    public float height = 0.12f;
    public int sortingOrder = 500;

    public Color colorGreen = new Color(0.16f, 0.78f, 0.3f, 1f);
    public Color colorYellow = new Color(1f, 0.85f, 0f, 1f);
    public Color colorRed = new Color(1f, 0.16f, 0.16f, 1f);
    public Color bgColor = new Color(0.15f, 0.15f, 0.15f, 1f);

    public Health targetHealth;

    // internal refs
    Transform fg;
    Transform bg;
    SpriteRenderer fgRenderer;
    SpriteRenderer bgRenderer;

    void Awake()
    {
        SetupBar();
        HookTarget();
    }

    void OnValidate()
    {
        // keep editor preview stable and avoid creating duplicates repeatedly
        SetupBar();
        HookTarget();
    }

    void OnEnable()
    {
        HookTarget();
    }

    void OnDisable()
    {
        if (targetHealth != null)
            targetHealth.OnHealthChanged -= UpdateBar;
    }

    void SetupBar()
    {
        // reuse existing children named HP_BG / HP_FG (avoid duplicates)
        bg = FindFirstChildByName("HP_BG");
        fg = FindFirstChildByName("HP_FG");

        // remove extra duplicates (keep first)
        RemoveExtraChildrenNamed("HP_BG");
        RemoveExtraChildrenNamed("HP_FG");

        if (bg == null)
        {
            GameObject g = new GameObject("HP_BG");
            g.transform.SetParent(transform, false);
            bg = g.transform;
            bgRenderer = g.AddComponent<SpriteRenderer>();
            bgRenderer.sortingOrder = 500; // ensure visible
        }
        if (fg == null)
        {
            GameObject f = new GameObject("HP_FG");
            f.transform.SetParent(transform, false);
            fg = f.transform;
            fgRenderer = f.AddComponent<SpriteRenderer>();
            fgRenderer.sortingOrder = 501;
        }

        // get or add renderers
        bgRenderer = bg.GetComponent<SpriteRenderer>() ?? bg.gameObject.AddComponent<SpriteRenderer>();
        fgRenderer = fg.GetComponent<SpriteRenderer>() ?? fg.gameObject.AddComponent<SpriteRenderer>();

        // assign sprite if present
        if (barSprite != null)
        {
            bgRenderer.sprite = barSprite;
            fgRenderer.sprite = barSprite;
        }

        bgRenderer.color = bgColor;
        fgRenderer.color = colorGreen;

        bgRenderer.sortingOrder = sortingOrder;
        fgRenderer.sortingOrder = sortingOrder + 1;

        // ensure transforms are reset
        bg.localRotation = Quaternion.identity;
        fg.localRotation = Quaternion.identity;

        // position and scale - we use center-pivot sprite and adjust fg position so left edge is fixed
        bg.localPosition = offset;
        // convert desired world width/height into local scale using the sprite's bounds so BG matches FG
        {
            float spriteUnitWidth = 1f;
            float spriteUnitHeight = 1f;
            if (bgRenderer.sprite != null)
            {
                var b = bgRenderer.sprite.bounds;
                spriteUnitWidth = Mathf.Approximately(b.size.x, 0f) ? 1f : b.size.x;
                spriteUnitHeight = Mathf.Approximately(b.size.y, 0f) ? 1f : b.size.y;
            }
            bg.localScale = new Vector3(width / spriteUnitWidth, height / spriteUnitHeight, 1f);
        }

        // set fg container to position left edge: offset + left
        float left = -width * 0.5f;
        fg.localPosition = offset + new Vector3(left, 0f, 0f);

        // initial full fg
        UpdateBar(1f);
    }

    void HookTarget()
    {
        if (targetHealth == null)
        {
            targetHealth = GetComponentInParent<Health>();
        }

        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= UpdateBar;
            targetHealth.OnHealthChanged += UpdateBar;
            UpdateBar((float)targetHealth.currentHealth / Mathf.Max(1, targetHealth.maxHealth));
        }
    }

    void UpdateBar(float pct)
    {
        pct = Mathf.Clamp01(pct);
        if (fg == null || bg == null || fgRenderer == null || bgRenderer == null) return;

        // bg stays centered at offset with width = width
        bg.localPosition = offset;

        // get sprite's unit size (bounds) to convert world size -> local scale
        float spriteUnitWidth = 1f;
        float spriteUnitHeight = 1f;
        if (fgRenderer.sprite != null)
        {
            var b = fgRenderer.sprite.bounds;
            // safety: avoid zero
            spriteUnitWidth = Mathf.Approximately(b.size.x, 0f) ? 1f : b.size.x;
            spriteUnitHeight = Mathf.Approximately(b.size.y, 0f) ? 1f : b.size.y;
        }

        // make BG use same conversion so BG and FG scales are consistent
        bg.localScale = new Vector3(width / spriteUnitWidth, height / spriteUnitHeight, 1f);

        // compute desired world-space size for fg
        float desiredWidth = width * pct;
        float desiredHeight = height;

        // set fg scale so the sprite occupies desired world size (handles arbitrary sprite pivots)
        float localScaleX = desiredWidth / spriteUnitWidth;
        float localScaleY = desiredHeight / spriteUnitHeight;
        fg.localScale = new Vector3(localScaleX, localScaleY, 1f);

        // compute left edge (bg center is 0, left = -width/2), fg center = left + desiredWidth/2
        float left = -width * 0.5f;
        float fgCenterX = left + (desiredWidth * 0.5f);
        fg.localPosition = offset + new Vector3(fgCenterX, 0f, 0f);

        // color by thresholds
        if (pct > 0.6f) fgRenderer.color = colorGreen;
        else if (pct > 0.3f) fgRenderer.color = colorYellow;
        else fgRenderer.color = colorRed;

        bool visible = pct > 0f;
        bgRenderer.enabled = visible;
        fgRenderer.enabled = visible;
    }

    Transform FindFirstChildByName(string name)
    {
        var t = transform.Find(name);
        if (t != null) return t;
        // fallback: search all children
        for (int i = 0; i < transform.childCount; i++)
        {
            var c = transform.GetChild(i);
            if (c.name == name) return c;
        }
        return null;
    }

    void RemoveExtraChildrenNamed(string name)
    {
        Transform first = null;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var c = transform.GetChild(i);
            if (c.name != name) continue;
            if (first == null) first = c;
            else
            {
                // destroy duplicates in editor/playmode
#if UNITY_EDITOR
                if (!Application.isPlaying) UnityEditor.Undo.DestroyObjectImmediate(c.gameObject);
                else Destroy(c.gameObject);
#else
                Destroy(c.gameObject);
#endif
            }
        }
    }

    void OnDestroy()
    {
        if (targetHealth != null)
            targetHealth.OnHealthChanged -= UpdateBar;
    }
}