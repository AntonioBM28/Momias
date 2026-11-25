using UnityEngine;

/// <summary>
/// Proyectil que disparan enemigos a distancia (Escupidora).
/// - Vuela con fisica parabolica (gravedad del rb).
/// - Dania al Player al tocarlo.
/// - Se autodestruye al tocar Ground/Boundary, al vencer tiempoVida, o por campanazo.
/// - Anima cambiando sprite de array.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyProjectile : MonoBehaviour
{
    [Header("Dano")]
    [SerializeField] private int danio = 2;

    [Header("Animacion sprite")]
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float velocidadFrame = 0.08f;

    [Header("Autodestruct")]
    [SerializeField] private float tiempoVida = 3f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float tiempoProximoFrame;
    private int frameIdx;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        var col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        if (rb.gravityScale <= 0f) rb.gravityScale = 1.5f;
    }

    void Start()
    {
        Destroy(gameObject, tiempoVida);
        if (frames != null && frames.Length > 0) sr.sprite = frames[0];
    }

    void Update()
    {
        if (frames != null && frames.Length > 1 && Time.time >= tiempoProximoFrame)
        {
            frameIdx = (frameIdx + 1) % frames.Length;
            sr.sprite = frames[frameIdx];
            tiempoProximoFrame = Time.time + velocidadFrame;
        }
    }

    /// <summary>Lanza el proyectil con velocidad inicial (parabolica).</summary>
    public void Lanzar(Vector2 velocidadInicial)
    {
        rb.linearVelocity = velocidadInicial;
    }

    /// <summary>Lo destruye con pequeno VFX (lo llama AttackHitbox al impacto del campanazo).</summary>
    public void DestruirPorCampanazo()
    {
        DustBurst.Spawn(transform.position, new Color(0.5f, 0.75f, 0.3f, 1f));
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null && !ph.EsInvulnerable)
                ph.RecibirDanio(danio, transform.position);
            DustBurst.Spawn(transform.position, new Color(0.5f, 0.75f, 0.3f, 1f));
            Destroy(gameObject);
            return;
        }

        int layer = other.gameObject.layer;
        if (layer == 6 || layer == 7)
        {
            DustBurst.Spawn(transform.position, new Color(0.5f, 0.75f, 0.3f, 1f));
            Destroy(gameObject);
        }
    }
}
