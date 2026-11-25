using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Monta la escena Nivel3_Callejones completa (sin boss, 5 oleadas con 4 variantes).
/// Oleadas segun brief: 4C+1B+1Cor+1Esc / 5C+1B+2Cor+2Esc / 5C+2B+2Cor+2Esc / 6C+2B+3Cor+2Esc / 8C+3B+4Cor+3Esc = 59 enemigos.
/// Al terminar oleadas -> Campana Sagrada. Sin boss.
/// </summary>
public static class FaseF_Nivel3
{
    private const string MENU = "Momias/Fase F/";
    private const string PATH_SCENE_N3 = "Assets/Scenes/Nivel3_Callejones.unity";
    private const string PATH_SCENE_N1 = "Assets/Scenes/Nivel1_Museo.unity";

    private const string PATH_PREFAB_CAMINANTE = "Assets/Prefabs/MomiaWalking_0.prefab";
    private const string PATH_PREFAB_BEBE = "Assets/Prefabs/MomiaBebe.prefab";
    private const string PATH_PREFAB_CORREDORA = "Assets/Prefabs/MomiaCorredora.prefab";
    private const string PATH_PREFAB_ESCUPIDORA = "Assets/Prefabs/MomiaEscupidora.prefab";
    private const string PATH_SO_CAMINANTE = "Assets/ScriptableObjects/Enemies/MomiaCaminante_Gris.asset";
    private const string PATH_SO_BEBE = "Assets/ScriptableObjects/Enemies/MomiaBebe.asset";
    private const string PATH_SO_CORREDORA = "Assets/ScriptableObjects/Enemies/MomiaCorredora.asset";
    private const string PATH_SO_ESCUPIDORA = "Assets/ScriptableObjects/Enemies/MomiaEscupidora.asset";
    private const string PATH_BELL_SPRITE = "Assets/Sprites/Items/CampanaSagrada.png";

    [MenuItem(MENU + "🚀 Build ALL Nivel 3 (Callejones + 4 variantes)", priority = 2)]
    public static void BuildAll()
    {
        if (!EditorUtility.DisplayDialog("Fase F - Nivel 3",
            "Monta Nivel3_Callejones:\n\n" +
            "  • Camara estatica\n" +
            "  • Diego (clonado de Nivel 1)\n" +
            "  • Managers + 11 audios cableados\n" +
            "  • GameUI (GameOver, Victory, LevelTitleCard \"NIVEL 3 / CALLEJONES\")\n" +
            "  • PauseMenu (mismo diseno dorado)\n" +
            "  • WaveManager con 5 oleadas con 4 variantes (59 enemigos totales)\n" +
            "  • Campana Sagrada al terminar oleadas (sin boss)\n" +
            "  • Agrega la escena a Build Settings\n\n" +
            "¿Continuar?", "Si", "Cancelar")) return;

        // Asegurar que el prefab del proyectil tiene el fix de layer Default (crucial para dañar a Diego)
        FaseE_Escupidora.IntegrarProyectilPrefabAntonio();
        FaseE_Escupidora.IntegrarEscupidoraPrefab();

        OpenScene();

        // Limpiar oleada TEST_Escupidora de Nivel 1 si quedo
        LimpiarTestEscupidoraNivel1();

        OpenScene();  // re-asegurar Nivel 3 abierto
        SetupCamaraEstatica();
        SpawnDiego();
        FaseD_Nivel2.SpawnManagersEnEscenaActual();
        SpawnGameUI();
        SpawnWaveManager();
        SpawnCampanaSagradaConfig();
        FaseD_Nivel2.AddBuildSettingsIfMissing(PATH_SCENE_N3);

        Scene s = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(s);
        EditorSceneManager.SaveScene(s);

        EditorUtility.DisplayDialog("Fase F",
            "✅ Nivel 3 Callejones listo!\n\n" +
            "Prueba:\n" +
            "  1. Abre Nivel3_Callejones y Play (o desde Menu -> Nivel 1 -> 2 -> 3)\n" +
            "  2. 5 oleadas: 7 + 10 + 11 + 13 + 18 enemigos = 59 total\n" +
            "  3. Ves: Caminantes, Bebes (agacharse para matar), Corredoras, Escupidoras\n" +
            "  4. Al terminar oleadas, aparece Campana Sagrada -> Victory",
            "OK");
    }

