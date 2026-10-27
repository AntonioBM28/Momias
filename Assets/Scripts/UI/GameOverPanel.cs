using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Escucha PlayerHealth.OnDeath -> espera un breve delay (para que la muerte
/// se vea) -> activa el panel de GameOver. Botones: Reintentar y Menu.
/// </summary>
public class GameOverPanel : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private CanvasGroup panelGroup; // para fade-in

    [Header("Config")]
    [SerializeField] private float delayAntesDeMostrar = 1.2f;
    [SerializeField] private float fadeInDuration = 0.5f;

    [Header("Referencias")]
    [SerializeField] private PlayerHealth playerHealth;

    private bool yaMostrado;

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (panelGroup != null) panelGroup.alpha = 0f;

        if (playerHealth == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerHealth = p.GetComponent<PlayerHealth>();
        }
        if (playerHealth != null)
            playerHealth.OnDeath.AddListener(OnPlayerDied);
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnDeath.RemoveListener(OnPlayerDied);
    }

    private void OnPlayerDied()
    {
        if (yaMostrado) return;
        yaMostrado = true;

        if (GameManager.Instance != null)
            GameManager.Instance.TriggerGameOver();

        StartCoroutine(MostrarPanel());
    }

    private IEnumerator MostrarPanel()
    {
        yield return new WaitForSeconds(delayAntesDeMostrar);

        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (AudioManager.Instance != null && AudioManager.Instance.musicGameOver != null)
            AudioManager.Instance.PlayMusic(AudioManager.Instance.musicGameOver, true);

        if (panelGroup != null)
        {
            float t = 0f;
            while (t < fadeInDuration)
            {
                t += Time.unscaledDeltaTime;
                panelGroup.alpha = Mathf.Clamp01(t / fadeInDuration);
                yield return null;
            }
            panelGroup.alpha = 1f;
        }
    }

    // --- Hooks de botones ---

    public void OnReintentar()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUIClick();
        if (GameManager.Instance != null) GameManager.Instance.RestartCurrentLevel();
    }

    public void OnMenu()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUIClick();
        if (GameManager.Instance != null) GameManager.Instance.GoToMenu();
    }
}
