using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Datos (ScriptableObject)")]
    [SerializeField] protected EnemyData data;

    [Header("Visual")]
    [SerializeField] protected Color colorFlash = Color.white;
    [SerializeField] protected float duracionFlash = 0.1f;

    [Header("Estado (solo lectura)")]
    [SerializeField] protected int vidaActual;

    [Header("Drops")]
    [SerializeField] protected GameObject heartPickupPrefab;
    [Range(0f, 1f)] [SerializeField] protected float heartDropChance = 0.20f;

    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator anim;
    protected Transform playerTransform;
    protected bool estaMuerto;

    public EnemyData Data => data;
    public bool EstaMuerto => estaMuerto;

    /// <summary>Permite al WaveManager asignar data despues de instanciar.</summary>
    public void SetData(EnemyData nuevoData)
    {
        data = nuevoData;
        if (gameObject.activeInHierarchy) AplicarData();
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        AplicarData();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
        else Debug.LogWarning($"[{name}] No se encontro GameObject con tag 'Player'.");
    }

    protected virtual void AplicarData()
    {
        if (data == null)
        {
            Debug.LogWarning($"[{name}] No tiene EnemyData asignado. Se usan valores por defecto.");
            vidaActual = 1;
            return;
        }
        vidaActual = data.vidaMaxima;
        sr.color = data.tinte;
        transform.localScale = new Vector3(data.escala, data.escala, 1f);
    }

    protected virtual void FixedUpdate()
    {
        if (estaMuerto || playerTransform == null) return;
        Mover();
    }

    protected abstract void Mover();

    public virtual void RecibirDanio(int cantidad, Vector2 origen)
    {
        if (estaMuerto) return;
        vidaActual -= cantidad;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayImpactoMomia();

        Vector2 dir = ((Vector2)transform.position - origen).normalized;
        rb.linearVelocity = new Vector2(dir.x * 3f, 2f);

        if (vidaActual <= 0) Morir();
        else StartCoroutine(FlashDanio());
    }

    protected System.Collections.IEnumerator FlashDanio()
    {
        Color original = sr.color;
        sr.color = colorFlash;
        yield return new WaitForSeconds(duracionFlash);
        if (!estaMuerto) sr.color = original;
    }

    protected virtual void Morir()
    {
        estaMuerto = true;
        HeartPickup.TrySpawnOnEnemyDeath(heartPickupPrefab, transform.position, heartDropChance);

        // Game feel: subclases pueden overridear FeedbackMuerte para ajustar la intensidad
        FeedbackMuerte();

        if (anim != null) anim.SetTrigger("Morir");
        // Frenar el movimiento horizontal pero dejar que la gravedad la lleve al piso.
        // Mantenemos el collider activo: por la matriz, Enemy<->Enemy y Player<->Enemy
        // ya estan apagados, asi que el cadaver solo colisiona con el suelo.
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        StartCoroutine(RutinaMorir());
    }

    /// <summary>
    /// Feedback visual al morir (shake, hitpause, polvo). Default para enemigos
    /// medianos. Las subclases chicas pueden bajar la intensidad y los bosses subirla.
    /// </summary>
    protected virtual void FeedbackMuerte()
    {
        if (CameraShaker.Instance != null) CameraShaker.Instance.Shake(0.08f, 0.08f);
        if (HitPause.Instance != null) HitPause.Instance.Freeze(0.06f);
        DustBurst.Spawn(transform.position, new Color(0.6f, 0.5f, 0.4f, 1f));
        if (AudioManager.Instance != null) AudioManager.Instance.PlayMuerteMomia();
    }

    private System.Collections.IEnumerator RutinaMorir()
    {
        // 1) Dejar que caiga/reproduzca anim de muerte si existe
        yield return new WaitForSeconds(0.4f);

        // 2) Ya en el suelo: congelar fisica para no seguir reaccionando
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // 3) Blink de alpha intermitente antes de desaparecer (clasico de arcade)
        Color colorOriginal = sr.color;
        float blinkTotal = 0.7f;
        float intervalo = 0.08f;
        int ciclos = Mathf.CeilToInt(blinkTotal / (intervalo * 2f));
        for (int i = 0; i < ciclos; i++)
        {
            Color c = sr.color; c.a = 0.2f; sr.color = c;
            yield return new WaitForSeconds(intervalo);
            c = sr.color; c.a = 1f; sr.color = c;
            yield return new WaitForSeconds(intervalo);
        }

        Destroy(gameObject);
    }
}
