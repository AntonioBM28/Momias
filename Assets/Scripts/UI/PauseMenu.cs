using UnityEngine;

/// <summary>
/// Detecta Esc y pausa/reanuda. Soporta sub-paneles (opciones y creditos) con
/// el mismo patron que el MainMenu, para que la experiencia sea consistente.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("Panel principal")]
    [SerializeField] private GameObject pausePanel;

    [Header("Sub-paneles")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Config")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        MostrarPrincipal();

        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += OnGameStateChanged;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnGameStateChanged;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        var state = GameManager.Instance.CurrentState;

        if (Input.GetKeyDown(pauseKey))
        {
            if (state == GameManager.GameState.Playing || state == GameManager.GameState.Paused)
            {
                GameManager.Instance.TogglePause();
                // Al reabrir el pause siempre volver al panel principal
                if (GameManager.Instance.CurrentState == GameManager.GameState.Paused)
                    MostrarPrincipal();
            }
        }
    }

    private void OnGameStateChanged(GameManager.GameState nuevo)
    {
        if (pausePanel == null) return;
        pausePanel.SetActive(nuevo == GameManager.GameState.Paused);
        if (nuevo == GameManager.GameState.Paused) MostrarPrincipal();
    }

    // --- Sub-paneles ---

    public void MostrarPrincipal()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    public void MostrarOpciones()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUIClick();
        if (mainPanel != null) mainPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    public void MostrarCreditos()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUIClick();
        if (mainPanel != null) mainPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    // --- Hooks para botones ---

    public void OnContinuar()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUIClick();
        if (GameManager.Instance != null) GameManager.Instance.Resume();
    }

    public void OnMenu()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUIClick();
        if (GameManager.Instance != null) GameManager.Instance.GoToMenu();
    }

    public void OnSalir()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUIClick();
        if (GameManager.Instance != null) GameManager.Instance.QuitGame();
    }
}
