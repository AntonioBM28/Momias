using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Monta la escena Nivel4_Parroquia completa (Boss Momia Madre + 2 oleadas de intro).
/// Boss tiene 3 fases: golpe cercano (F1), + embestida (F2 al 66%), + invocar mini-momias (F3 al 33%).
/// </summary>
public static class FaseG_Nivel4
{
    private const string MENU = "Momias/Fase G/";
    private const string PATH_SCENE_N4 = "Assets/Scenes/Nivel4_Parroquia.unity";
    private const string PATH_SCENE_N1 = "Assets/Scenes/Nivel1_Museo.unity";

    private const string PATH_PREFAB_CAMINANTE = "Assets/Prefabs/MomiaWalking_0.prefab";
    private const string PATH_PREFAB_BEBE = "Assets/Prefabs/MomiaBebe.prefab";
    private const string PATH_PREFAB_CORREDORA = "Assets/Prefabs/MomiaCorredora.prefab";
    private const string PATH_PREFAB_MADRE = "Assets/Prefabs/MomiaMadre.prefab";
    private const string PATH_SO_CAMINANTE = "Assets/ScriptableObjects/Enemies/MomiaCaminante_Gris.asset";
    private const string PATH_SO_BEBE = "Assets/ScriptableObjects/Enemies/MomiaBebe.asset";
    private const string PATH_SO_CORREDORA = "Assets/ScriptableObjects/Enemies/MomiaCorredora.asset";
    private const string PATH_SO_MADRE = "Assets/ScriptableObjects/Enemies/MomiaMadre.asset";
    private const string PATH_BELL_SPRITE = "Assets/Sprites/Items/CampanaSagrada.png";

    [MenuItem(MENU + "🚀 Build ALL Nivel 4 (Parroquia + Boss Madre)", priority = 2)]
    public static void BuildAll()
    {
        if (!EditorUtility.DisplayDialog("Fase G - Nivel 4",
            "Monta Nivel4_Parroquia:\n\n" +
            "  • Camara estatica\n" +
            "  • Diego (clonado de Nivel 1)\n" +
            "  • Managers + audios\n" +
            "  • GameUI (GameOver, Victory, LevelTitleCard \"NIVEL 4 / PARROQUIA\")\n" +
            "  • PauseMenu\n" +
            "  • WaveManager con 2 oleadas intro (3C+2B / 4C+2B+1Cor)\n" +
            "  • Boss MomiaMadre oculto + BossHealthBar + BossBridge generico\n" +
            "  • Campana Sagrada al matar a la Madre\n" +
            "  • Agrega la escena a Build Settings\n\n" +
            "¿Continuar?", "Si", "Cancelar")) return;

        // Fix del proyectil (layer Default para dañar a Diego)
        FaseE_Escupidora.IntegrarProyectilPrefabAntonio();
        FaseE_Escupidora.IntegrarEscupidoraPrefab();

        OpenScene();
        SetupCamaraEstatica();
        SpawnDiego();
        FaseD_Nivel2.SpawnManagersEnEscenaActual();
        SpawnGameUI();
        SpawnWaveManagerIntro();
        SpawnBossMadreYHealthBar();
        FaseD_Nivel2.AddBuildSettingsIfMissing(PATH_SCENE_N4);

        Scene s = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(s);
        EditorSceneManager.SaveScene(s);

        EditorUtility.DisplayDialog("Fase G",
            "✅ Nivel 4 Parroquia listo!\n\n" +
            "Prueba:\n" +
            "  1. Abre Nivel4_Parroquia y Play\n" +
            "  2. 2 oleadas intro (3C+2B, 4C+2B+1Cor)\n" +
            "  3. Aparece la MOMIA MADRE con barra dorada 'MOMIA MADRE'\n" +
            "  4. F1 (100-66%): golpe cruzado cercano\n" +
            "  5. F2 (66-33%): + embestida al correr a Diego\n" +
            "  6. F3 (33-0%): + invoca 2 mini-momias cada 8s\n" +
            "  7. Al morir: Campana Sagrada -> Victory final",
            "OK");
    }

