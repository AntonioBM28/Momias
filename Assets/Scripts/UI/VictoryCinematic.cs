using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Secuencia cinematografica cuando Diego toca la Campana Sagrada:
/// 1. Flash blanco + screen shake
/// 2. Slow motion + letterbox bars + camera zoom
/// 3. SFX campana con onda circular dorada
/// 4. Fade a dorado
/// 5. Activa el VictoryPanel con bounce
/// Todo con codigo, sin sprites extras.
/// </summary>
public class VictoryCinematic : MonoBehaviour
{
    [Header("Dependencias")]
    [SerializeField] private Image flashImage;             // Image blanco de pantalla completa
    [SerializeField] private Image goldFadeImage;          // Image dorado de pantalla completa
    [SerializeField] private RectTransform barraArriba;    // Barra negra arriba
    [SerializeField] private RectTransform barraAbajo;     // Barra negra abajo
    [SerializeField] private RectTransform ondaDorada;     // Image circular dorado (anillo)
    [SerializeField] private VictoryPanel victoryPanel;
    [SerializeField] private Camera gameCamera;

    [Header("Flash")]
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private float flashIntensity = 1f;

    [Header("Screen shake")]
    [SerializeField] private float shakeDuration = 0.25f;
    [SerializeField] private float shakeMagnitude = 0.15f;

    [Header("Slow motion")]
    [SerializeField] private float slowMoScale = 0.3f;
    [SerializeField] private float slowMoDuration = 1.3f;

    [Header("Letterbox")]
    [SerializeField] private float letterboxHeight = 100f;
    [SerializeField] private float letterboxDuration = 0.4f;

    [Header("Camera zoom")]
    [SerializeField] private float zoomFactor = 0.92f; // reduce ortho size 8%
    [SerializeField] private float zoomDuration = 1.3f;

    [Header("Onda dorada")]
    [SerializeField] private float ondaDuration = 1f;
    [SerializeField] private float ondaMaxScale = 10f;

    [Header("Fade a dorado")]
    [SerializeField] private float goldFadeDuration = 0.5f;

    private bool playing;

    void Awake()
    {
        if (gameCamera == null) gameCamera = Camera.main;
        // Estados iniciales invisibles
        if (flashImage != null) SetImageAlpha(flashImage, 0f);
        if (goldFadeImage != null) SetImageAlpha(goldFadeImage, 0f);
        if (ondaDorada != null)
        {
            ondaDorada.localScale = Vector3.zero;
            Image ondaImg = ondaDorada.GetComponent<Image>();
            if (ondaImg != null) SetImageAlpha(ondaImg, 0f);
        }
        if (barraArriba != null) barraArriba.sizeDelta = new Vector2(barraArriba.sizeDelta.x, 0);
        if (barraAbajo != null) barraAbajo.sizeDelta = new Vector2(barraAbajo.sizeDelta.x, 0);
    }

    /// <summary>Llamalo cuando Diego recoge la Campana Sagrada.</summary>
    public void PlayCinematic(Vector3 posicionCampana)
    {
        if (playing) return;
        playing = true;
        StartCoroutine(SecuenciaCompleta(posicionCampana));
    }

