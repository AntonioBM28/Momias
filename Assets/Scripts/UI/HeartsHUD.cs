using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Muestra la vida de Diego como corazones llenos, medios o vacíos en el HUD.
/// </summary>
public class HeartsHUD : MonoBehaviour
{
    [Header("Sprites del HUD")]
    [SerializeField] private Sprite corazonLleno;
    [SerializeField] private Sprite corazonMedio;
    [SerializeField] private Sprite corazonVacio;

    [Header("Diseño")]
    [SerializeField] private Vector2 margen = new Vector2(24f, 24f);
    [SerializeField] private float tamanoCorazon = 40f;
    [SerializeField] private float separacion = 4f;

    private PlayerHealth salud;
    private Image[] iconos;

    private void Awake()
    {
        salud = GetComponent<PlayerHealth>();
        CrearHUD();
    }

    private void OnEnable()
    {
        if (salud == null) salud = GetComponent<PlayerHealth>();
        if (salud != null) salud.OnHealthChanged.AddListener(ActualizarCorazones);
    }

    private void Start()
    {
        if (salud != null) ActualizarCorazones(salud.VidaActual, salud.VidaMaxima);
    }

    private void OnDisable()
    {
        if (salud != null) salud.OnHealthChanged.RemoveListener(ActualizarCorazones);
    }

    private void CrearHUD()
    {
        if (corazonLleno == null || corazonMedio == null || corazonVacio == null)
        {
            Debug.LogError("HeartsHUD: faltan asignar uno o más sprites de corazón.", this);
            return;
        }

        int cantidad = salud != null ? Mathf.CeilToInt(salud.VidaMaxima / 2f) : 6;
        float anchoCorazones = cantidad * tamanoCorazon + Mathf.Max(0, cantidad - 1) * separacion;

        GameObject canvasObject = new GameObject("HUD Vida", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        canvasRect.anchorMin = Vector2.zero;
        canvasRect.anchorMax = Vector2.one;
        canvasRect.offsetMin = Vector2.zero;
        canvasRect.offsetMax = Vector2.zero;

        GameObject panelObject = new GameObject("Panel Corazones", typeof(RectTransform), typeof(Image));
        RectTransform panel = panelObject.GetComponent<RectTransform>();
        panel.SetParent(canvasRect, false);
        panel.anchorMin = new Vector2(0f, 1f);
        panel.anchorMax = new Vector2(0f, 1f);
        panel.pivot = new Vector2(0f, 1f);
        panel.anchoredPosition = new Vector2(margen.x, -margen.y);
        panel.sizeDelta = new Vector2(anchoCorazones + 16f, tamanoCorazon + 16f);

        Image fondo = panelObject.GetComponent<Image>();
        fondo.color = new Color(0f, 0f, 0f, 0.45f);
        fondo.raycastTarget = false;

        iconos = new Image[cantidad];
        for (int i = 0; i < cantidad; i++)
        {
            GameObject iconoObject = new GameObject($"Corazon {i + 1}", typeof(RectTransform), typeof(Image));
            RectTransform rect = iconoObject.GetComponent<RectTransform>();
            rect.SetParent(panel, false);
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(tamanoCorazon, tamanoCorazon);
            rect.anchoredPosition = new Vector2(8f + tamanoCorazon * 0.5f + i * (tamanoCorazon + separacion), -8f - tamanoCorazon * 0.5f);

            Image imagen = iconoObject.GetComponent<Image>();
            imagen.preserveAspect = true;
            imagen.raycastTarget = false;
            iconos[i] = imagen;
        }
    }

    private void ActualizarCorazones(int vidaActual, int vidaMaxima)
    {
        if (iconos == null) return;

        for (int i = 0; i < iconos.Length; i++)
        {
            int unidadesEnEsteCorazon = vidaActual - i * 2;
            iconos[i].sprite = unidadesEnEsteCorazon >= 2
                ? corazonLleno
                : unidadesEnEsteCorazon == 1 ? corazonMedio : corazonVacio;
        }
    }
}
