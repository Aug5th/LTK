using UnityEngine;

public class FloatingText : MonoBehaviour
{
    TextMesh tm;
    float elapsed;
    float waitTime = 0.5f;   // sau 0.5s mới bắt đầu mờ
    float fadeTime = 0.5f;   // thời gian mờ
    Vector3 velocity;
    Color baseColor;

    static readonly Quaternion IsometricRotation = Quaternion.Euler(30f, 45f, 0f);

    void Awake()
    {
        tm = GetComponent<TextMesh>();
        if (tm == null) tm = gameObject.AddComponent<TextMesh>();
        // default
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.characterSize = 0.08f;
        tm.fontSize = 64;
        tm.richText = false;
        baseColor = tm.color;
        
        velocity = new Vector3(0f, 1f, 0f);

        transform.rotation = IsometricRotation;
    }

    void Update()
    {
        // move and billboard facing isometric camera
        transform.position += velocity * Time.deltaTime;
    
        elapsed += Time.deltaTime;
        if (elapsed > waitTime)
        {
            float t = Mathf.Clamp01((elapsed - waitTime) / fadeTime);
            var c = baseColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            tm.color = c;
        }

        if (elapsed >= (waitTime + fadeTime))
            Destroy(gameObject);
    }

    // Convenience factory - call this to spawn floating text
    public static void Create(Vector3 worldPos, string text, Color color, float verticalOffset = 0.8f, float randomRadius = 0.25f)
    {
        // random small offset around top of enemy
        Vector2 rnd = Random.insideUnitCircle * randomRadius;
        Vector3 spawn = worldPos + new Vector3(rnd.x, verticalOffset + Mathf.Abs(rnd.y), rnd.y * 0.1f);

        GameObject go = new GameObject("FloatingText");
        go.transform.position = spawn;
        var tm = go.AddComponent<TextMesh>();
        tm.text = text;
        tm.color = color;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.characterSize = 0.08f;
        tm.fontSize = 64;
        tm.richText = false;

        // add behaviour
        var ft = go.AddComponent<FloatingText>();
        ft.baseColor = color;
    }
}