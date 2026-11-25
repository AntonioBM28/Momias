using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Barra de vida del boss estilo Hollow Knight: barra horizontal abajo
/// con nombre encima. Entra con fade-in + slide. Tiene barra "blanca"
/// delayed que muestra el daño que acaba de recibir (game feel).
/// </summary>
public class BossHealthBar : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CanvasGroup group;
    [SerializeField] private RectTransform rootRect;
    [SerializeField] private Image barraVida;         // dorada
    [SerializeField] private Image barraDanoRecibido; // blanca delayed
    [SerializeField] private TMP_Text nombreText;

    [Header("Config")]
    [SerializeField] private float velocidadLerpDano = 2f;
    [SerializeField] private float fadeInDuration = 0.5f;

    private int vidaMaxima;
    private float fillActual;
    private float fillDelayed;
    private Vector2 posicionOculta;
    private Vector2 posicionVisible;
    private EnemyBase bossRef;  // referencia directa para polling robusto (fallback a eventos)

    void Awake()
    {
        if (group != null) group.alpha = 0f;
        if (rootRect != null)
        {
            posicionVisible = rootRect.anchoredPosition;
            posicionOculta = posicionVisible + Vector2.down * 100f;
            rootRect.anchoredPosition = posicionOculta;
        }
    }

    public void Inicializar(string nombre, int vidaMax)
    {
        if (nombreText != null) nombreText.text = nombre;
        vidaMaxima = vidaMax;
        fillActual = 1f;
        fillDelayed = 1f;
        if (barraVida != null) barraVida.fillAmount = 1f;
        if (barraDanoRecibido != null) barraDanoRecibido.fillAmount = 1f;
    }

    /// <summary>Guarda referencia al boss para hacer polling de VidaActual cada frame.</summary>
    public void SetBoss(EnemyBase boss)
    {
        bossRef = boss;
        Debug.Log($"[BossHealthBar] SetBoss -> {(boss!=null?boss.name:"NULL")}");
    }

    public void MostrarConFade()
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }

    public void OcultarConFade()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / fadeInDuration);
            if (group != null) group.alpha = p;
            if (rootRect != null) rootRect.anchoredPosition = Vector2.Lerp(posicionOculta, posicionVisible, p);
            yield return null;
        }
        if (group != null) group.alpha = 1f;
        if (rootRect != null) rootRect.anchoredPosition = posicionVisible;
    }

    private IEnumerator FadeOut()
    {
        float t = 0f;
        float startAlpha = group != null ? group.alpha : 1f;
        Vector2 startPos = rootRect != null ? rootRect.anchoredPosition : Vector2.zero;
        while (t < fadeInDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / fadeInDuration);
            if (group != null) group.alpha = Mathf.Lerp(startAlpha, 0f, p);
            if (rootRect != null) rootRect.anchoredPosition = Vector2.Lerp(startPos, posicionOculta, p);
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public void SetVida(int vidaActual)
    {
        if (vidaMaxima <= 0)
        {
            Debug.LogWarning("[BossHealthBar] vidaMaxima <= 0, usando 15 como fallback");
            vidaMaxima = 15;
        }
        float v = Mathf.Clamp01(vidaActual / (float)vidaMaxima);
        fillActual = v;
        if (barraVida != null) barraVida.fillAmount = fillActual;
        Debug.Log($"[BossHealthBar] SetVida -> {vidaActual}/{vidaMaxima} fill={fillActual:F2} barraVida={(barraVida!=null?"OK":"NULL")}");  // TMP debug
    }

    void Update()
    {
        // POLLING directo del boss (fallback robusto si los eventos UnityEvent fallan)
        if (bossRef != null && vidaMaxima > 0 && !bossRef.EstaMuerto)
        {
            float nuevaFill = Mathf.Clamp01(bossRef.VidaActual / (float)vidaMaxima);
            if (Mathf.Abs(nuevaFill - fillActual) > 0.001f)
            {
                fillActual = nuevaFill;
                if (barraVida != null) barraVida.fillAmount = fillActual;
            }
        }

        // La barra delayed (blanca) persigue a la barra vida real
        if (fillDelayed > fillActual)
        {
            fillDelayed = Mathf.MoveTowards(fillDelayed, fillActual, velocidadLerpDano * Time.deltaTime);
            if (barraDanoRecibido != null) barraDanoRecibido.fillAmount = fillDelayed;
        }
    }
}
