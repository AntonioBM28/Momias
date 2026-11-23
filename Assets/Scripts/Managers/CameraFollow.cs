using UnityEngine;

/// <summary>
/// La camara sigue a Diego horizontalmente dentro de una zona activa limitada.
/// Afuera de esa zona (bordes del nivel), la camara queda clamped para no
/// mostrar el "vacio" mas alla del background.
/// Smooth lerp para que no se sienta pegada al pixel.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform objetivo;
    [SerializeField] private string objetivoTag = "Player";

    [Header("Zona activa (bordes del nivel)")]
    [SerializeField] private float minX = -4f;
    [SerializeField] private float maxX = 4f;

    [Header("Suavizado")]
    [SerializeField] private float smoothFactor = 5f;
    [SerializeField] private float offsetY = 0f;

    private float fixedZ;

    void Start()
    {
        fixedZ = transform.position.z;
        if (objetivo == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag(objetivoTag);
            if (go != null) objetivo = go.transform;
        }
    }

    void LateUpdate()
    {
        if (objetivo == null) return;
        float targetX = Mathf.Clamp(objetivo.position.x, minX, maxX);
        Vector3 goal = new Vector3(targetX, transform.position.y + offsetY, fixedZ);
        transform.position = Vector3.Lerp(transform.position, goal, smoothFactor * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 a = new Vector3(minX, transform.position.y - 3f, 0);
        Vector3 b = new Vector3(minX, transform.position.y + 3f, 0);
        Vector3 c = new Vector3(maxX, transform.position.y - 3f, 0);
        Vector3 d = new Vector3(maxX, transform.position.y + 3f, 0);
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(c, d);
    }
}
