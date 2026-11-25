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
    private Animator animator;
    private Rigidbody2D rb;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
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
        if (AudioManager.Instance != null) AudioManager.Instance.PlayDanoDiego();

        // Game feel: shake + hitpause al recibir dano
        if (CameraShaker.Instance != null) CameraShaker.Instance.Shake(0.12f, 0.15f);
        if (HitPause.Instance != null) HitPause.Instance.Freeze(0.03f);

        Debug.Log($"[Diego] Recibió daño. Vida: {vidaActual}/{vidaMaxima}");
        // Persistir vida en SaveSystem
        if (!EstaMuerto)
        {
            int nivel = SaveSystem.NivelDeEscena(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            if (nivel > 0) SaveSystem.GuardarPartida(nivel, vidaActual);
        }

        if (vidaActual <= 0)
        {
            Morir();
            return;
        }

        if (animator != null) animator.SetTrigger("Damage");

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
        if (animator != null) animator.SetBool("IsDead", true);
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
