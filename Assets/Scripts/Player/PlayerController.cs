using UnityEngine;

/// <summary>
/// Controlador de Diego con escala base ajustable (para que se vea mas grande).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float velocidadAgachado = 1.5f;
    [SerializeField] private float fuerzaSalto = 11f;

    [Header("Deteccion de suelo")]
    [SerializeField] private Transform checkSuelo;
    [SerializeField] private float radioSuelo = 0.15f;
    [SerializeField] private LayerMask capaSuelo;

    [Header("Collider agachado")]
    [SerializeField] private CapsuleCollider2D colliderPrincipal;
    [SerializeField] private Vector2 tamanioParado = new Vector2(0.8f, 1.6f);
    [SerializeField] private Vector2 offsetParado = new Vector2(0f, 0f);
    [SerializeField] private Vector2 tamanioAgachado = new Vector2(0.8f, 0.9f);
    [SerializeField] private Vector2 offsetAgachado = new Vector2(0f, -0.35f);

    [Header("Visual")]
    [Tooltip("Tamanio base de Diego en pantalla. 1=normal, 1.5=mas grande, 2=mucho mas grande.")]
    [SerializeField] private float escalaBase = 1.5f;
    [SerializeField] private float escalaYAgachado = 0.65f;

    private Rigidbody2D rb;
    private Animator anim;
    private float movimientoX;

    public bool IsGrounded { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool IsFacingRight { get; private set; } = true;
    public bool PuedeActuar { get; set; } = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!PuedeActuar) { movimientoX = 0; AplicarScaleVisual(); return; }

        movimientoX = Input.GetAxisRaw("Horizontal");

        bool quiereAgacharse = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        SetAgachado(quiereAgacharse);

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded && !IsCrouching)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);

        if (movimientoX > 0.01f) IsFacingRight = true;
        else if (movimientoX < -0.01f) IsFacingRight = false;

        AplicarScaleVisual();

        anim.SetFloat("velocidadAbsoluta", Mathf.Abs(movimientoX));
        anim.SetBool("isGrounded", IsGrounded);
        anim.SetBool("isCrouching", IsCrouching);
    }

    void FixedUpdate()
    {
        if (checkSuelo != null)
            IsGrounded = Physics2D.OverlapCircle(checkSuelo.position, radioSuelo, capaSuelo);

        float vel = IsCrouching ? velocidadAgachado : velocidad;
        rb.linearVelocity = new Vector2(movimientoX * vel, rb.linearVelocity.y);
    }

    private void AplicarScaleVisual()
    {
        float sx = (IsFacingRight ? 1f : -1f) * escalaBase;
        float sy = (IsCrouching ? escalaYAgachado : 1f) * escalaBase;
        transform.localScale = new Vector3(sx, sy, 1f);
    }

    private void SetAgachado(bool agachado)
    {
        if (agachado == IsCrouching) return;
        IsCrouching = agachado;
        if (colliderPrincipal != null)
        {
            colliderPrincipal.size = agachado ? tamanioAgachado : tamanioParado;
            colliderPrincipal.offset = agachado ? offsetAgachado : offsetParado;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (checkSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkSuelo.position, radioSuelo);
        }
    }
}
