using UnityEngine;

/// <summary>
/// Freeze-frame breve al matar enemigos (hit-pause). Baja Time.timeScale
/// a casi 0 por N ms para darle peso al golpe. Singleton persistente.
/// Usa unscaledDeltaTime para medir, asi no se afecta a si mismo.
/// </summary>
public class HitPause : MonoBehaviour
{
    public static HitPause Instance { get; private set; }

    [SerializeField] private float defaultDuracion = 0.06f; // 60ms
    [SerializeField] private float timeScaleDuranteFreeze = 0.05f;

    private Coroutine rutinaActiva;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Freeze(float duracion = -1f)
    {
        if (duracion <= 0f) duracion = defaultDuracion;

        if (rutinaActiva != null) StopCoroutine(rutinaActiva);
        rutinaActiva = StartCoroutine(RutinaFreeze(duracion));
    }

    private System.Collections.IEnumerator RutinaFreeze(float duracion)
    {
        // IMPORTANTE: NO guardar Time.timeScale actual porque si ya habia
        // un freeze corriendo, estariamos capturando 0.05 y dejando el juego en camara lenta.
        // Siempre restauramos a 1 (o a lo que haya en el GameManager si es Pausa).
        Time.timeScale = timeScaleDuranteFreeze;

        float t = 0f;
        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        // Restaurar: si el GameManager esta en Paused, mantener 0; si no, 1
        if (GameManager.Instance != null && GameManager.Instance.IsPaused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;

        rutinaActiva = null;
    }
}
