using System.Collections;
using UnityEngine;

/// <summary>
/// Gestor de musica y SFX. Singleton persistente.
/// Soporta fade entre pistas de musica. Para SFX usa PlaySFX(clip).
///
/// Para Chunk 1 esta "vacio" (sin clips asignados). En Chunk 3 cuando
/// Jose entregue los audios, se arrastran a los slots del inspector.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Fuentes de audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Volumenes (0 a 1)")]
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.6f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;

    [Header("Pistas de musica (asignar en Chunk 3)")]
    public AudioClip musicMenu;
    public AudioClip musicNivel1;
    public AudioClip musicVictoria;
    public AudioClip musicGameOver;

    [Header("SFX (asignar en Chunk 3)")]
    public AudioClip sfxCampanazo;
    public AudioClip sfxImpactoMomia;
    public AudioClip sfxMuerteMomia;
    public AudioClip sfxSalto;
    public AudioClip sfxPickupCorazon;
    public AudioClip sfxCampanaSagrada;
    public AudioClip sfxUIClick;

    [Header("Configuracion")]
    [SerializeField] private float musicFadeDuration = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }

    // --- Musica ---

    public void PlayMusic(AudioClip clip, bool fade = true)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] PlayMusic llamado con clip null (normal en Chunk 1).");
            return;
        }
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        if (fade) StartCoroutine(CrossfadeMusic(clip));
        else
        {
            musicSource.clip = clip;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    public void StopMusic(bool fade = true)
    {
        if (fade) StartCoroutine(FadeOutMusic());
        else musicSource.Stop();
    }

    public void SetMusicVolume(float v)
    {
        musicVolume = Mathf.Clamp01(v);
        musicSource.volume = musicVolume;
    }

    private IEnumerator CrossfadeMusic(AudioClip nuevo)
    {
        float t = 0f;
        float startVol = musicSource.volume;
        while (t < musicFadeDuration)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVol, 0f, t / musicFadeDuration);
            yield return null;
        }
        musicSource.Stop();
        musicSource.clip = nuevo;
        musicSource.Play();

        t = 0f;
        while (t < musicFadeDuration)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, musicVolume, t / musicFadeDuration);
            yield return null;
        }
        musicSource.volume = musicVolume;
    }

    private IEnumerator FadeOutMusic()
    {
        float startVol = musicSource.volume;
        float t = 0f;
        while (t < musicFadeDuration)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVol, 0f, t / musicFadeDuration);
            yield return null;
        }
        musicSource.Stop();
        musicSource.volume = musicVolume;
    }

    // --- SFX ---

    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume * volumeScale);
    }

    public void SetSFXVolume(float v)
    {
        sfxVolume = Mathf.Clamp01(v);
        sfxSource.volume = sfxVolume;
    }

    // --- Atajos para los SFX comunes del juego ---
    public void PlayCampanazo() => PlaySFX(sfxCampanazo);
    public void PlayImpactoMomia() => PlaySFX(sfxImpactoMomia);
    public void PlayMuerteMomia() => PlaySFX(sfxMuerteMomia);
    public void PlaySalto() => PlaySFX(sfxSalto, 0.6f);
    public void PlayPickupCorazon() => PlaySFX(sfxPickupCorazon);
    public void PlayCampanaSagrada() => PlaySFX(sfxCampanaSagrada);
    public void PlayUIClick() => PlaySFX(sfxUIClick, 0.8f);
}
