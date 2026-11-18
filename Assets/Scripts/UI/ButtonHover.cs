using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Componente para agregar un hover effect a cualquier Button:
/// escala suave 1.0 -> 1.1 al pasar el mouse por encima, vuelve a 1.0 al salir.
/// Por ahora sin sonido (Antonio pendiente de entregar sfx hover).
/// </summary>
public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float escalaHover = 1.1f;
    [SerializeField] private float velocidad = 10f;

    private Vector3 escalaBase;
    private Vector3 escalaObjetivo;

    void Awake()
    {
        escalaBase = transform.localScale;
        escalaObjetivo = escalaBase;
    }

    void OnEnable()
    {
        transform.localScale = escalaBase;
        escalaObjetivo = escalaBase;
    }

    void Update()
    {
        // Lerp suave hacia la escala objetivo con unscaledDeltaTime (funciona en pausa)
        transform.localScale = Vector3.Lerp(transform.localScale, escalaObjetivo, velocidad * Time.unscaledDeltaTime);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        escalaObjetivo = escalaBase * escalaHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        escalaObjetivo = escalaBase;
    }
}
