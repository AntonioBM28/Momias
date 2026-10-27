using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Carga escenas con fade a negro: fade out -> cargar -> fade in.
/// Singleton persistente. Depende de un Fader asignado en el inspector.
/// </summary>
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [Header("Dependencias")]
    [SerializeField] private Fader fader;

    [Header("Configuracion")]
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float fadeInDuration = 0.5f;

    public bool IsTransitioning { get; private set; }

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
        // Arranca con fade in para que la primera escena no aparezca de golpe
        if (fader != null) StartCoroutine(fader.FadeIn(fadeInDuration));
    }

    public void LoadSceneWithFade(string sceneName)
    {
        if (IsTransitioning) return;
        StartCoroutine(LoadRoutine(sceneName));
    }

    private IEnumerator LoadRoutine(string sceneName)
    {
        IsTransitioning = true;

        if (fader != null)
            yield return fader.FadeOut(fadeOutDuration);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) yield return null;

        if (fader != null)
            yield return fader.FadeIn(fadeInDuration);

        IsTransitioning = false;
    }
}
