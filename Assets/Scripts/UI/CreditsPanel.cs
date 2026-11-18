using UnityEngine;

/// <summary>
/// Panel de creditos con scroll vertical automatico.
/// Resetea la posicion al abrirse; el boton Volver llama MostrarPrincipal del MainMenu.
/// </summary>
public class CreditsPanel : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RectTransform scrollingContent;
    [SerializeField] private MainMenu mainMenu;

    [Header("Animacion")]
    [SerializeField] private float velocidadScroll = 40f;
    [SerializeField] private float yInicio = -400f;
    [SerializeField] private float yFin = 1600f;
    [SerializeField] private bool loop = true;

    void OnEnable()
    {
        if (scrollingContent != null)
        {
            Vector2 pos = scrollingContent.anchoredPosition;
            pos.y = yInicio;
            scrollingContent.anchoredPosition = pos;
        }
    }

    void Update()
    {
        if (scrollingContent == null) return;
        Vector2 pos = scrollingContent.anchoredPosition;
        pos.y += velocidadScroll * Time.unscaledDeltaTime;
        if (pos.y > yFin)
        {
            if (loop) pos.y = yInicio;
            else pos.y = yFin;
        }
        scrollingContent.anchoredPosition = pos;
    }

    public void OnVolver()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUIClick();
        if (mainMenu != null) mainMenu.MostrarPrincipal();
    }
}
