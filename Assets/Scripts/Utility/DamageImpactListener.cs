using UnityEngine;

[RequireComponent(typeof(Health))]
public class DamageImpactListener : MonoBehaviour
{
    [Header("VFX Settings")]
    public GameObject hitVfxPrefab;
    
    public float surfaceOffset = 0.1f;

    public float defaultHeight = 1.0f;

    private Health health;
    private Collider2D ownCollider;
    private Transform playerTransform;

    private void Awake()
    {
        health = GetComponent<Health>();
        ownCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private void OnEnable()
    {
        if (health != null) health.OnDamageTaken += PlayHitEffect;
    }

    private void OnDisable()
    {
        if (health != null) health.OnDamageTaken -= PlayHitEffect;
    }

    private void PlayHitEffect()
    {
        PlayHitEffectLogic();
    }

    private void PlayHitEffectLogic()
    {
        if (hitVfxPrefab == null) return;

        Vector3 spawnPos;
        
        if (playerTransform != null)
        {
            Vector2 direction = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
            Vector2 basePos = (Vector2)transform.position + (direction * 0.5f);
            spawnPos = new Vector3(basePos.x, basePos.y + 0.5f, 0f);
        }
        else
        {
            spawnPos = transform.position + new Vector3(0, 1.0f, 0);
        }

        GameObject vfxInstance = Instantiate(hitVfxPrefab, spawnPos, Quaternion.identity);
        Destroy(vfxInstance, 1.0f);
    }
}