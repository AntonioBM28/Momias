using System.IO;
using UnityEngine;

/// <summary>
/// Datos persistidos del juego. JSON en Application.persistentDataPath.
/// </summary>
[System.Serializable]
public class SaveData
{
    public int nivelMax = 1;                 // Nivel mas alto alcanzado (1-4)
    public int nivelActualGuardado = 0;      // 0 = no hay partida en curso; 1-4 = "Continuar" empieza ese nivel
    public int vidaActualGuardada = -1;      // Solo informativo: al continuar Diego empieza con vida completa
    public bool juegoCompletado = false;     // true al vencer el ultimo nivel (Momia Madre)
    public float volumenMusica = 0.6f;
    public float volumenSFX = 1.0f;
}

/// <summary>
/// Sistema de guardado: persiste progreso + vida + volumenes entre sesiones.
/// Uso: SaveSystem.Load() para leer, Save() para persistir, helpers especificos para cosas puntuales.
/// </summary>
public static class SaveSystem
{
    private static string PATH => Path.Combine(Application.persistentDataPath, "momias_save.json");
    private static SaveData _cache;

    public static SaveData Load()
    {
        if (_cache != null) return _cache;
        if (File.Exists(PATH))
        {
            try
            {
                string json = File.ReadAllText(PATH);
                _cache = JsonUtility.FromJson<SaveData>(json);
                if (_cache == null) _cache = new SaveData();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[SaveSystem] Error leyendo save: {e.Message}. Usando defaults.");
                _cache = new SaveData();
            }
        }
        else _cache = new SaveData();
        return _cache;
    }

    public static void Save()
    {
        if (_cache == null) _cache = new SaveData();
        try
        {
            File.WriteAllText(PATH, JsonUtility.ToJson(_cache, true));
            Debug.Log($"[SaveSystem] Guardado: nivelMax={_cache.nivelMax} actual={_cache.nivelActualGuardado} vida={_cache.vidaActualGuardada}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Error guardando: {e.Message}");
        }
    }

    public static void Delete()
    {
        _cache = new SaveData();
        if (File.Exists(PATH)) File.Delete(PATH);
        Debug.Log("[SaveSystem] Progreso borrado");
    }

    /// <summary>
    /// Hay partida para "Continuar" si se guardo un nivel en curso (1-4).
    /// La vida ya no cuenta: al morir se conserva el nivel y se continua con vida completa.
    /// </summary>
    public static bool HayPartidaGuardada()
    {
        var d = Load();
        return d.nivelActualGuardado >= 1 && d.nivelActualGuardado <= NivelFinal;
    }

    /// <summary>Numero del ultimo nivel del juego.</summary>
    public const int NivelFinal = 4;

    /// <summary>Guarda que el jugador va en este nivel (lo llama el GameManager al cargar cada nivel).</summary>
    public static void GuardarNivelEnCurso(int nivel)
    {
        if (nivel < 1 || nivel > NivelFinal) return;
        var d = Load();
        bool cambio = d.nivelActualGuardado != nivel || nivel > d.nivelMax;
        d.nivelActualGuardado = nivel;
        if (nivel > d.nivelMax) d.nivelMax = nivel;
        if (cambio) Save();
    }

    /// <summary>Al ganar un nivel: deja guardado el siguiente, o marca el juego como completado si era el ultimo.</summary>
    public static void GuardarNivelCompletado(int nivel)
    {
        if (nivel < 1) return;
        if (nivel >= NivelFinal) { MarcarJuegoCompletado(); return; }
        GuardarNivelEnCurso(nivel + 1);
    }

    /// <summary>El jugador vencio el ultimo nivel: ya no hay partida que continuar.</summary>
    public static void MarcarJuegoCompletado()
    {
        var d = Load();
        d.juegoCompletado = true;
        d.nivelMax = NivelFinal;
        d.nivelActualGuardado = 0;
        d.vidaActualGuardada = -1;
        Save();
    }

    /// <summary>"Nueva partida": borra el avance (conserva volumenes y el nivel maximo alcanzado).</summary>
    public static void NuevaPartida()
    {
        var d = Load();
        d.nivelActualGuardado = 0;
        d.vidaActualGuardada = -1;
        d.juegoCompletado = false;
        Save();
    }

    public static void GuardarNivelMaxSi(int nivel)
    {
        var d = Load();
        if (nivel > d.nivelMax) { d.nivelMax = nivel; Save(); }
    }

    public static void GuardarPartida(int nivel, int vida)
    {
        var d = Load();
        d.nivelActualGuardado = nivel;
        d.vidaActualGuardada = vida;
        Save();
    }

    public static void LimpiarPartida()
    {
        var d = Load();
        d.nivelActualGuardado = 0;
        d.vidaActualGuardada = -1;
        Save();
    }

    public static void GuardarVolumenes(float musica, float sfx)
    {
        var d = Load();
        d.volumenMusica = musica;
        d.volumenSFX = sfx;
        Save();
    }

    /// <summary>Numero de nivel (1-4) a partir de un nombre de escena.</summary>
    public static int NivelDeEscena(string sceneName) => sceneName switch
    {
        "Nivel1_Museo" => 1,
        "Nivel2_Alhondiga" => 2,
        "Nivel3_Callejones" => 3,
        "Nivel4_Parroquia" => 4,
        _ => 0,
    };

    public static string EscenaDeNivel(int nivel) => nivel switch
    {
        1 => "Nivel1_Museo",
        2 => "Nivel2_Alhondiga",
        3 => "Nivel3_Callejones",
        4 => "Nivel4_Parroquia",
        _ => "Nivel1_Museo",
    };
}
