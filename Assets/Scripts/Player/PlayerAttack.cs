using UnityEngine;

/// <summary>
/// Ataque direccional de Diego: campanazo en la direccion a la que mira.
/// Añade un pulso visual (escala rapida) como feedback mientras no haya
/// animacion de ataque formal en el Animator.
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Configuracion de ataque")]
    [SerializeField] private int danio = 1;
    [SerializeField] private float cooldown = 0.35f;
    [SerializeField] private float duracionHitbox = 0.15f;

    [Header("Hitbox")]
    [SerializeField] private GameObject hitboxPrefab;
    [SerializeField] private Transform puntoAtaque;
    [SerializeField] private float distanciaAtaque = 1.2f;
    [SerializeField] private Vector2 tamanioHitbox = new Vector2(1.8f, 1.2f);

    [Header("Modificaciones al agacharse")]
    [SerializeField] private float yOffsetAgachado = -0.4f;
    [SerializeField] private Vector2 tamanioHitboxAgachado = new Vector2(1.8f, 0.7f);

    [Header("Feedback visual")]
    [SerializeField] private float pulsoEscala = 1.15f;
    [SerializeField] private float pulsoDuracion = 0.1f;
    [SerializeField] private SpriteRenderer flashSprite;
    [SerializeField] private Color colorFlash = new Color(1f, 1f, 0.6f, 1f);

    [Header("Visual / audio")]
    [SerializeField] private AudioClip sfxCampanazo;

    private PlayerController controller;
    private Animator anim;
    private AudioSource audioSource;
    private float ultimoAtaque = -999f;
    private bool pulsando;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (flashSprite == null) flashSprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!controller.PuedeActuar) return;

        bool quiereAtacar = Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0);
        if (quiereAtacar && Time.time - ultimoAtaque >= cooldown)
        {
            Atacar();
            ultimoAtaque = Time.time;
        }
    }

    private void Atacar()
    {
        if (anim != null) anim.SetTrigger("Attack");
        if (audioSource != null && sfxCampanazo != null) audioSource.PlayOneShot(sfxCampanazo);

        if (hitboxPrefab == null || puntoAtaque == null)
        {
            Debug.LogWarning("[PlayerAttack] Falta hitboxPrefab o puntoAtaque en el inspector.");
            return;
        }

        float dirX = controller.IsFacingRight ? 1f : -1f;
        Vector3 pos = puntoAtaque.position;
        pos.x = transform.position.x + distanciaAtaque * dirX;
        if (controller.IsCrouching) pos.y += yOffsetAgachado;

        Vector2 tam = controller.IsCrouching ? tamanioHitboxAgachado : tamanioHitbox;

        GameObject hb = Instantiate(hitboxPrefab, pos, Quaternion.identity);
        AttackHitbox hitbox = hb.GetComponent<AttackHitbox>();
        if (hitbox != null) hitbox.Configurar(danio, tam, duracionHitbox);

        if (!pulsando) StartCoroutine(PulsoVisual());
    }

    private System.Collections.IEnumerator PulsoVisual()
    {
        pulsando = true;
        Color original = flashSprite != null ? flashSprite.color : Color.white;
        if (flashSprite != null) flashSprite.color = colorFlash;

        yield return new WaitForSeconds(pulsoDuracion);

        if (flashSprite != null) flashSprite.color = original;
        pulsando = false;
    }
}
