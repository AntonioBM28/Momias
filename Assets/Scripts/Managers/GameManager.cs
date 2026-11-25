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
        ReproducirMusicaDelNivel(sceneName);
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
        ReproducirMusicaDelNivel(scene.name);
        // Guardar que el jugador va en este nivel: si muere o sale, "Continuar" lo trae aqui con vida completa
        int nivelEscena = SaveSystem.NivelDeEscena(scene.name);
        if (nivelEscena > 0) SaveSystem.GuardarNivelEnCurso(nivelEscena);
    }

    private void ReproducirMusicaDelNivel(string sceneName)
    {
        if (AudioManager.Instance == null) return;
        AudioClip musica = sceneName switch
        {
            "Menu"              => AudioManager.Instance.musicMenu,
            "Nivel1_Museo"      => AudioManager.Instance.musicNivel1,
            "Nivel2_Alhondiga"  => AudioManager.Instance.musicNivel2,
            "Nivel3_Callejones" => AudioManager.Instance.musicNivel3,
            "Nivel4_Parroquia"  => AudioManager.Instance.musicNivel4,
            _ => null
        };
        if (musica != null) AudioManager.Instance.PlayMusic(musica, true);
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
        // Guardar en cuanto se gana: asi el progreso queda aunque el jugador elija "Menu" en vez de "Siguiente"
        GuardarNivelActualCompletado();
        SetState(GameState.Victory);
    }

    /// <summary>Guarda el siguiente nivel como partida en curso, o marca el juego completado si era el ultimo.</summary>
    private void GuardarNivelActualCompletado()
    {
        int nivelActual = SaveSystem.NivelDeEscena(SceneManager.GetActiveScene().name);
        if (nivelActual > 0) SaveSystem.GuardarNivelCompletado(nivelActual);
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

    /// <summary>Nombre de la escena siguiente a la actual, o "" si no hay.</summary>
    public string GetNextLevelSceneName()
    {
        string current = SceneManager.GetActiveScene().name;
        return current switch
        {
            "Nivel1_Museo"      => "Nivel2_Alhondiga",
            "Nivel2_Alhondiga"  => "Nivel3_Callejones",
            "Nivel3_Callejones" => "Nivel4_Parroquia",
            _ => ""
        };
    }

    public void GoToNextLevel()
    {
        // Guardar progreso: el nivel actual quedo completado (ya se guarda en TriggerVictory; repetirlo es inofensivo)
        GuardarNivelActualCompletado();

        string next = GetNextLevelSceneName();
        if (string.IsNullOrEmpty(next))
        {
            Debug.LogWarning("[GameManager] No hay nivel siguiente, voy al menu.");
            GoToMenu();
            return;
        }
        Time.timeScale = 1f;
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadSceneWithFade(next);
        else
            SceneManager.LoadScene(next);
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