    private IEnumerator SecuenciaCompleta(Vector3 posCampana)
    {
        // Paso 1: Flash blanco + shake
        if (AudioManager.Instance != null) AudioManager.Instance.PlayCampanaSagrada();

        if (flashImage != null) StartCoroutine(FlashRoutine());
        if (gameCamera != null) StartCoroutine(ShakeRoutine());

        yield return new WaitForSecondsRealtime(0.15f);

        // Paso 2: Entrar en slow-mo + letterbox + zoom
        Time.timeScale = slowMoScale;

        if (barraArriba != null && barraAbajo != null)
            StartCoroutine(LetterboxRoutine(true));

        if (gameCamera != null)
            StartCoroutine(CameraZoomRoutine(true));

        yield return new WaitForSecondsRealtime(0.6f);

        // Paso 3: Onda dorada expansiva desde el centro
        if (ondaDorada != null)
            StartCoroutine(OndaDoradaRoutine());

        yield return new WaitForSecondsRealtime(slowMoDuration - 0.6f);

        // Paso 4: Fade a dorado
        Time.timeScale = 1f;
        if (goldFadeImage != null)
            yield return StartCoroutine(FadeToGold());

        // Paso 5: Mostrar panel de Victoria
        if (victoryPanel != null) victoryPanel.MostrarConBounce();

        // Resetear camara y letterbox al final (quedan invisibles bajo panel dorado)
        if (gameCamera != null) StartCoroutine(CameraZoomRoutine(false));
    }

    private IEnumerator FlashRoutine()
    {
        SetImageAlpha(flashImage, flashIntensity);
        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.unscaledDeltaTime;
            SetImageAlpha(flashImage, Mathf.Lerp(flashIntensity, 0f, t / flashDuration));
            yield return null;
        }
        SetImageAlpha(flashImage, 0f);
    }

    private IEnumerator ShakeRoutine()
    {
        Vector3 orig = gameCamera.transform.localPosition;
        float t = 0f;
        while (t < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            gameCamera.transform.localPosition = orig + new Vector3(x, y, 0);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        gameCamera.transform.localPosition = orig;
    }

    private IEnumerator LetterboxRoutine(bool mostrar)
    {
        float startH = barraArriba.sizeDelta.y;
        float endH = mostrar ? letterboxHeight : 0f;
        float t = 0f;
        while (t < letterboxDuration)
        {
            t += Time.unscaledDeltaTime;
            float h = Mathf.Lerp(startH, endH, t / letterboxDuration);
            barraArriba.sizeDelta = new Vector2(barraArriba.sizeDelta.x, h);
            barraAbajo.sizeDelta = new Vector2(barraAbajo.sizeDelta.x, h);
            yield return null;
        }
        barraArriba.sizeDelta = new Vector2(barraArriba.sizeDelta.x, endH);
        barraAbajo.sizeDelta = new Vector2(barraAbajo.sizeDelta.x, endH);
    }

    private IEnumerator CameraZoomRoutine(bool zoomIn)
    {
        float startOrtho = gameCamera.orthographicSize;
        float endOrtho = zoomIn ? startOrtho * zoomFactor : startOrtho / zoomFactor;
        float t = 0f;
        while (t < zoomDuration)
        {
            t += Time.unscaledDeltaTime;
            gameCamera.orthographicSize = Mathf.Lerp(startOrtho, endOrtho, t / zoomDuration);
            yield return null;
        }
        gameCamera.orthographicSize = endOrtho;
    }

    private IEnumerator OndaDoradaRoutine()
    {
        Image img = ondaDorada.GetComponent<Image>();
        float t = 0f;
        while (t < ondaDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = t / ondaDuration;
            ondaDorada.localScale = Vector3.one * Mathf.Lerp(0f, ondaMaxScale, p);
            if (img != null) SetImageAlpha(img, Mathf.Lerp(0.8f, 0f, p));
            yield return null;
        }
        ondaDorada.localScale = Vector3.zero;
        if (img != null) SetImageAlpha(img, 0f);
    }

    private IEnumerator FadeToGold()
    {
        float t = 0f;
        Color start = goldFadeImage.color; start.a = 0f;
        Color end = goldFadeImage.color; end.a = 1f;
        goldFadeImage.color = start;
        while (t < goldFadeDuration)
        {
            t += Time.unscaledDeltaTime;
            goldFadeImage.color = Color.Lerp(start, end, t / goldFadeDuration);
            yield return null;
        }
        goldFadeImage.color = end;
    }

    private void SetImageAlpha(Image i, float a)
    {
        Color c = i.color; c.a = a; i.color = c;
    }
}
