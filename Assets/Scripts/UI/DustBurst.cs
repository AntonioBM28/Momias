using UnityEngine;

/// <summary>
/// Partículas de polvo al aterrizar saltos o al morir enemigos.
/// Spawnea 6-10 mini GameObjects con SpriteRenderer + Rigidbody2D que se
/// dispersan radialmente y hacen fade-out antes de autodestruirse.
/// Es Singleton no estricto: tiene un slot para el sprite default, y el metodo
/// estatico Spawn() lo usa automaticamente. Si no hay Instance, usa un sprite 1x1.
/// </summary>
public class DustBurst : MonoBehaviour
{
    public static DustBurst Instance { get; private set; }

    [Header("Sprite de particula")]
    [SerializeField] private Sprite dustSprite;

    [Header("Defaults al spawnear")]
    [SerializeField] private int cantidadMin = 6;
    [SerializeField] private int cantidadMax = 10;
    [SerializeField] private float velocidadMin = 1.5f;
    [SerializeField] private float velocidadMax = 3.5f;
    [SerializeField] private float duracionVida = 0.5f;
    [SerializeField] private float gravityScale = 0.6f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>Crea una explosion de polvo en la posicion dada con el tinte opcional.</summary>
    public static void Spawn(Vector3 posicion, Color? tinte = null)
    {
        if (Instance == null)
        {
            Debug.LogWarning("[DustBurst] No hay instancia activa, llamada ignorada.");
            return;
        }
        Instance.SpawnInterno(posicion, tinte ?? new Color(0.84f, 0.75f, 0.59f, 1f));
    }

    private void SpawnInterno(Vector3 posicion, Color tinte)
    {
        int cantidad = Random.Range(cantidadMin, cantidadMax + 1);
        for (int i = 0; i < cantidad; i++)
        {
            GameObject p = new GameObject("DustParticle");
            p.transform.position = posicion + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(0f, 0.1f), 0f);

            SpriteRenderer sr = p.AddComponent<SpriteRenderer>();
            sr.sprite = dustSprite;
            sr.color = tinte;
            sr.sortingOrder = 12;

            Rigidbody2D rb = p.AddComponent<Rigidbody2D>();
            rb.gravityScale = gravityScale;
            // Dispersion radial hacia arriba (0-180 grados)
            float angulo = Random.Range(20f, 160f) * Mathf.Deg2Rad;
            float vel = Random.Range(velocidadMin, velocidadMax);
            rb.linearVelocity = new Vector2(Mathf.Cos(angulo) * vel, Mathf.Sin(angulo) * vel);

            // Escala aleatoria pequena
            float s = Random.Range(0.7f, 1.3f);
            p.transform.localScale = new Vector3(s, s, 1f);

            p.AddComponent<DustParticle>().Init(duracionVida);
        }
    }
}

/// <summary>Helper interno: hace fade-out y autodestruye una particula.</summary>
public class DustParticle : MonoBehaviour
{
    private SpriteRenderer sr;
    private float vida;
    private float t;

    public void Init(float vidaMax)
    {
        vida = vidaMax;
    }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        t += Time.deltaTime;
        if (sr != null)
        {
            Color c = sr.color;
            c.a = Mathf.Lerp(1f, 0f, t / vida);
            sr.color = c;
        }
        if (t >= vida) Destroy(gameObject);
    }
}
