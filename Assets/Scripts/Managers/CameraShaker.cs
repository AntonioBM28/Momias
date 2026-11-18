using UnityEngine;

/// <summary>
/// Shake de camara al recibir daño o matar enemigos.
/// Singleton persistente: busca Camera.main dinamicamente para cada shake,
/// asi sobrevive al cambio de escenas y siempre sacude la camara activa.
/// </summary>
public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance { get; private set; }

    private Camera targetCam;
    private Vector3 posBase;
    private float tiempoRestante;
    private float magnitud;
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

    public void Shake(float magnitud, float duracion)
    {
        if (duracion <= 0f) return;
        targetCam = Camera.main;
        if (targetCam == null) return;

        // Si ya estaba shakeando, guarda la posicion base original (no la movida)
        if (rutinaActiva != null)
        {
            StopCoroutine(rutinaActiva);
            targetCam.transform.position = posBase;
        }
        posBase = targetCam.transform.position;
        this.magnitud = magnitud;
        this.tiempoRestante = duracion;
        rutinaActiva = StartCoroutine(Sacudir(duracion));
    }

    private System.Collections.IEnumerator Sacudir(float duracion)
    {
        float t = 0f;
        while (t < duracion && targetCam != null)
        {
            // Atenua el shake hacia el final para que no corte bruscamente
            float fuerza = magnitud * (1f - (t / duracion));
            Vector2 offset = Random.insideUnitCircle * fuerza;
            targetCam.transform.position = posBase + new Vector3(offset.x, offset.y, 0f);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        if (targetCam != null) targetCam.transform.position = posBase;
        rutinaActiva = null;
    }
}
