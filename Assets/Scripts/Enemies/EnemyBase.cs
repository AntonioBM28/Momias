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
        rb.linearVelocity = Vector2.zero;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (anim != null) anim.SetTrigger("Morir");
        Destroy(gameObject, 1f);
    }
}
