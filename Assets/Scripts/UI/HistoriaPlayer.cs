using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Muestra una historia en diapositivas (escena "Historia").
/// - Imagen a pantalla completa con fundido entre diapositivas.
/// - Texto abajo, letra por letra.
/// - Avanzar: tocar/clic en cualquier parte, Espacio o Enter.
///   Si el texto se esta escribiendo, el primer toque lo completa.
/// - Saltar: boton "SALTAR" en pantalla o Esc.
/// Avanzar() y Saltar() son publicos para conectarlos a otros controles (movil).
/// </summary>
public class HistoriaPlayer : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image imagen;
    [SerializeField] private AspectRatioFitter ajusteImagen;
    [SerializeField] private CanvasGroup grupoImagen;
    [SerializeField] private TMP_Text texto;
    [SerializeField] private GameObject indicadorContinuar;
    [SerializeField] private Button botonSaltar;

    [Header("Ritmo")]
    [SerializeField] private float letrasPorSegundo = 45f;
    [SerializeField] private float duracionFundido = 0.35f;

    [Header("Pruebas")]
    [Tooltip("Historia que se muestra si se da Play directamente en esta escena.")]
    [SerializeField] private HistoriaData historiaDePrueba;

    private HistoriaData data;
    private int indice = -1;
    private bool escribiendo;
    private bool cambiando;
    private bool terminado;
    private Coroutine rutinaTexto;

    void Start()
    {
        data = Historia.Pendiente != null ? Historia.Pendiente : historiaDePrueba;
        if (data == null || data.diapositivas.Count == 0)
        {
            Terminar();
            return;
        }

        if (botonSaltar != null) botonSaltar.onClick.AddListener(Saltar);

        if (AudioManager.Instance != null)
        {
            AudioClip musica = data.musica != null ? data.musica : AudioManager.Instance.musicMenu;
            if (musica != null) AudioManager.Instance.PlayMusic(musica, true);
        }

        if (grupoImagen != null) grupoImagen.alpha = 0f;
        StartCoroutine(MostrarDiapositiva(0));
    }

    void Update()
    {
        if (terminado) return;

        if (Input.GetKeyDown(KeyCode.Escape)) { Saltar(); return; }

        bool avanzar = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)
                       || Input.GetKeyDown(KeyCode.KeypadEnter);

        // Toque o clic en cualquier parte que no sea un boton (p. ej. "SALTAR")
        if (Input.GetMouseButtonDown(0) && !PunteroSobreUI()) avanzar = true;

        if (avanzar) Avanzar();
    }

    /// <summary>Completa el texto si se esta escribiendo; si no, pasa a la siguiente diapositiva.</summary>
    public void Avanzar()
    {
        if (terminado || cambiando) return;

        if (escribiendo)
        {
            if (rutinaTexto != null) StopCoroutine(rutinaTexto);
            texto.maxVisibleCharacters = int.MaxValue;
            escribiendo = false;
            if (indicadorContinuar != null) indicadorContinuar.SetActive(true);
            return;
        }

        if (indice + 1 < data.diapositivas.Count) StartCoroutine(MostrarDiapositiva(indice + 1));
        else Terminar();
    }

    /// <summary>Salta toda la historia y va a la escena siguiente.</summary>
    public void Saltar()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUIClick();
        Terminar();
    }

    private void Terminar()
    {
        if (terminado) return;
        terminado = true;
        Historia.Terminar();
    }

    private IEnumerator MostrarDiapositiva(int nuevoIndice)
    {
        cambiando = true;
        if (indicadorContinuar != null) indicadorContinuar.SetActive(false);

        // Fundido de salida (solo si ya habia una diapositiva)
        if (indice >= 0) yield return Fundido(1f, 0f);

        indice = nuevoIndice;
        HistoriaData.Diapositiva d = data.diapositivas[indice];

        imagen.sprite = d.imagen;
        imagen.color = d.imagen != null ? Color.white : Color.clear;
        if (ajusteImagen != null && d.imagen != null)
            ajusteImagen.aspectRatio = d.imagen.rect.width / d.imagen.rect.height;

        texto.text = d.texto;
        texto.maxVisibleCharacters = 0;

        yield return Fundido(0f, 1f);
        cambiando = false;

        rutinaTexto = StartCoroutine(EscribirTexto());
    }

    private IEnumerator EscribirTexto()
    {
        escribiendo = true;
        texto.ForceMeshUpdate();
        int total = texto.textInfo.characterCount;
        float visibles = 0f;
        while (visibles < total)
        {
            visibles += letrasPorSegundo * Time.unscaledDeltaTime;
            texto.maxVisibleCharacters = Mathf.Min(total, Mathf.FloorToInt(visibles));
            yield return null;
        }
        texto.maxVisibleCharacters = int.MaxValue;
        escribiendo = false;
        if (indicadorContinuar != null) indicadorContinuar.SetActive(true);
    }

    private IEnumerator Fundido(float desde, float hasta)
    {
        if (grupoImagen == null) yield break;
        float t = 0f;
        while (t < duracionFundido)
        {
            t += Time.unscaledDeltaTime;
            grupoImagen.alpha = Mathf.Lerp(desde, hasta, t / duracionFundido);
            yield return null;
        }
        grupoImagen.alpha = hasta;
    }

    private static bool PunteroSobreUI()
    {
        if (EventSystem.current == null) return false;
        if (Input.touchCount > 0) return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        return EventSystem.current.IsPointerOverGameObject();
    }
}
