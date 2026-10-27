using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Director central del juego. Singleton con DontDestroyOnLoad: vive una sola
/// vez y sobrevive al cambio de escenas. Mantiene el GameState actual y notifica
/// a cualquier UI o sistema que escuche OnStateChanged.
///
/// Estados: Menu, Playing, Paused, GameOver, Victory.
/// Uso rapido:
///   GameManager.Instance.StartPlaying();
///   GameManager.Instance.TogglePause();
///   GameManager.Instance.TriggerGameOver();
///   GameManager.Instance.TriggerVictory();
/// </summary>
public class GameManager : MonoBehaviour
{
    public enum GameState { Menu, Playing, Paused, GameOver, Victory }

    // --- Singleton ---
    public static GameManager Instance { get; private set; }

    [Header("Configuracion")]
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private string firstLevelSceneName = "Nivel1_Museo";

    [Header("Estado (solo lectura en runtime)")]
    [SerializeField] private GameState currentState = GameState.Menu;

    // --- Eventos ---
    /// <summary>Dispara con el nuevo estado cada vez que cambia.</summary>
    public event Action<GameState> OnStateChanged;

    // --- Accesores publicos ---
    public GameState CurrentState => currentState;
    public bool IsMenu => currentState == GameState.Menu;
    public bool IsPlaying => currentState == GameState.Playing;
    public bool IsPaused => currentState == GameState.Paused;
    public bool IsGameOver => currentState == GameState.GameOver;
    public bool IsVictory => currentState == GameState.Victory;
    public string MenuSceneName => menuSceneName;
    public string FirstLevelSceneName => firstLevelSceneName;

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

    void Start()
    {
        // Al iniciar, decide el estado por la escena que se abrio
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == menuSceneName) SetState(GameState.Menu);
        else SetState(GameState.Playing);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Cada vez que carga una escena nueva, ajusta el estado por default
        Time.timeScale = 1f;
        if (scene.name == menuSceneName) SetState(GameState.Menu);
        else SetState(GameState.Playing);
    }

    // --- Metodos publicos para disparar transiciones ---

    public void StartPlaying()
    {
        Time.timeScale = 1f;
        SetState(GameState.Playing);
    }

    /// <summary>Pausa / reanuda. Se llama desde PauseMenu con Esc.</summary>
    public void TogglePause()
    {
        if (currentState == GameState.Playing) Pause();
        else if (currentState == GameState.Paused) Resume();
    }

    public void Pause()
    {
        if (currentState != GameState.Playing) return;
        Time.timeScale = 0f;
        SetState(GameState.Paused);
    }

    public void Resume()
    {
        if (currentState != GameState.Paused) return;
        Time.timeScale = 1f;
        SetState(GameState.Playing);
    }

    /// <summary>Lo llama PlayerHealth cuando la vida llega a 0.</summary>
    public void TriggerGameOver()
    {
        if (currentState == GameState.GameOver || currentState == GameState.Victory) return;
        Time.timeScale = 1f; // dejamos activo para que animaciones de GameOver corran
        SetState(GameState.GameOver);
    }

    /// <summary>Lo llama SacredBell cuando Diego la recoge con E.</summary>
    public void TriggerVictory()
    {
        if (currentState == GameState.Victory || currentState == GameState.GameOver) return;
        SetState(GameState.Victory);
    }

    // --- Navegacion de alto nivel ---

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadSceneWithFade(menuSceneName);
        else
            SceneManager.LoadScene(menuSceneName);
    }

    public void GoToFirstLevel()
    {
        Time.timeScale = 1f;
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadSceneWithFade(firstLevelSceneName);
        else
            SceneManager.LoadScene(firstLevelSceneName);
    }

    public void RestartCurrentLevel()
    {
        Time.timeScale = 1f;
        string current = SceneManager.GetActiveScene().name;
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadSceneWithFade(current);
        else
            SceneManager.LoadScene(current);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // --- Interno ---

    private void SetState(GameState nuevo)
    {
        if (currentState == nuevo) return;
        currentState = nuevo;
        Debug.Log($"[GameManager] Estado -> {nuevo}");
        OnStateChanged?.Invoke(nuevo);
    }
}
