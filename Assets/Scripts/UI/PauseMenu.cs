using UnityEngine;

/// <summary>
/// Detecta Esc y pausa/reanuda. Activa el panel de pausa y los botones.
/// Los botones llaman a OnContinuar(), OnMenu(), OnSalir() desde el inspector.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject pausePanel;

    [Header("Config")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);

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

        // Esc solo funciona si estamos jugando o pausados
        if (Input.GetKeyDown(pauseKey))
        {
            if (state == GameManager.GameState.Playing || state == GameManager.GameState.Paused)
                GameManager.Instance.TogglePause();
        }
    }

    private void OnGameStateChanged(GameManager.GameState nuevo)
    {
        if (pausePanel == null) return;
        pausePanel.SetActive(nuevo == GameManager.GameState.Paused);
    }

    // --- Hooks para botones del inspector ---

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
