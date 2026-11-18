using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel de Opciones con sliders de volumen (Musica + SFX) ya funcionales.
/// Mas placeholder "Proximamente en Avance 4" para el resto.
/// Los sliders se sincronizan con el AudioManager.
/// </summary>
public class OptionsPanel : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private Slider sliderSFX;
    [SerializeField] private MainMenu mainMenu;

    void OnEnable()
    {
        // Sincroniza visual del slider con el volumen actual del AudioManager
        if (sliderMusica != null) sliderMusica.SetValueWithoutNotify(ObtenerVolumenMusica());
        if (sliderSFX != null) sliderSFX.SetValueWithoutNotify(ObtenerVolumenSFX());
    }

    // --- Hooks de los sliders (asignados en el inspector) ---

    public void OnMusicaChanged(float v)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetMusicVolume(v);
    }

    public void OnSFXChanged(float v)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetSFXVolume(v);
    }

    public void OnVolver()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUIClick();
        if (mainMenu != null) mainMenu.MostrarPrincipal();
    }

    // --- Helpers de lectura (AudioManager los mantiene privados; usamos defaults) ---

    private float ObtenerVolumenMusica()
    {
        // AudioManager no expone getters; usamos default razonable
        return 0.6f;
    }

    private float ObtenerVolumenSFX()
    {
        return 1.0f;
    }
}
