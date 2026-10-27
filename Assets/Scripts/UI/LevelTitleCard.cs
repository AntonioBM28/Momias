using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Cartel que aparece al cargar un nivel con el nombre del nivel.
/// Fade in -> permanece X segundos -> fade out. Puro UI, sin bloquear gameplay.
/// </summary>
public class LevelTitleCard : MonoBehaviour
{
    [Header("Contenido")]
    [SerializeField] private string tituloNivel = "NIVEL 1";
    [SerializeField] private string subtituloNivel = "MUSEO DE LAS MOMIAS";

    [Header("Referencias UI")]
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TMP_Text tituloText;
    [SerializeField] private TMP_Text subtituloText;

    [Header("Timings")]
    [SerializeField] private float delayInicial = 0.3f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float holdDuration = 1.5f;
    [SerializeField] private float fadeOutDuration = 0.6f;

    void Start()
    {
        if (tituloText != null) tituloText.text = tituloNivel;
        if (subtituloText != null) subtituloText.text = subtituloNivel;
        if (group != null) group.alpha = 0f;

        StartCoroutine(MostrarCartel());
    }

    private IEnumerator MostrarCartel()
    {
        yield return new WaitForSeconds(delayInicial);

        // Fade in
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            if (group != null) group.alpha = Mathf.Clamp01(t / fadeInDuration);
            yield return null;
        }
        if (group != null) group.alpha = 1f;

        yield return new WaitForSeconds(holdDuration);

        // Fade out
        t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            if (group != null) group.alpha = 1f - Mathf.Clamp01(t / fadeOutDuration);
            yield return null;
        }
        if (group != null) group.alpha = 0f;

        gameObject.SetActive(false);
    }
}
