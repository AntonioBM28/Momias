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

    public void OnJugar()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUIClick();
        if (GameManager.Instance != null)
        {
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
