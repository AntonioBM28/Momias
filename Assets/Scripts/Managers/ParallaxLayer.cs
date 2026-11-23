using UnityEngine;

/// <summary>
/// Componente que se asigna a cada capa de background. Mueve la capa en X
/// proporcionalmente al movimiento de la camara, dando sensacion de profundidad.
///
/// factor 0.0 -> capa completamente estatica (fondo infinito)
/// factor 0.1 -> fondo muy lejano
/// factor 0.5 -> fondo medio
/// factor 1.0 -> capa pegada a la camara (primer plano)
/// </summary>
[RequireComponent(typeof(Transform))]
public class ParallaxLayer : MonoBehaviour
{
    [Range(0f, 1f)] [SerializeField] private float factor = 0.5f;

    private Transform cam;
    private float camStartX;
    private float myStartX;

    void Start()
    {
        cam = Camera.main != null ? Camera.main.transform : null;
        if (cam != null) camStartX = cam.position.x;
        myStartX = transform.position.x;
    }

    void LateUpdate()
    {
        if (cam == null) { cam = Camera.main != null ? Camera.main.transform : null; return; }
        float delta = cam.position.x - camStartX;
        Vector3 p = transform.position;
        p.x = myStartX + delta * factor;
        transform.position = p;
    }
}
