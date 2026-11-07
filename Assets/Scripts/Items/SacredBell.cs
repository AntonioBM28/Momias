using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Permite que Diego recoja la Campana Sagrada al acercarse y pulsar E.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class SacredBell : MonoBehaviour
{
    [Header("Interaccion")]
    [SerializeField] private string textoInteraccion = "E - Recoger Campana Sagrada";
    public UnityEvent OnPickedUp;

    private bool jugadorCerca;
    private bool recogida;

    private void Awake()
    {
        CircleCollider2D trigger = GetComponent<CircleCollider2D>();
        trigger.isTrigger = true;
        trigger.radius = 1.15f;
    }

    private void Update()
    {
        if (!jugadorCerca || recogida || !Input.GetKeyDown(KeyCode.E)) return;

        recogida = true;
        Debug.Log("[CampanaSagrada] Campana recogida. Nivel 1 completado.");
        OnPickedUp?.Invoke();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerController controller = player != null ? player.GetComponent<PlayerController>() : null;
        if (controller != null) controller.PuedeActuar = false;

        VictoryCinematic cinematic = FindFirstObjectByType<VictoryCinematic>();
        if (cinematic != null)
            cinematic.PlayCinematic(transform.position);
        else if (GameManager.Instance != null)
            GameManager.Instance.TriggerVictory();

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) jugadorCerca = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) jugadorCerca = false;
    }

    private void OnGUI()
    {
        if (!jugadorCerca || recogida || Camera.main == null) return;

        Vector3 punto = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 0.9f);
        if (punto.z <= 0f) return;

        Rect etiqueta = new Rect(punto.x - 105f, Screen.height - punto.y - 18f, 210f, 36f);
        GUI.Box(etiqueta, textoInteraccion);
    }
}