    [MenuItem(MENU + "N3.1 Open scene", priority = 40)]
    public static void OpenScene()
    {
        Scene cur = EditorSceneManager.GetActiveScene();
        if (cur.path != PATH_SCENE_N3)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(PATH_SCENE_N3);
        }
    }

    [MenuItem(MENU + "N3.2 Camara estatica", priority = 41)]
    public static void SetupCamaraEstatica()
    {
        OpenScene();
        Camera cam = Camera.main;
        if (cam == null) cam = Object.FindFirstObjectByType<Camera>();
        if (cam == null) { Debug.LogError("[N3] No hay camara"); return; }
        var cf = cam.GetComponent<CameraFollow>();
        if (cf != null) Object.DestroyImmediate(cf);
        cam.transform.position = new Vector3(0f, 0f, -10f);
        cam.orthographic = true;
        cam.orthographicSize = 3f;  // match N1/N2
        Debug.Log("[N3] ✅ Camara estatica en (0,0,-10) size 5");
    }

    [MenuItem(MENU + "N3.3 Spawn Diego", priority = 42)]
    public static void SpawnDiego()
    {
        OpenScene();
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            Debug.Log("[N3] Diego ya existe");
            return;
        }
        Scene cur = EditorSceneManager.GetActiveScene();
        Scene n1 = EditorSceneManager.OpenScene(PATH_SCENE_N1, OpenSceneMode.Additive);
        GameObject diegoN1 = null;
        foreach (GameObject go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            if (go.scene == n1 && go.CompareTag("Player")) { diegoN1 = go; break; }

        if (diegoN1 == null) { Debug.LogError("[N3] No encontre Diego en Nivel 1"); EditorSceneManager.CloseScene(n1, true); return; }

        GameObject clone = Object.Instantiate(diegoN1);
        clone.name = diegoN1.name;
        clone.transform.position = new Vector3(0f, -1f, 0f);
        SceneManager.MoveGameObjectToScene(clone, cur);
        EditorSceneManager.CloseScene(n1, true);
        Debug.Log("[N3] ✅ Diego spawneado");
    }

    [MenuItem(MENU + "N3.4 Spawn GameUI", priority = 43)]
    public static void SpawnGameUI()
    {
        OpenScene();
        Chunk1Setup.BuildGameOverPanel(silent: true);
        Chunk1Setup.BuildVictorySystem(silent: true);
        Chunk1Setup.BuildLevelTitleCard(silent: true);

        var card = Object.FindFirstObjectByType<LevelTitleCard>();
        if (card != null)
        {
            SerializedObject so = new SerializedObject(card);
            so.FindProperty("tituloNivel").stringValue = "NIVEL 3";
            so.FindProperty("subtituloNivel").stringValue = "CALLEJONES DE GUANAJUATO";
            so.ApplyModifiedProperties();
            Transform contenido = card.transform.Find("Contenido");
            if (contenido != null)
            {
                var t = contenido.Find("Titulo"); if (t != null) t.GetComponent<TMPro.TMP_Text>().text = "NIVEL 3";
                var st = contenido.Find("Subtitulo"); if (st != null) st.GetComponent<TMPro.TMP_Text>().text = "CALLEJONES DE GUANAJUATO";
            }
        }

        FaseD_Nivel2.BuildHeartsHUDSiFalta();

        GameObject canvas = GameObject.Find("GameUI_Canvas");
        if (canvas != null)
        {
            Transform existing = canvas.transform.Find("PauseMenu");
            if (existing != null) Object.DestroyImmediate(existing.gameObject);
            FaseD_Nivel2.BuildPauseMenuInline(canvas);
        }
        Debug.Log("[N3] ✅ GameUI listo");
    }

    [MenuItem(MENU + "N3.5 Spawn WaveManager (5 oleadas)", priority = 44)]
    public static void SpawnWaveManager()
    {
        OpenScene();
        WaveManager wm = Object.FindFirstObjectByType<WaveManager>();
        if (wm == null)
        {
            GameObject wmGo = new GameObject("WaveManager");
            wm = wmGo.AddComponent<WaveManager>();
        }

        // FORZAR actualizar spawners a ±7 (aunque ya existan desde un BuildAll previo)
        Transform wmT = wm.transform;
        Transform existingI = wmT.Find("SpawnIzquierdo");
        if (existingI != null) Object.DestroyImmediate(existingI.gameObject);
        Transform existingD = wmT.Find("SpawnDerecho");
        if (existingD != null) Object.DestroyImmediate(existingD.gameObject);

        GameObject spawnI = new GameObject("SpawnIzquierdo");
        spawnI.transform.SetParent(wmT, false);
        spawnI.transform.position = new Vector3(-7f, -1f, 0f);
        GameObject spawnD = new GameObject("SpawnDerecho");
        spawnD.transform.SetParent(wmT, false);
        spawnD.transform.position = new Vector3(7f, -1f, 0f);

        {
            SerializedObject soWm = new SerializedObject(wm);
            soWm.FindProperty("spawnIzquierdo").objectReferenceValue = spawnI.transform;
            soWm.FindProperty("spawnDerecho").objectReferenceValue = spawnD.transform;
            soWm.ApplyModifiedProperties();
        }

        // Cargar prefabs y SO
        var pC = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_CAMINANTE);
        var pB = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_BEBE);
        var pCor = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_CORREDORA);
        var pEsc = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_ESCUPIDORA);
        var dC = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_CAMINANTE);
        var dB = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_BEBE);
        var dCor = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_CORREDORA);
        var dEsc = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_ESCUPIDORA);

        SerializedObject wmSo = new SerializedObject(wm);
        SerializedProperty oleadas = wmSo.FindProperty("oleadas");
        oleadas.arraySize = 0;

        // Estructura: cada oleada "grande" del brief se parte en sub-oleadas por tipo
        // con pausaAntesDeOleada corta entre sub-oleadas (0.5s) y larga entre principales (3s)
        List<(string, GameObject, EnemyData, int, float, float, bool)> waves = new List<(string, GameObject, EnemyData, int, float, float, bool)>
        {
            ("Oleada 1.1 (2C)", pC, dC, 2, 0.35f, 3.0f, true),
            ("Oleada 1.2 (1Cor)", pCor, dCor, 1, 0.35f, 0.4f, true),
            ("Oleada 1.3 (1B)", pB, dB, 1, 0.35f, 0.4f, true),
            ("Oleada 1.4 (1Esc)", pEsc, dEsc, 1, 0.35f, 0.4f, true),
            ("Oleada 1.5 (2C)", pC, dC, 2, 0.35f, 0.4f, false),
            ("Oleada 2.1 (2C)", pC, dC, 2, 0.35f, 3.0f, true),
            ("Oleada 2.2 (2Cor)", pCor, dCor, 2, 0.35f, 0.4f, true),
            ("Oleada 2.3 (1B)", pB, dB, 1, 0.35f, 0.4f, true),
            ("Oleada 2.4 (2Esc)", pEsc, dEsc, 2, 0.35f, 0.4f, true),
            ("Oleada 2.5 (2C)", pC, dC, 2, 0.35f, 0.4f, true),
            ("Oleada 2.6 (1C)", pC, dC, 1, 0.35f, 0.4f, false),
            ("Oleada 3.1 (2C)", pC, dC, 2, 0.35f, 3.0f, true),
            ("Oleada 3.2 (2Cor)", pCor, dCor, 2, 0.35f, 0.4f, true),
            ("Oleada 3.3 (2B)", pB, dB, 2, 0.35f, 0.4f, true),
            ("Oleada 3.4 (2Esc)", pEsc, dEsc, 2, 0.35f, 0.4f, true),
            ("Oleada 3.5 (2C)", pC, dC, 2, 0.35f, 0.4f, true),
            ("Oleada 3.6 (1C)", pC, dC, 1, 0.35f, 0.4f, false),
            ("Oleada 4.1 (2C)", pC, dC, 2, 0.35f, 3.0f, true),
            ("Oleada 4.2 (2Cor)", pCor, dCor, 2, 0.35f, 0.4f, true),
            ("Oleada 4.3 (2B)", pB, dB, 2, 0.35f, 0.4f, true),
            ("Oleada 4.4 (2Esc)", pEsc, dEsc, 2, 0.35f, 0.4f, true),
            ("Oleada 4.5 (2C)", pC, dC, 2, 0.35f, 0.4f, true),
            ("Oleada 4.6 (1Cor)", pCor, dCor, 1, 0.35f, 0.4f, true),
            ("Oleada 4.7 (2C)", pC, dC, 2, 0.35f, 0.4f, false),
            ("Oleada 5.1 (2C)", pC, dC, 2, 0.35f, 3.0f, true),
            ("Oleada 5.2 (2Cor)", pCor, dCor, 2, 0.35f, 0.4f, true),
            ("Oleada 5.3 (2B)", pB, dB, 2, 0.35f, 0.4f, true),
            ("Oleada 5.4 (2Esc)", pEsc, dEsc, 2, 0.35f, 0.4f, true),
            ("Oleada 5.5 (2C)", pC, dC, 2, 0.35f, 0.4f, true),
            ("Oleada 5.6 (2Cor)", pCor, dCor, 2, 0.35f, 0.4f, true),
            ("Oleada 5.7 (1B)", pB, dB, 1, 0.35f, 0.4f, true),
            ("Oleada 5.8 (1Esc)", pEsc, dEsc, 1, 0.35f, 0.4f, true),
            ("Oleada 5.9 (2C)", pC, dC, 2, 0.35f, 0.4f, true),
            ("Oleada 5.10 (2C)", pC, dC, 2, 0.35f, 0.4f, false),
        };

        oleadas.arraySize = waves.Count;
        for (int i = 0; i < waves.Count; i++)
        {
            var (nombre, prefab, data, cant, intv, pausa, continuar) = waves[i];
            var el = oleadas.GetArrayElementAtIndex(i);
            el.FindPropertyRelative("nombre").stringValue = nombre;
            el.FindPropertyRelative("prefabEnemigo").objectReferenceValue = prefab;
            el.FindPropertyRelative("dataEnemigo").objectReferenceValue = data;
            el.FindPropertyRelative("cantidad").intValue = cant;
            el.FindPropertyRelative("intervaloSpawn").floatValue = intv;
            el.FindPropertyRelative("pausaAntesDeOleada").floatValue = pausa;
            el.FindPropertyRelative("continuarSinEsperarMuertes").boolValue = continuar;
        }

        // Asignar sprite de Campana Sagrada (sin boss, es automatica al terminar)
        Sprite bell = AssetDatabase.LoadAssetAtPath<Sprite>(PATH_BELL_SPRITE);
        wmSo.FindProperty("spriteCampanaSagrada").objectReferenceValue = bell;
        wmSo.ApplyModifiedProperties();

        Debug.Log($"[N3] ✅ WaveManager con {waves.Count} sub-oleadas (59 enemigos totales)");
    }

    /// <summary>Config final de la Campana (sprite + pos). Nivel 3 NO tiene boss, campana sale directo.</summary>
    public static void SpawnCampanaSagradaConfig()
    {
        // La campana la crea el WaveManager automaticamente al OnAllWavesCompleted (default behavior)
        // Nada extra que hacer, ya quedo wired arriba.
        Debug.Log("[N3] ✅ Campana configurada (automatica al terminar oleadas)");
    }

    private static void LimpiarTestEscupidoraNivel1()
    {
        Scene n1 = EditorSceneManager.OpenScene(PATH_SCENE_N1);
        WaveManager wm = Object.FindFirstObjectByType<WaveManager>();
        if (wm == null) return;
        SerializedObject so = new SerializedObject(wm);
        SerializedProperty oleadas = so.FindProperty("oleadas");
        for (int i = oleadas.arraySize - 1; i >= 0; i--)
        {
            if (oleadas.GetArrayElementAtIndex(i).FindPropertyRelative("nombre").stringValue == "TEST_Escupidora")
            {
                oleadas.DeleteArrayElementAtIndex(i);
                so.ApplyModifiedProperties();
                EditorSceneManager.MarkSceneDirty(n1);
                EditorSceneManager.SaveScene(n1);
                Debug.Log("[N3] Limpiado TEST_Escupidora de Nivel 1");
                return;
            }
        }
    }
}
