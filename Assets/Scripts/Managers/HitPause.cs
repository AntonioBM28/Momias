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
        float timeScaleOriginal = Time.timeScale;
        Time.timeScale = timeScaleDuranteFreeze;

        float t = 0f;
        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = timeScaleOriginal;
        rutinaActiva = null;
    }
}
