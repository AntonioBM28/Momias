using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Anima el fondo del menu con efecto Ken Burns:
/// - Zoom lento 1.00 -> 1.08 PingPong
/// - Paneo horizontal con Sin wave
/// Para que no se vean bordes, el sprite original debe ser mas grande
/// que el Canvas (nosotros generamos 2200x1200 para 1920x1080).
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class MenuBackground : MonoBehaviour
{
    [Header("Zoom")]
    [SerializeField] private float escalaMin = 1.00f;
    [SerializeField] private float escalaMax = 1.08f;
    [SerializeField] private float periodoZoom = 14f;

    [Header("Paneo horizontal")]
    [SerializeField] private float amplitudPaneo = 60f;
    [SerializeField] private float periodoPaneo = 18f;

    private RectTransform rt;
    private Vector2 posInicial;
    private float tiempo;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        posInicial = rt.anchoredPosition;
    }

    void Update()
    {
        tiempo += Time.unscaledDeltaTime;

        // Zoom PingPong suave con Sin wave
        float tZoom = (Mathf.Sin(tiempo * (2f * Mathf.PI / periodoZoom)) + 1f) * 0.5f;
        float escala = Mathf.Lerp(escalaMin, escalaMax, tZoom);
        rt.localScale = new Vector3(escala, escala, 1f);

        // Paneo horizontal con Sin
        float offsetX = Mathf.Sin(tiempo * (2f * Mathf.PI / periodoPaneo)) * amplitudPaneo;
        rt.anchoredPosition = new Vector2(posInicial.x + offsetX, posInicial.y);
    }
}
