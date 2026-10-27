using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Componente que anima el alpha de un Image negro de pantalla completa
/// para hacer fades. Usado por SceneTransition. Se puede llamar directamente
/// con FadeTo(alpha, duration) para fades manuales (ej. a dorado en Victoria).
/// </summary>
[RequireComponent(typeof(Image))]
public class Fader : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private float defaultDuration = 0.5f;

    private Image img;

    public bool IsFading { get; private set; }

    void Awake()
    {
        img = GetComponent<Image>();
        // Arranca invisible
        SetAlpha(0f);
    }

    public void SetAlpha(float a)
    {
        Color c = img.color;
        c.a = Mathf.Clamp01(a);
        img.color = c;
    }

    public void SetColor(Color c)
    {
        img.color = c;
    }

    public IEnumerator FadeIn(float duration = -1f)
    {
        return Fade(1f, 0f, duration > 0 ? duration : defaultDuration);
    }

    public IEnumerator FadeOut(float duration = -1f)
    {
        return Fade(0f, 1f, duration > 0 ? duration : defaultDuration);
    }

    public IEnumerator FadeTo(float targetAlpha, float duration)
    {
        return Fade(img.color.a, targetAlpha, duration);
    }

    public IEnumerator FadeToColor(Color targetColor, float duration)
    {
        IsFading = true;
        Color start = img.color;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            img.color = Color.Lerp(start, targetColor, Mathf.Clamp01(t / duration));
            yield return null;
        }
        img.color = targetColor;
        IsFading = false;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        IsFading = true;
        float t = 0f;
        SetAlpha(from);
        while (t < duration)
        {
            // unscaledDeltaTime para que funcione en pausa
            t += Time.unscaledDeltaTime;
            SetAlpha(Mathf.Lerp(from, to, Mathf.Clamp01(t / duration)));
            yield return null;
        }
        SetAlpha(to);
        IsFading = false;
    }
}
