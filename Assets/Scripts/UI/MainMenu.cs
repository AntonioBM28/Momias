using UnityEngine;

/// <summary>
/// Controlador del Menu Principal. Maneja los 3 paneles (main/credits/options)
/// y los hooks de los botones. Al arrancar reproduce la musica del menu
/// si esta disponible en el AudioManager.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Botones de partida")]
    [Tooltip("Se muestra solo si hay una partida guardada para continuar.")]
    [SerializeField] private GameObject btnContinuar;

    void Start()
    {
        MostrarPrincipal();

        // Intenta tocar musica del menu si el AudioManager existe y tiene el clip
        if (AudioManager.Instance != null && AudioManager.Instance.musicMenu != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.musicMenu);
        }
        else
        {
            Debug.Log("[MainMenu] musicMenu no asignada aun en AudioManager (pendiente de Antonio)");
        }
    }

    public void MostrarPrincipal()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (btnContinuar != null) btnContinuar.SetActive(SaveSystem.HayPartidaGuardada());
    }

    public void MostrarCreditos()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    public void MostrarOpciones()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    /// <summary>Boton "Continuar": empieza el nivel guardado con vida completa.</summary>
    public void OnContinuar()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUIClick();
        if (!SaveSystem.HayPartidaGuardada()) { OnNuevaPartida(); return; }
        string escena = SaveSystem.EscenaDeNivel(SaveSystem.Load().nivelActualGuardado);
        Debug.Log($"[MainMenu] Continuando partida en {escena}");
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadSceneWithFade(escena);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(escena);
    }

    /// <summary>Boton "Nueva partida": borra el avance y empieza en el Nivel 1.</summary>
    public void OnNuevaPartida()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUIClick();
        SaveSystem.NuevaPartida();
        if (GameManager.Instance != null)
            GameManager.Instance.GoToFirstLevel();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("Nivel1_Museo");
    }

    /// <summary>Boton anterior "Jugar" (se conserva por compatibilidad): continua si hay partida, si no empieza una nueva.</summary>
    public void OnJugar()
    {
        if (SaveSystem.HayPartidaGuardada())
        {
            var save = SaveSystem.Load();
            string escena = SaveSystem.EscenaDeNivel(save.nivelActualGuardado);
            Debug.Log($"[MainMenu] Continuando partida en {escena} con vida {save.vidaActualGuardada}");
            if (SceneTransition.Instance != null)
                SceneTransition.Instance.LoadSceneWithFade(escena);
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(escena);
        }
        else if (GameManager.Instance != null)
        {
            SaveSystem.LimpiarPartida();  // asegura arranque limpio
            GameManager.Instance.GoToFirstLevel();
        }
        else
        {
            // Fallback sin GameManager
            UnityEngine.SceneManagement.SceneManager.LoadScene("Nivel1_Museo");
        }
    }

    public void OnSalir()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUIClick();
        if (GameManager.Instance != null) GameManager.Instance.QuitGame();
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
