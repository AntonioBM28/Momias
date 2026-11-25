using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Fase G: Nivel 4 Parroquia de Dolores Hidalgo + Boss Momia Madre.
///
/// ⚠️ BLOQUEADO hasta que Antonio entregue:
///   - Sprites Momia Madre (Idle/Walk/Attack x2-3/Die)
///   - Animator Controller MomiaMadre.controller
///   - Prefab MomiaMadre.prefab
///   - ScriptableObject MomiaMadre.asset
///   - Musica Nivel 4 (loop)
///   - Musica Boss Final (loop)
///
/// Patron de referencia: ver FaseD_Nivel2.cs (SpawnBossYHealthBar, BossBridge).
/// Reemplazar "Pipila" por "MomiaMadre" en todos los paths/strings.
/// </summary>
public static class FaseG_Nivel4
{
    private const string MENU = "Momias/Fase G/";

    // TODO: paths cuando Antonio entregue
    private const string PATH_SCENE_N4 = "Assets/Scenes/Nivel4_Parroquia.unity";
    private const string PATH_PREFAB_MADRE = "Assets/Prefabs/MomiaMadre.prefab";
    private const string PATH_SO_MADRE = "Assets/ScriptableObjects/Enemies/MomiaMadre.asset";
    // private const string PATH_BELL_SPRITE = "Assets/Sprites/Items/SacredBell.png"; // reusar

    // =====================================================================
    // MENU PRINCIPAL
    // =====================================================================

    [MenuItem(MENU + "🚀 Build ALL Nivel 4 (Parroquia + Madre)", priority = 2)]
    public static void BuildAll()
    {
        if (!EditorUtility.DisplayDialog("Fase G - Nivel 4",
            "⚠️ BLOQUEADO: faltan assets de Antonio.\n\n" +
            "Cuando llegue la Momia Madre + musicas, llenar este script siguiendo\n" +
            "el patron de FaseD_Nivel2.cs.\n\n" +
            "Steps que falta implementar:\n" +
            "  1. SetupCamaraEstatica (misma camara que Nivel 1/2/3)\n" +
            "  2. SpawnDiego (clonar de Nivel 1 via additive)\n" +
            "  3. SpawnManagers con AsignarAudios (incluir musicNivel4 + musicBossFinal por GUID)\n" +
            "  4. SpawnGameUI (GameOver/Victory/LevelTitleCard 'NIVEL 4 / PARROQUIA DE DOLORES HIDALGO')\n" +
            "  5. SpawnPauseMenu inline\n" +
            "  6. SpawnWaveManager con 1-2 oleadas cortas de intro (opcional)\n" +
            "  7. SpawnBossMadreYHealthBar (con BossBridge para 3 fases: lagrimas / torbellino / mini-momias+salto)\n" +
            "  8. AddBuildSettingsIfMissing\n\n" +
            "Script MomiaMadre.cs tambien hay que crearlo:\n" +
            "  - Hereda EnemyBase\n" +
            "  - Fase 1 (100-66%): lagrimas que caen del cielo (proyectiles verticales)\n" +
            "  - Fase 2 (66-33%): torbellino de polvo que persigue a Diego\n" +
            "  - Fase 3 (33-0%): invoca 2-3 mini-momias + salto con onda expansiva",
            "OK (saldra avisando)", "Cancelar")) return;

        Debug.LogWarning("[Fase G] BLOQUEADO: implementar cuando lleguen assets de Antonio.");
    }

    // =====================================================================
    // STEPS (esqueletos vacios, llenar cuando haya assets)
    // =====================================================================

    [MenuItem(MENU + "N4.1 Open scene", priority = 40)]
    public static void OpenScene()
    {
        // TODO: crear Nivel4_Parroquia.unity cuando haya backgrounds
        if (!System.IO.File.Exists(PATH_SCENE_N4))
        {
            Debug.LogError("[FaseG] No existe " + PATH_SCENE_N4 + ". Hay que crearla primero (Antonio tiene el block-out).");
            return;
        }
        EditorSceneManager.OpenScene(PATH_SCENE_N4);
    }

    [MenuItem(MENU + "N4.7 Spawn Boss Madre + HealthBar", priority = 46)]
    public static void SpawnBossMadreYHealthBar()
    {
        // TODO: copiar patron de FaseD_Nivel2.SpawnBossYHealthBar
        // - FixBossMadrePrefab (asignar MomiaMadre.cs + data + collider)
        // - Instanciar prefab disabled en (0, 2, 0)
        // - BuildBossHealthBar (reusar la clase BossHealthBar, solo cambiar texto a "MOMIA MADRE")
        // - BossBridge separado con suscripcion en AparicionBoss
        // - Hook del WaveManager.OnAllWavesCompleted -> bridge.AparicionBoss
        Debug.LogWarning("[FaseG] TODO: implementar SpawnBossMadreYHealthBar cuando lleguen sprites de Antonio");
    }

    // TODO: SetupCamaraEstatica, SpawnDiego, SpawnManagers, SpawnGameUI,
    //       SpawnPauseMenu, SpawnWaveManager, AddBuildSettingsIfMissing, FixBossMadrePrefab
}
