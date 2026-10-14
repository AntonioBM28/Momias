using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Sistema de vida de Diego por medios corazones.
/// vidaActual se muestra en el inspector en runtime (gracias al [SerializeField]).
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Configuracion de vida")]
    [SerializeField] private int vidaMaxima = 12;
    [SerializeField] private float tiempoInvulnerabilidad = 1f;
    [SerializeField] private float fuerzaKnockback = 6f;

    [Header("Estado (solo lectura)")]
    [SerializeField] private int vidaActual;

    [Header("Visuales")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color colorDanio = Color.red;
    [SerializeField] private float parpadeoCadaSegundos = 0.1f;

    [Header("Eventos")]
    public UnityEvent<int, int> OnHealthChanged;
    public UnityEvent OnDamaged;
    public UnityEvent OnDeath;

    public int VidaActual => vidaActual;
    public int VidaMaxima => vidaMaxima;
    public bool EstaMuerto { get; private set; }
    public bool EsInvulnerable { get; private set; }

    private PlayerController controller;
    private Rigidbody2D rb;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        vidaActual = vidaMaxima;
    }

    void Start()
    {
        OnHealthChanged?.Invoke(vidaActual, vidaMaxima);
    }

    public void RecibirDanio(int cantidad, Vector2 origenDanio)
    {
        if (EstaMuerto || EsInvulnerable) return;

        vidaActual = Mathf.Max(0, vidaActual - cantidad);
        OnHealthChanged?.Invoke(vidaActual, vidaMaxima);
        OnDamaged?.Invoke();

        Debug.Log($"[Diego] Recibió daño. Vida: {vidaActual}/{vidaMaxima}");

        if (vidaActual <= 0)
        {
            Morir();
            return;
        }

        Vector2 dir = ((Vector2)transform.position - origenDanio).normalized;
        if (dir.sqrMagnitude < 0.01f) dir = Vector2.up;
        rb.linearVelocity = new Vector2(dir.x * fuerzaKnockback, fuerzaKnockback * 0.5f);

        StartCoroutine(RutinaInvulnerabilidad());
    }

    private System.Collections.IEnumerator RutinaInvulnerabilidad()
    {
        EsInvulnerable = true;
        float t = 0f;
        bool flash = false;
        Color colorOriginal = spriteRenderer != null ? spriteRenderer.color : Color.white;

        while (t < tiempoInvulnerabilidad)
        {
            if (spriteRenderer != null)
            {
                flash = !flash;
                spriteRenderer.color = flash ? colorDanio : colorOriginal;
            }
            yield return new WaitForSeconds(parpadeoCadaSegundos);
            t += parpadeoCadaSegundos;
        }

        if (spriteRenderer != null) spriteRenderer.color = colorOriginal;
        EsInvulnerable = false;
    }

    private void Morir()
    {
        EstaMuerto = true;
        if (controller != null) controller.PuedeActuar = false;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("[Diego] Murió.");
        OnDeath?.Invoke();
    }

    public void Curar(int cantidad)
    {
        if (EstaMuerto) return;
        vidaActual = Mathf.Min(vidaMaxima, vidaActual + cantidad);
        OnHealthChanged?.Invoke(vidaActual, vidaMaxima);
    }
}
