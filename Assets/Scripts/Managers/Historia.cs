using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Punto de entrada para mostrar la historia entre escenas.
///
/// Uso: Historia.Reproducir("Intro", "Nivel1_Museo");
/// Carga la escena "Historia", que muestra las diapositivas del asset
/// Resources/Historia/{nombre} y al terminar (o al saltar) carga la escena siguiente.
/// Si el asset no existe o no tiene diapositivas, va directo a la escena siguiente.
/// </summary>
public static class Historia
{
    public const string EscenaHistoria = "Historia";
    private const string CarpetaResources = "Historia/";

    // Nombres de los assets en Assets/Resources/Historia/
    public const string Intro = "Intro";
    public const string Final = "Final";
    public static string Interludio(int nivelCompletado) => "Interludio_" + nivelCompletado;

    /// <summary>Historia pendiente de mostrar (la lee HistoriaPlayer al abrir la escena).</summary>
    public static HistoriaData Pendiente { get; private set; }

    /// <summary>Escena que se carga al terminar la historia.</summary>
    public static string EscenaSiguiente { get; private set; }

    public static void Reproducir(string nombre, string escenaSiguiente)
    {
        HistoriaData data = Resources.Load<HistoriaData>(CarpetaResources + nombre);
        if (data == null || data.diapositivas == null || data.diapositivas.Count == 0)
        {
            Debug.Log($"[Historia] No hay historia '{nombre}', voy directo a {escenaSiguiente}");
            CargarEscena(escenaSiguiente);
            return;
        }

        Pendiente = data;
        EscenaSiguiente = escenaSiguiente;
        CargarEscena(EscenaHistoria);
    }

    /// <summary>Lo llama HistoriaPlayer al terminar o al saltar.</summary>
    public static void Terminar()
    {
        string siguiente = string.IsNullOrEmpty(EscenaSiguiente) ? "Menu" : EscenaSiguiente;
        Pendiente = null;
        EscenaSiguiente = null;
        CargarEscena(siguiente);
    }

    private static void CargarEscena(string escena)
    {
        Time.timeScale = 1f;
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadSceneWithFade(escena);
        else
            SceneManager.LoadScene(escena);
    }
}
