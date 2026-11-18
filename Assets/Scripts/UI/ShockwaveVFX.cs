using UnityEngine;

/// <summary>
/// Onda sonica dorada que se expande + desvanece al ejecutar el campanazo.
/// Hace sprite-swap en Update para animar los 4 frames (placeholder hasta
/// que Antonio entregue una version mejor en Avance 4).
/// Autodestruye al acabar la animacion.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class ShockwaveVFX : MonoBehaviour
{
    [Header("Frames")]
    public Sprite[] sprites;
    [SerializeField] private float framesPorSegundo = 14f;

    [Header("Animacion")]
    [SerializeField] private float duracion = 0.3f;
    [SerializeField] private float escalaInicial = 0.4f;
    [SerializeField] private float escalaFinal = 2.5f;
    [SerializeField]
    private AnimationCurve curvaEscala = new AnimationCurve(
        new Keyframe(0f, 0f, 2f, 2f),
        new Keyframe(1f, 1f, 0.2f, 0.2f)
    );

    private SpriteRenderer sr;
    private Color colorInicial;
    private float tiempoInicio;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        colorInicial = sr.color;
        tiempoInicio = Time.time;
        transform.localScale = Vector3.one * escalaInicial;
    }

    void Update()
    {
        float elapsed = Time.time - tiempoInicio;
        float t = elapsed / duracion;
        if (t >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        float eased = curvaEscala.Evaluate(t);
        float escala = Mathf.Lerp(escalaInicial, escalaFinal, eased);
        transform.localScale = new Vector3(escala, escala, 1f);

        Color c = colorInicial;
        c.a = Mathf.Lerp(colorInicial.a, 0f, t);
        sr.color = c;

        if (sprites != null && sprites.Length > 0)
        {
            int idx = Mathf.Min(sprites.Length - 1, Mathf.FloorToInt(elapsed * framesPorSegundo));
            sr.sprite = sprites[idx];
        }
    }

    /// <summary>Spawnea una onda en la posicion dada, orientada al sortingOrder indicado.</summary>
    public static ShockwaveVFX Spawn(Sprite[] sprites, Vector3 posicion, Color tinte, int sortingOrder = 15)
    {
        if (sprites == null || sprites.Length == 0) return null;
        GameObject go = new GameObject("ShockwaveVFX");
        go.transform.position = posicion;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprites[0];
        sr.color = tinte;
        sr.sortingOrder = sortingOrder;
        ShockwaveVFX vfx = go.AddComponent<ShockwaveVFX>();
        vfx.sprites = sprites;
        return vfx;
    }
}
