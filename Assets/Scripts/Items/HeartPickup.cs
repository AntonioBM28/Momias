using UnityEngine;

/// <summary>
/// Corazón recolectable que spawnea cuando muere una momia (prob configurable).
/// Hace bob vertical + rotacion pingpong. Al tocarlo Diego recupera 2 unidades
/// (1 corazon completo). Si Diego ya esta en vida maxima, no lo recoge.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class HeartPickup : MonoBehaviour
{
    [Header("Animacion idle")]
    [SerializeField] private float amplitudBob = 0.15f;
    [SerializeField] private float frecuenciaBob = 2.5f;
    [SerializeField] private float rotPingPongGrados = 10f;
    [SerializeField] private float frecuenciaRot = 1.0f;

    [Header("Curacion")]
    [SerializeField] private int cantidadCuracion = 2;

    private Vector3 posInicial;
    private Quaternion rotInicial;
    private float tiempoNacimiento;
    private bool recogido;

    void Start()
    {
        posInicial = transform.position;
        rotInicial = transform.rotation;
        tiempoNacimiento = Time.time;
    }

    void Update()
    {
        if (recogido) return;
        float t = Time.time - tiempoNacimiento;
        float offsetY = Mathf.Sin(t * frecuenciaBob * Mathf.PI * 2f) * amplitudBob;
        transform.position = new Vector3(posInicial.x, posInicial.y + offsetY, posInicial.z);

        float rotZ = Mathf.Sin(t * frecuenciaRot * Mathf.PI * 2f) * rotPingPongGrados;
        transform.rotation = rotInicial * Quaternion.Euler(0f, 0f, rotZ);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (recogido) return;
        if (!other.CompareTag("Player")) return;

        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph == null) return;
        if (ph.EstaMuerto) return;
        if (ph.VidaActual >= ph.VidaMaxima) return; // vida llena, no recoge

        ph.Curar(cantidadCuracion);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayPickupCorazon();

        recogido = true;
        Destroy(gameObject);
    }

    /// <summary>Spawnea un HeartPickup con probabilidad dada al morir un enemigo.</summary>
    public static void TrySpawnOnEnemyDeath(GameObject prefab, Vector3 posicion, float probabilidad)
    {
        if (prefab == null) return;
        if (Random.value < probabilidad)
        {
            // Spawn ligeramente arriba para que no quede bajo el suelo
            Vector3 spawnPos = posicion + new Vector3(0f, 0.5f, 0f);
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }
}
