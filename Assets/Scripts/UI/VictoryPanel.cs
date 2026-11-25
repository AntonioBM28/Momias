using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel que aparece al final de VictoryCinematic. Entra con animacion
/// de bounce (scale 0 -> 1 con rebote). Boton "Siguiente Nivel" aparece
/// deshabilitado para Avance 2 (solo hay Nivel 1).
/// </summary>
public class VictoryPanel : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private Button siguienteNivelButton;

    [Header("Animacion")]
    [SerializeField] private float bounceDuration = 0.6f;
    [SerializeField] private AnimationCurve bounceCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    void Awake()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    void Start()
    {
        // El boton "Siguiente Nivel" siempre esta habilitado: en el ultimo nivel lleva a la historia final.
        if (siguienteNivelButton != null && GameManager.Instance != null)
        {
            siguienteNivelButton.interactable = true;
            if (string.IsNullOrEmpty(GameManager.Instance.GetNextLevelSceneName()))
            {
                var etiqueta = siguienteNivelButton.GetComponentInChildren<TMPro.TMP_Text>(true);
                if (etiqueta != null) etiqueta.text = "CONTINUAR";
            }
        }
    }

    public void MostrarConBounce()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.TriggerVictory();

        // Jingle corto: tocar como SFX (no loop) y parar la musica de fondo
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic(true);
            if (AudioManager.Instance.musicVictoria != null)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.musicVictoria);
        }

        if (victoryPanel != null) victoryPanel.SetActive(true);
        if (panelRect != null) StartCoroutine(BounceIn());
    }

    private IEnumerator BounceIn()
    {
        panelRect.localScale = Vector3.zero;
        float t = 0f;
        while (t < bounceDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / bounceDuration);
            // Bounce con sobre-escala a 1.15 y de regreso a 1.0
            float s;
            if (p < 0.7f)
                s = Mathf.Lerp(0f, 1.15f, p / 0.7f);
            else
                s = Mathf.Lerp(1.15f, 1f, (p - 0.7f) / 0.3f);
            panelRect.localScale = Vector3.one * s;
            yield return null;
        }
        panelRect.localScale = Vector3.one;
    }

    // --- Hooks botones ---

    public void OnSiguienteNivel()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUIClick();
        if (GameManager.Instance != null) GameManager.Instance.GoToNextLevel();
    }

    public void OnMenu()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUIClick();
        if (GameManager.Instance != null) GameManager.Instance.GoToMenu();
    }
}