    [MenuItem(MENU + "N4.1 Open scene", priority = 40)]
    public static void OpenScene()
    {
        Scene cur = EditorSceneManager.GetActiveScene();
        if (cur.path != PATH_SCENE_N4)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(PATH_SCENE_N4);
        }
    }

    [MenuItem(MENU + "N4.2 Camara estatica", priority = 41)]
    public static void SetupCamaraEstatica()
    {
        OpenScene();
        Camera cam = Camera.main;
        if (cam == null) cam = Object.FindFirstObjectByType<Camera>();
        if (cam == null) { Debug.LogError("[N4] No hay camara"); return; }
        var cf = cam.GetComponent<CameraFollow>();
        if (cf != null) Object.DestroyImmediate(cf);
        cam.transform.position = new Vector3(0f, 0f, -10f);
        cam.orthographic = true;
        cam.orthographicSize = 3f;  // match N1/N2
        Debug.Log("[N4] ✅ Camara estatica");
    }

    [MenuItem(MENU + "N4.3 Spawn Diego", priority = 42)]
    public static void SpawnDiego()
    {
        OpenScene();
        if (GameObject.FindGameObjectWithTag("Player") != null) return;
        Scene cur = EditorSceneManager.GetActiveScene();
        Scene n1 = EditorSceneManager.OpenScene(PATH_SCENE_N1, OpenSceneMode.Additive);
        GameObject diegoN1 = null;
        foreach (GameObject go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            if (go.scene == n1 && go.CompareTag("Player")) { diegoN1 = go; break; }
        if (diegoN1 == null) { Debug.LogError("[N4] No encontre Diego"); EditorSceneManager.CloseScene(n1, true); return; }
        GameObject clone = Object.Instantiate(diegoN1);
        clone.name = diegoN1.name;
        clone.transform.position = new Vector3(0f, -1f, 0f);
        SceneManager.MoveGameObjectToScene(clone, cur);
        EditorSceneManager.CloseScene(n1, true);
        Debug.Log("[N4] ✅ Diego spawneado");
    }

    [MenuItem(MENU + "N4.4 Spawn GameUI", priority = 43)]
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
            so.FindProperty("tituloNivel").stringValue = "NIVEL 4";
            so.FindProperty("subtituloNivel").stringValue = "PARROQUIA DE DOLORES HIDALGO";
            so.ApplyModifiedProperties();
            Transform contenido = card.transform.Find("Contenido");
            if (contenido != null)
            {
                var t = contenido.Find("Titulo"); if (t != null) t.GetComponent<TMPro.TMP_Text>().text = "NIVEL 4";
                var st = contenido.Find("Subtitulo"); if (st != null) st.GetComponent<TMPro.TMP_Text>().text = "PARROQUIA DE DOLORES HIDALGO";
            }
        }

        FaseD_Nivel2.BuildHeartsHUDSiFalta();

        GameObject canvas = GameObject.Find("GameUI_Canvas");
        if (canvas != null)
        {
            Transform ex = canvas.transform.Find("PauseMenu");
            if (ex != null) Object.DestroyImmediate(ex.gameObject);
            FaseD_Nivel2.BuildPauseMenuInline(canvas);
        }
        Debug.Log("[N4] ✅ GameUI listo");
    }

    [MenuItem(MENU + "N4.5 Spawn WaveManager (intro)", priority = 44)]
    public static void SpawnWaveManagerIntro()
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

        var pC = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_CAMINANTE);
        var pB = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_BEBE);
        var pCor = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_CORREDORA);
        var pEsc = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MomiaEscupidora.prefab");
        var dC = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_CAMINANTE);
        var dB = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_BEBE);
        var dCor = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_CORREDORA);
        var dEsc = AssetDatabase.LoadAssetAtPath<EnemyData>("Assets/ScriptableObjects/Enemies/MomiaEscupidora.asset");

        SerializedObject wmSo = new SerializedObject(wm);
        SerializedProperty oleadas = wmSo.FindProperty("oleadas");
        oleadas.arraySize = 0;

        List<(string, GameObject, EnemyData, int, float, float, bool)> waves = new List<(string, GameObject, EnemyData, int, float, float, bool)>
        {
            ("Oleada 1.1 (2C)", pC, dC, 2, 0.35f, 2.5f, true),
            ("Oleada 1.2 (2Cor)", pCor, dCor, 2, 0.35f, 0.4f, true),
            ("Oleada 1.3 (2B)", pB, dB, 2, 0.35f, 0.4f, true),
            ("Oleada 1.4 (1Esc)", pEsc, dEsc, 1, 0.35f, 0.4f, true),
            ("Oleada 1.5 (2C)", pC, dC, 2, 0.35f, 0.4f, true),
            ("Oleada 1.6 (1C)", pC, dC, 1, 0.35f, 0.4f, false),
            ("Oleada 2.1 (2C)", pC, dC, 2, 0.35f, 2.5f, true),
            ("Oleada 2.2 (2Cor)", pCor, dCor, 2, 0.35f, 0.4f, true),
            ("Oleada 2.3 (2B)", pB, dB, 2, 0.35f, 0.4f, true),
            ("Oleada 2.4 (2Esc)", pEsc, dEsc, 2, 0.35f, 0.4f, true),
            ("Oleada 2.5 (2C)", pC, dC, 2, 0.35f, 0.4f, true),
            ("Oleada 2.6 (2C)", pC, dC, 2, 0.35f, 0.4f, false),
            ("Oleada 3.1 (2C)", pC, dC, 2, 0.35f, 2.5f, true),
            ("Oleada 3.2 (2Cor)", pCor, dCor, 2, 0.35f, 0.4f, true),
            ("Oleada 3.3 (2B)", pB, dB, 2, 0.35f, 0.4f, true),
            ("Oleada 3.4 (2Esc)", pEsc, dEsc, 2, 0.35f, 0.4f, true),
            ("Oleada 3.5 (2C)", pC, dC, 2, 0.35f, 0.4f, true),
            ("Oleada 3.6 (1Cor)", pCor, dCor, 1, 0.35f, 0.4f, true),
            ("Oleada 3.7 (1B)", pB, dB, 1, 0.35f, 0.4f, true),
            ("Oleada 3.8 (2C)", pC, dC, 2, 0.35f, 0.4f, true),
            ("Oleada 3.9 (1C)", pC, dC, 1, 0.35f, 0.4f, false),
            ("Oleada 4.1 (2C)", pC, dC, 2, 0.35f, 2.5f, true),
            ("Oleada 4.2 (2Cor)", pCor, dCor, 2, 0.35f, 0.4f, true),
            ("Oleada 4.3 (2B)", pB, dB, 2, 0.35f, 0.4f, true),
            ("Oleada 4.4 (2Esc)", pEsc, dEsc, 2, 0.35f, 0.4f, true),
            ("Oleada 4.5 (2C)", pC, dC, 2, 0.35f, 0.4f, true),
            ("Oleada 4.6 (2Cor)", pCor, dCor, 2, 0.35f, 0.4f, true),
            ("Oleada 4.7 (1B)", pB, dB, 1, 0.35f, 0.4f, true),
            ("Oleada 4.8 (1Esc)", pEsc, dEsc, 1, 0.35f, 0.4f, true),
            ("Oleada 4.9 (2C)", pC, dC, 2, 0.35f, 0.4f, true),
            ("Oleada 4.10 (2C)", pC, dC, 2, 0.35f, 0.4f, false),
            ("Oleada 5.1 (2C)", pC, dC, 2, 0.35f, 2.5f, true),
            ("Oleada 5.2 (2Cor)", pCor, dCor, 2, 0.35f, 0.4f, true),
            ("Oleada 5.3 (2B)", pB, dB, 2, 0.35f, 0.4f, true),
            ("Oleada 5.4 (2Esc)", pEsc, dEsc, 2, 0.35f, 0.4f, true),
            ("Oleada 5.5 (2C)", pC, dC, 2, 0.35f, 0.4f, true),
            ("Oleada 5.6 (2Cor)", pCor, dCor, 2, 0.35f, 0.4f, true),
            ("Oleada 5.7 (2B)", pB, dB, 2, 0.35f, 0.4f, true),
            ("Oleada 5.8 (1Esc)", pEsc, dEsc, 1, 0.35f, 0.4f, true),
            ("Oleada 5.9 (2C)", pC, dC, 2, 0.35f, 0.4f, true),
            ("Oleada 5.10 (1Cor)", pCor, dCor, 1, 0.35f, 0.4f, true),
            ("Oleada 5.11 (2C)", pC, dC, 2, 0.35f, 0.4f, true),
            ("Oleada 5.12 (2C)", pC, dC, 2, 0.35f, 0.4f, false),
            ("Oleada 6.1 (2C)", pC, dC, 2, 0.35f, 2.5f, true),
            ("Oleada 6.2 (2Cor)", pCor, dCor, 2, 0.35f, 0.4f, true),
            ("Oleada 6.3 (2B)", pB, dB, 2, 0.35f, 0.4f, true),
            ("Oleada 6.4 (2Esc)", pEsc, dEsc, 2, 0.35f, 0.4f, true),
            ("Oleada 6.5 (2C)", pC, dC, 2, 0.35f, 0.4f, true),
            ("Oleada 6.6 (2Cor)", pCor, dCor, 2, 0.35f, 0.4f, true),
            ("Oleada 6.7 (2B)", pB, dB, 2, 0.35f, 0.4f, true),
            ("Oleada 6.8 (2Esc)", pEsc, dEsc, 2, 0.35f, 0.4f, true),
            ("Oleada 6.9 (2C)", pC, dC, 2, 0.35f, 0.4f, true),
            ("Oleada 6.10 (2Cor)", pCor, dCor, 2, 0.35f, 0.4f, true),
            ("Oleada 6.11 (1B)", pB, dB, 1, 0.35f, 0.4f, true),
            ("Oleada 6.12 (2C)", pC, dC, 2, 0.35f, 0.4f, true),
            ("Oleada 6.13 (2C)", pC, dC, 2, 0.35f, 0.4f, true),
            ("Oleada 6.14 (2C)", pC, dC, 2, 0.35f, 0.4f, false),
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

        // Sin sprite de campana automatica: bridge spawnea al matar al boss
        wmSo.FindProperty("spriteCampanaSagrada").objectReferenceValue = null;
        wmSo.ApplyModifiedProperties();

        Debug.Log($"[N4] ✅ WaveManager intro con {waves.Count} sub-oleadas (12 enemigos totales)");
    }

    [MenuItem(MENU + "N4.6 Spawn Boss Madre + HealthBar + Bridge", priority = 45)]
    public static void SpawnBossMadreYHealthBar()
    {
        OpenScene();

        FixBossMadrePrefab();

        // Limpiar existentes
        GameObject existing = GameObject.Find("BossMadre");
        if (existing != null) Object.DestroyImmediate(existing);

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_MADRE);
        if (prefab == null) { Debug.LogError("[N4] No existe MomiaMadre.prefab"); return; }

        GameObject bossGo = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        bossGo.name = "BossMadre";
        bossGo.transform.position = new Vector3(0f, -1f, 0f);
        bossGo.SetActive(false);

        MomiaMadre mm = bossGo.GetComponent<MomiaMadre>();

        // Boss Health Bar en canvas
        GameObject canvas = GameObject.Find("GameUI_Canvas");
        if (canvas == null) { Debug.LogError("[N4] No GameUI_Canvas"); return; }
        Transform exBar = canvas.transform.Find("BossHealthBar");
        if (exBar != null) Object.DestroyImmediate(exBar.gameObject);

        // Reusar el builder de FaseD_Nivel2 (ya es publico no, usa reflection o duplicar)
        BossHealthBar bar = BuildBossHealthBarMadre(canvas.transform);

        // Bridge
        GameObject exBridge = GameObject.Find("BossBridge");
        if (exBridge != null) Object.DestroyImmediate(exBridge);
        GameObject bridgeGo = new GameObject("BossBridge");
        BossBridge bridge = bridgeGo.AddComponent<BossBridge>();
        bridge.boss = mm;
        bridge.bar = bar;
        bridge.bossName = "MOMIA MADRE";
        bridge.bellSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PATH_BELL_SPRITE);
        // Musica boss: asignar directamente musicBossMomiaMadre del AudioManager
        AudioManager amGo = Object.FindFirstObjectByType<AudioManager>();
        if (amGo != null && amGo.musicBossMomiaMadre != null)
            bridge.musicBoss = amGo.musicBossMomiaMadre;

        WaveManager wm = Object.FindFirstObjectByType<WaveManager>();
        if (wm != null)
        {
            int count = wm.OnAllWavesCompleted.GetPersistentEventCount();
            for (int i = count - 1; i >= 0; i--) UnityEventTools.RemovePersistentListener(wm.OnAllWavesCompleted, i);
            UnityEventTools.AddPersistentListener(wm.OnAllWavesCompleted, bridge.AparicionBoss);

            SerializedObject wmSo = new SerializedObject(wm);
            wmSo.FindProperty("spriteCampanaSagrada").objectReferenceValue = null;
            wmSo.ApplyModifiedProperties();
        }

        bar.gameObject.SetActive(false);
        Debug.Log("[N4] ✅ Boss Madre + Bar + Bridge configurados");
    }

    private static void FixBossMadrePrefab()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(PATH_PREFAB_MADRE);
        try
        {
            MomiaMadre mm = root.GetComponent<MomiaMadre>();
            if (mm == null) mm = root.AddComponent<MomiaMadre>();

            EnemyData d = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_MADRE);
            GameObject miniPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_CAMINANTE);
            EnemyData miniData = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_CAMINANTE);
            Transform puntoAtaque = root.transform.Find("PuntoAtaque");

            SerializedObject so = new SerializedObject(mm);
            so.FindProperty("data").objectReferenceValue = d;
            so.FindProperty("prefabMiniMomia").objectReferenceValue = miniPrefab;
            so.FindProperty("dataMiniMomia").objectReferenceValue = miniData;
            so.FindProperty("miniMomiasPorInvocar").intValue = 2;
            so.FindProperty("cadenciaInvocar").floatValue = 8f;
            if (puntoAtaque != null) so.FindProperty("puntoAtaque").objectReferenceValue = puntoAtaque;
            so.ApplyModifiedProperties();

            // Ajustar RB2D para que caiga bien
            Rigidbody2D rb = root.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = root.AddComponent<Rigidbody2D>();
                rb.gravityScale = 3f;
                rb.freezeRotation = true;
            }
            else
            {
                if (rb.gravityScale <= 0f) rb.gravityScale = 3f;
                rb.freezeRotation = true;
            }

            PrefabUtility.SaveAsPrefabAsset(root, PATH_PREFAB_MADRE);
            Debug.Log($"[N4] FixBossMadre: data+mini+punto wired, rb grav={rb.gravityScale}");
        }
        finally { PrefabUtility.UnloadPrefabContents(root); }
    }

    private static readonly Color DORADO = new Color(1f, 0.86f, 0.33f, 1f);

    private static BossHealthBar BuildBossHealthBarMadre(Transform canvasParent)
    {
        // Duplicado del builder de FaseD pero con nombre especifico
        GameObject go = new GameObject("BossHealthBar", typeof(RectTransform), typeof(CanvasGroup));
        go.transform.SetParent(canvasParent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0f); rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = new Vector2(0f, 60f);
        rt.sizeDelta = new Vector2(900f, 90f);
        CanvasGroup cg = go.GetComponent<CanvasGroup>();

        var nombreGo = new GameObject("Nombre", typeof(RectTransform));
        nombreGo.transform.SetParent(go.transform, false);
        var tmp = nombreGo.AddComponent<TMPro.TextMeshProUGUI>();
        tmp.text = "MOMIA MADRE"; tmp.fontSize = 32; tmp.fontStyle = TMPro.FontStyles.Bold;
        tmp.color = DORADO; tmp.alignment = TMPro.TextAlignmentOptions.Center;
        var nrt = tmp.rectTransform;
        nrt.anchorMin = new Vector2(0, 1); nrt.anchorMax = new Vector2(1, 1); nrt.pivot = new Vector2(0.5f, 1);
        nrt.anchoredPosition = new Vector2(0, 0); nrt.sizeDelta = new Vector2(0, 40);

        var bgGo = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
        bgGo.transform.SetParent(go.transform, false);
        var bgImg = bgGo.GetComponent<UnityEngine.UI.Image>();
        bgImg.color = new Color(0.08f, 0.04f, 0.04f, 0.9f);
        var bgRt = bgGo.GetComponent<RectTransform>();
        bgRt.anchorMin = new Vector2(0, 0); bgRt.anchorMax = new Vector2(1, 0); bgRt.pivot = new Vector2(0.5f, 0);
        bgRt.anchoredPosition = Vector2.zero; bgRt.sizeDelta = new Vector2(0, 32);

        var whiteGo = new GameObject("DanioRecibido", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
        whiteGo.transform.SetParent(bgGo.transform, false);
        var whiteImg = whiteGo.GetComponent<UnityEngine.UI.Image>();
        whiteImg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        whiteImg.color = Color.white;
        whiteImg.type = UnityEngine.UI.Image.Type.Filled;
        whiteImg.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
        whiteImg.fillAmount = 1f;
        var wrt = whiteGo.GetComponent<RectTransform>();
        wrt.anchorMin = Vector2.zero; wrt.anchorMax = Vector2.one;
        wrt.offsetMin = new Vector2(4, 4); wrt.offsetMax = new Vector2(-4, -4);

        var goldGo = new GameObject("Vida", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
        goldGo.transform.SetParent(bgGo.transform, false);
        var goldImg = goldGo.GetComponent<UnityEngine.UI.Image>();
        goldImg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        goldImg.color = DORADO;
        goldImg.type = UnityEngine.UI.Image.Type.Filled;
        goldImg.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
        goldImg.fillAmount = 1f;
        var grt = goldGo.GetComponent<RectTransform>();
        grt.anchorMin = Vector2.zero; grt.anchorMax = Vector2.one;
        grt.offsetMin = new Vector2(4, 4); grt.offsetMax = new Vector2(-4, -4);

        BossHealthBar bar = go.AddComponent<BossHealthBar>();
        SerializedObject bso = new SerializedObject(bar);
        bso.FindProperty("group").objectReferenceValue = cg;
        bso.FindProperty("rootRect").objectReferenceValue = rt;
        bso.FindProperty("barraVida").objectReferenceValue = goldImg;
        bso.FindProperty("barraDanoRecibido").objectReferenceValue = whiteImg;
        bso.FindProperty("nombreText").objectReferenceValue = tmp;
        bso.ApplyModifiedProperties();
        return bar;
    }
}
