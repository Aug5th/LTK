using UnityEngine;

[RequireComponent(typeof(Health))]
public class DamageImpactListener : MonoBehaviour
{
    [Header("VFX Settings")]
    public GameObject hitVfxPrefab;
    
    public float surfaceOffset = 0.1f;

    public float defaultHeight = 1.0f;

    private Health health;
    private Collider ownCollider;
    private Transform playerTransform;

    private void Awake()
    {
        health = GetComponent<Health>();
        ownCollider = GetComponent<Collider>();
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
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            
            spawnPos = transform.position + (direction * 0.5f);
            
            spawnPos.y += 0.5f; 
        }
        else
        {
            spawnPos = transform.position + new Vector3(0, 1.0f, 0);
        }

        GameObject vfxInstance = Instantiate(hitVfxPrefab, spawnPos, Quaternion.identity);
        Destroy(vfxInstance, 1.0f);
    }
}