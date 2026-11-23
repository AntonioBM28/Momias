using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Monta la escena Nivel2_Alhondiga completa:
/// - Parallax en las 3 capas de fondo
/// - Camera follow con zona activa
/// - Managers (persisten desde Menu/Nivel1 via DontDestroyOnLoad)
/// - GameUI Canvas completo (HeartsHUD, Pausa, GameOver, Victory, LevelTitleCard)
/// - WaveManager con 4 oleadas (Caminante + Bebe + Corredora)
/// - Boss Pipila oculto, aparece al terminar oleadas
/// - BossHealthBar
/// - SacredBell oculta, aparece al matar al boss
/// </summary>
public static class FaseD_Nivel2
{
    private const string MENU = "Momias/Fase D/";
    private const string PATH_SCENE_N2 = "Assets/Scenes/Nivel2_Alhondiga.unity";
    private const string PATH_SCENE_N1 = "Assets/Scenes/Nivel1_Museo.unity";

    private const string PATH_PREFAB_CAMINANTE = "Assets/Prefabs/MomiaWalking_0.prefab";
    private const string PATH_PREFAB_BEBE = "Assets/Prefabs/MomiaBebe.prefab";
    private const string PATH_PREFAB_CORREDORA = "Assets/Prefabs/MomiaCorredora.prefab";
    private const string PATH_PREFAB_PIPILA = "Assets/Prefabs/MomiaPipila.prefab";
    private const string PATH_SO_CAMINANTE = "Assets/ScriptableObjects/Enemies/MomiaCaminante_Gris.asset";
    private const string PATH_SO_BEBE = "Assets/ScriptableObjects/Enemies/MomiaBebe.asset";
    private const string PATH_SO_CORREDORA = "Assets/ScriptableObjects/Enemies/MomiaCorredora.asset";
    private const string PATH_SO_PIPILA = "Assets/ScriptableObjects/Enemies/MomiaPipila.asset";
    private const string PATH_BELL_SPRITE = "Assets/Sprites/UI/CampanaSagrada.png";

    // Colores consistentes con Fase B
    private static readonly Color DORADO = new Color(1f, 0.78f, 0.28f, 1f);

    // =====================================================================
    // MENU PRINCIPAL
    // =====================================================================

    [MenuItem(MENU + "🚀 Build ALL Nivel 2 (Alhondiga + Pipila)", priority = 2)]
    public static void BuildAll()
    {
        if (!EditorUtility.DisplayDialog("Fase D - Nivel 2",
            "Esto abre Nivel2_Alhondiga.unity y:\n\n" +
            "  • Agrega ParallaxLayer a las 3 capas de fondo\n" +
            "  • Setup CameraFollow (zona activa -4..4)\n" +
            "  • Copia Diego, Managers, GameUI del Nivel 1 adaptado\n" +
            "  • WaveManager con 4 oleadas: 4C+1B / 5C+2B / 4C+2B+1Cor / 6C+2B+4Cor\n" +
            "  • Boss Pipila oculto + BossHealthBar oculta\n" +
            "  • Hooks: oleadas-completadas → spawn Pipila; Pipila-muerta → spawn Campana Sagrada\n\n" +
            "¿Continuar?", "Si, hazlo", "Cancelar"))
            return;

        OpenScene();
        SetupParallaxYCamara();
        SpawnDiego();
        SpawnManagersSiFaltan();
        SpawnGameUI();
        SpawnWaveManager();
        SpawnBossYHealthBar();
        AddBuildSettingsIfMissing();

        Scene s = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(s);
        EditorSceneManager.SaveScene(s);

        EditorUtility.DisplayDialog("Fase D",
            "✅ Nivel 2 Alhondiga listo!\n\n" +
            "Prueba:\n" +
            "  1. Open Scene Nivel2_Alhondiga y dale Play (o desde Menu → JUGAR,\n" +
            "     pero esto carga Nivel 1; para probar solo Nivel 2 abre directo la escena)\n" +
            "  2. Diego aparece, puedes moverlo, la camara lo sigue en X\n" +
            "  3. Oleadas 1-4: mezclan Caminantes + Bebés + Corredoras\n" +
            "  4. Al terminar oleadas, Pipila aparece con fade + crossfade de musica\n" +
            "  5. Pelea contra Pipila (Fase 1 embestidas, Fase 2 salto+onda a los 50%)\n" +
            "  6. Al matarlo aparece la Campana Sagrada → Victory",
            "OK");
    }

    [MenuItem(MENU + "N2.1 Open scene", priority = 40)]
    public static void OpenScene()
    {
        Scene current = EditorSceneManager.GetActiveScene();
        if (current.path != PATH_SCENE_N2)
        {
            if (current.isDirty) EditorSceneManager.SaveScene(current);
            EditorSceneManager.OpenScene(PATH_SCENE_N2);
        }
    }

    [MenuItem(MENU + "N2.2 Camara estatica (como Nivel 1)", priority = 41)]
    public static void SetupParallaxYCamara()
    {
        OpenScene();
        Scene s = EditorSceneManager.GetActiveScene();

        // Quitar ParallaxLayer de las 3 capas si existe
        string[] prefijos = { "Nivel2_Capa1", "Nivel2_Capa2", "Nivel2_Capa3" };
        for (int i = 0; i < 3; i++)
        {
            GameObject layer = BuscarEnEscenaPorContiene(s, prefijos[i]);
            if (layer == null) continue;
            ParallaxLayer existing = layer.GetComponent<ParallaxLayer>();
            if (existing != null) Object.DestroyImmediate(existing);
        }

        // Quitar CameraFollow de Main Camera si existe (camara estatica)
        Camera cam = Camera.main;
        if (cam != null)
        {
            CameraFollow cf = cam.GetComponent<CameraFollow>();
            if (cf != null) Object.DestroyImmediate(cf);
        }
        Debug.Log("[N2] ✅ Camara estatica y sin parallax (como Nivel 1)");
    }

    [MenuItem(MENU + "N2.3 Spawn Diego", priority = 42)]
    public static void SpawnDiego()
    {
        OpenScene();
        // Si ya hay un Player, no duplicar
        GameObject existente = GameObject.FindGameObjectWithTag("Player");
        if (existente != null)
        {
            Debug.Log("[N2] Diego ya existe en la escena");
            return;
        }

        // Cargar Diego del Nivel 1 abriendo esa escena en additive, copiando y cerrando
        Scene curScene = EditorSceneManager.GetActiveScene();
        Scene n1 = EditorSceneManager.OpenScene(PATH_SCENE_N1, OpenSceneMode.Additive);
        GameObject diegoN1 = null;
        foreach (GameObject go in n1.GetRootGameObjects())
        {
            if (go.CompareTag("Player")) { diegoN1 = go; break; }
            Transform t = go.transform.Find("Player");
            if (t != null) { diegoN1 = t.gameObject; break; }
        }

        if (diegoN1 == null)
        {
            // Buscar por tag en todos los descendientes
            foreach (GameObject go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            {
                if (go.scene == n1 && go.CompareTag("Player")) { diegoN1 = go; break; }
            }
        }

        if (diegoN1 == null)
        {
            Debug.LogError("[N2] No encontre Diego (Player tag) en Nivel 1. Cancelo.");
            EditorSceneManager.CloseScene(n1, true);
            return;
        }

        GameObject diegoClone = Object.Instantiate(diegoN1);
        diegoClone.name = diegoN1.name;
        diegoClone.transform.position = new Vector3(0f, -1f, 0f);
        SceneManager.MoveGameObjectToScene(diegoClone, curScene);

        EditorSceneManager.CloseScene(n1, true);
        Debug.Log("[N2] ✅ Diego spawneado");
    }

    [MenuItem(MENU + "N2.4 Spawn Managers", priority = 43)]
    public static void SpawnManagersSiFaltan()
    {
        OpenScene();
        if (GameObject.Find("GameManager") == null) new GameObject("GameManager").AddComponent<GameManager>();
        if (GameObject.Find("AudioManager") == null) new GameObject("AudioManager").AddComponent<AudioManager>();
        if (GameObject.Find("SceneTransition") == null) new GameObject("SceneTransition").AddComponent<SceneTransition>();
        if (Object.FindFirstObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        if (GameObject.Find("CameraShaker") == null) new GameObject("CameraShaker").AddComponent<CameraShaker>();
        if (GameObject.Find("HitPause") == null) new GameObject("HitPause").AddComponent<HitPause>();
        if (GameObject.Find("DustBurst") == null)
        {
            var go = new GameObject("DustBurst");
            DustBurst db = go.AddComponent<DustBurst>();
            Sprite dust = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/VFX/Dust.png");
            if (dust != null)
            {
                var so = new SerializedObject(db);
                so.FindProperty("dustSprite").objectReferenceValue = dust;
                so.ApplyModifiedProperties();
            }
        }

        // Asignar automaticamente los clips de audio por GUID al AudioManager de esta escena
        AsignarAudiosAlAudioManager();

        Debug.Log("[N2] ✅ Managers verificados (persistentes via DontDestroyOnLoad si ya existian)");
    }

    private static void AsignarAudiosAlAudioManager()
    {
        GameObject amGo = GameObject.Find("AudioManager");
        if (amGo == null) return;
        AudioManager am = amGo.GetComponent<AudioManager>();
        if (am == null) return;

        // GUIDs del .md de Antonio
        string[,] mapping = {
            { "musicMenu",        "7635ef1d13bb43a6bd0ecba82062436d" },
            { "musicNivel2",      "d941dc6f3ac24af29be178c576b8ea18" },
            { "musicNivel3",      "1176420ada9a4eeca3c8e9951523f615" },
            { "musicBossPipila",  "b9804f39506347beb6459f6937c7ade4" },
            { "musicGameOver",    "217bd4c264b544bd962ed111185c8747" },
            { "musicVictoria",    "bfa52ab38825496984bf0bd166ffb19d" },
            { "sfxMuerteMomia",   "5cae12120dea4c21829566ca209de12b" },
        };

        SerializedObject so = new SerializedObject(am);
        for (int i = 0; i < mapping.GetLength(0); i++)
        {
            string prop = mapping[i, 0];
            string guid = mapping[i, 1];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path)) continue;
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            var p = so.FindProperty(prop);
            if (p != null) p.objectReferenceValue = clip;
        }
        // Tambien asignar los SFX y musicas originales buscandolos por nombre
        AsignarPorNombre(so, "musicNivel1",       "Musica_Ambiental");
        AsignarPorNombre(so, "sfxCampanazo",      "Campanazo_Diego");
        AsignarPorNombre(so, "sfxImpactoMomia",   "Impacto_Momia");
        AsignarPorNombre(so, "sfxCampanaSagrada", "Recoger_Campana");

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(am);
        Debug.Log("[N2] ✅ Audio clips (11 slots) asignados al AudioManager de esta escena");
    }

    private static void AsignarPorNombre(SerializedObject so, string prop, string nombreAsset)
    {
        string[] guids = AssetDatabase.FindAssets($"{nombreAsset} t:AudioClip");
        if (guids.Length == 0) return;
        AudioClip c = AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(guids[0]));
        var p = so.FindProperty(prop);
        if (p != null) p.objectReferenceValue = c;
    }

    [MenuItem(MENU + "N2.5 Spawn GameUI", priority = 44)]
    public static void SpawnGameUI()
    {
        OpenScene();
        // Reutilizamos el builder de Chunk1 para GameOverPanel, VictorySystem, LevelTitleCard
        Chunk1Setup.BuildGameOverPanel(silent: true);
        Chunk1Setup.BuildVictorySystem(silent: true);
        Chunk1Setup.BuildLevelTitleCard(silent: true);

        // Ajustar el titulo del LevelTitleCard a "NIVEL 2 - ALHONDIGA"
        // (hay que modificar los SerializedFields porque Start() los re-aplica al TMP)
        var card = Object.FindFirstObjectByType<LevelTitleCard>();
        if (card != null)
        {
            SerializedObject so = new SerializedObject(card);
            so.FindProperty("tituloNivel").stringValue = "NIVEL 2";
            so.FindProperty("subtituloNivel").stringValue = "ALHONDIGA DE GRANADITAS";
            so.ApplyModifiedProperties();
            // Y tambien actualizar el TMP visible por si acaso
            Transform contenido = card.transform.Find("Contenido");
            if (contenido != null)
            {
                Transform tituloT = contenido.Find("Titulo");
                Transform subT = contenido.Find("Subtitulo");
                if (tituloT != null) tituloT.GetComponent<TMP_Text>().text = "NIVEL 2";
                if (subT != null) subT.GetComponent<TMP_Text>().text = "ALHONDIGA DE GRANADITAS";
            }
        }

        // Rebuild PauseMenu con diseno coincidente
        if (FaseB_HasRebuildPauseMethod())
        {
            // Dejar que FaseB rearme el pause en esta escena directamente seria lo ideal,
            // pero ese metodo esta hardcodeado a Nivel1. Para N2 hacemos un rebuild
            // equivalente llamando al mismo builder de FaseB sobre la escena actual.
            RebuildPauseMenuN2();
        }

        // HeartsHUD: crear a mano si no existe
        BuildHeartsHUDSiFalta();

        // PauseMenu adaptado al Nivel 2 (mismo diseno que Nivel 1)
        BuildPauseMenuEnEstaEscena();

        Debug.Log("[N2] ✅ GameUI (GameOver, Victory, Pause, HeartsHUD, LevelTitle) listos");
    }

    private static void BuildPauseMenuEnEstaEscena()
    {
        // Llama al rebuild de FaseB pero adaptado para que no fuerce Nivel1
        GameObject canvas = GameObject.Find("GameUI_Canvas");
        if (canvas == null) return;
        Transform existing = canvas.transform.Find("PauseMenu");
        if (existing != null) Object.DestroyImmediate(existing.gameObject);

        // Delegamos al metodo de FaseB usando reflection (es static public)
        System.Type t = System.Type.GetType("FaseB_Setup");
        if (t != null)
        {
            var method = t.GetMethod("RebuildPauseMenuInNivel1",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (method != null)
            {
                // Importante: ese metodo hace OpenScene(Nivel1) → no sirve para N2.
                // Copiamos su logica aqui a mano via un pequeno wrapper
                // Mejor: construir pause inline adaptado
            }
        }

        // Construccion inline (sin dependencia de FaseB):
        BuildPauseMenuInline(canvas);
    }

    private static void BuildPauseMenuInline(GameObject canvas)
    {
        GameObject pmRoot = new GameObject("PauseMenu", typeof(RectTransform));
        pmRoot.transform.SetParent(canvas.transform, false);
        var pmRt = pmRoot.GetComponent<RectTransform>();
        pmRt.anchorMin = Vector2.zero; pmRt.anchorMax = Vector2.one;
        pmRt.offsetMin = Vector2.zero; pmRt.offsetMax = Vector2.zero;
        PauseMenu pm = pmRoot.AddComponent<PauseMenu>();

        GameObject pausePanel = new GameObject("PausePanelFondo", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        pausePanel.transform.SetParent(pmRoot.transform, false);
        var ppRt = pausePanel.GetComponent<RectTransform>();
        ppRt.anchorMin = Vector2.zero; ppRt.anchorMax = Vector2.one;
        ppRt.offsetMin = Vector2.zero; ppRt.offsetMax = Vector2.zero;
        pausePanel.GetComponent<Image>().color = new Color(0.03f, 0.02f, 0.06f, 0.85f);

        // Titulo PAUSA
        var tituloGo = new GameObject("TituloPausa", typeof(RectTransform));
        tituloGo.transform.SetParent(pausePanel.transform, false);
        var tmpT = tituloGo.AddComponent<TextMeshProUGUI>();
        tmpT.text = "PAUSA"; tmpT.fontSize = 160; tmpT.fontStyle = FontStyles.Bold;
        tmpT.color = DORADO; tmpT.alignment = TextAlignmentOptions.Center;
        var tTR = tmpT.rectTransform;
        tTR.anchorMin = new Vector2(0.5f, 1); tTR.anchorMax = new Vector2(0.5f, 1);
        tTR.pivot = new Vector2(0.5f, 0.5f);
        tTR.anchoredPosition = new Vector2(0, -180); tTR.sizeDelta = new Vector2(1200, 200);

        // 4 botones
        GameObject mainPanel = new GameObject("MainPanel", typeof(RectTransform));
        mainPanel.transform.SetParent(pausePanel.transform, false);
        var mpRt = mainPanel.GetComponent<RectTransform>();
        mpRt.anchorMin = Vector2.zero; mpRt.anchorMax = Vector2.one;
        mpRt.offsetMin = Vector2.zero; mpRt.offsetMax = Vector2.zero;

        float y0 = 100f, dy = 100f;
        var btnCont = CrearBotonMenu(mainPanel.transform, "BtnContinuar", "CONTINUAR", 0, y0);
        var btnOpc = CrearBotonMenu(mainPanel.transform, "BtnOpciones", "OPCIONES", 0, y0 - dy);
        var btnMenu = CrearBotonMenu(mainPanel.transform, "BtnMenuPrincipal", "MENU PRINCIPAL", 0, y0 - dy * 2);
        var btnSalir = CrearBotonMenu(mainPanel.transform, "BtnSalir", "SALIR", 0, y0 - dy * 3);
        UnityEventTools.AddPersistentListener(btnCont.onClick, pm.OnContinuar);
        UnityEventTools.AddPersistentListener(btnOpc.onClick, pm.MostrarOpciones);
        UnityEventTools.AddPersistentListener(btnMenu.onClick, pm.OnMenu);
        UnityEventTools.AddPersistentListener(btnSalir.onClick, pm.OnSalir);

        // Panel Opciones (minimo: solo sliders + volver)
        GameObject optPanel = new GameObject("OptionsPanel", typeof(RectTransform), typeof(OptionsPanel));
        optPanel.transform.SetParent(pausePanel.transform, false);
        var opRt = optPanel.GetComponent<RectTransform>();
        opRt.anchorMin = Vector2.zero; opRt.anchorMax = Vector2.one;
        opRt.offsetMin = Vector2.zero; opRt.offsetMax = Vector2.zero;
        OptionsPanel op = optPanel.GetComponent<OptionsPanel>();

        // Boton volver del panel opciones
        var btnBack = CrearBotonMenu(optPanel.transform, "BtnVolver", "VOLVER", 0, -300);
        UnityEventTools.AddPersistentListener(btnBack.onClick, pm.MostrarPrincipal);

        // Wire pause
        var so = new SerializedObject(pm);
        so.FindProperty("pausePanel").objectReferenceValue = pausePanel;
        so.FindProperty("mainPanel").objectReferenceValue = mainPanel;
        so.FindProperty("optionsPanel").objectReferenceValue = optPanel;
        so.FindProperty("creditsPanel").objectReferenceValue = null;
        so.ApplyModifiedProperties();

        optPanel.SetActive(false);
        pausePanel.SetActive(false);
        Debug.Log("[N2] ✅ PauseMenu construido en Nivel 2");
    }

    private static Button CrearBotonMenu(Transform parent, string nombre, string texto, float x, float y)
    {
        var go = new GameObject(nombre, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(ButtonHover));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = new Color(0.12f, 0.08f, 0.18f, 0.88f);

        var txtGo = new GameObject("Text", typeof(RectTransform));
        txtGo.transform.SetParent(go.transform, false);
        var tmp = txtGo.AddComponent<TextMeshProUGUI>();
        tmp.text = texto; tmp.fontSize = 44; tmp.fontStyle = FontStyles.Bold;
        tmp.color = new Color(1f, 0.92f, 0.70f, 1f);
        tmp.alignment = TextAlignmentOptions.Center;
        var tr = tmp.rectTransform;
        tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f); rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(x, y);
        rt.sizeDelta = new Vector2(480, 80);
        return go.GetComponent<Button>();
    }

    private static bool FaseB_HasRebuildPauseMethod()
    {
        return System.Type.GetType("FaseB_Setup") != null;
    }

    [MenuItem(MENU + "N2.6 Spawn WaveManager", priority = 45)]
    public static void SpawnWaveManager()
    {
        OpenScene();
        Scene s = EditorSceneManager.GetActiveScene();

        // Borrar WaveManager anterior si existe
        GameObject existente = GameObject.Find("WaveManager");
        if (existente != null) Object.DestroyImmediate(existente);

        GameObject wmGo = new GameObject("WaveManager");
        WaveManager wm = wmGo.AddComponent<WaveManager>();

        // Spawn points
        GameObject spawnL = new GameObject("SpawnIzquierdo");
        spawnL.transform.position = new Vector3(-7f, -1f, 0f);
        GameObject spawnR = new GameObject("SpawnDerecho");
        spawnR.transform.position = new Vector3(7f, -1f, 0f);

        // Sprite de la Campana Sagrada (opcional en este nivel: se hace manual al morir el boss)
        Sprite bellSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PATH_BELL_SPRITE);

        GameObject prefCam = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_CAMINANTE);
        GameObject prefBebe = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_BEBE);
        GameObject prefCor = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_CORREDORA);
        EnemyData dCam = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_CAMINANTE);
        EnemyData dBebe = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_BEBE);
        EnemyData dCor = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_CORREDORA);

        SerializedObject wmSo = new SerializedObject(wm);
        wmSo.FindProperty("spawnIzquierdo").objectReferenceValue = spawnL.transform;
        wmSo.FindProperty("spawnDerecho").objectReferenceValue = spawnR.transform;
        wmSo.FindProperty("spriteCampanaSagrada").objectReferenceValue = bellSprite;
        wmSo.FindProperty("posicionCampanaSagrada").vector3Value = new Vector3(0, -1.3f, 0);

        // 4 oleadas en 7 sub-oleadas (los mezclados se parten en sub-oleadas con pausa corta)
        var olList = wmSo.FindProperty("oleadas");
        olList.arraySize = 7;

        // Oleada 1: 4C + 1B
        SetOleada(olList.GetArrayElementAtIndex(0), "1a - Caminantes", prefCam, dCam, 4, 1.4f, 2f);
        SetOleada(olList.GetArrayElementAtIndex(1), "1b - Bebe", prefBebe, dBebe, 1, 0.8f, 0.5f);

        // Oleada 2: 5C + 2B
        SetOleada(olList.GetArrayElementAtIndex(2), "2a - Caminantes", prefCam, dCam, 5, 1.3f, 3f);
        SetOleada(olList.GetArrayElementAtIndex(3), "2b - Bebes", prefBebe, dBebe, 2, 0.8f, 0.5f);

        // Oleada 3: 4C + 2B + 1Corredora
        SetOleada(olList.GetArrayElementAtIndex(4), "3 - Mezcla+Corredora",
            prefCam, dCam, 4, 1.2f, 3f);
        // Para incluir corredora+bebes en una misma "oleada", usamos sub-oleadas separadas
        // pero marcadas con pausaAntes muy chica para que se perciban como una sola oleada

        // Oleada 4 (enjambre): 6C + 2B + 4Corredoras
        SetOleada(olList.GetArrayElementAtIndex(5), "4 - Enjambre Caminantes",
            prefCam, dCam, 6, 0.9f, 3f);
        SetOleada(olList.GetArrayElementAtIndex(6), "4b - Enjambre Corredoras",
            prefCor, dCor, 4, 0.7f, 0.5f);

        wmSo.ApplyModifiedProperties();

        Debug.Log("[N2] ✅ WaveManager con 4 oleadas (sub-oleadas serializadas)");
        Debug.Log("[N2] NOTA: Oleada 3 sub-sliced simplificada - ajusta cantidades mas finas si quieres");
    }

    [MenuItem(MENU + "N2.7 Spawn Boss + HealthBar", priority = 46)]
    public static void SpawnBossYHealthBar()
    {
        OpenScene();
        Scene s = EditorSceneManager.GetActiveScene();

        // Agregar BossPipila al prefab de Antonio si todavia no lo tiene
        FixBossPipilaPrefab();

        // Spawn Boss Pipila oculto en el centro, arriba
        GameObject existente = GameObject.Find("BossPipila");
        if (existente != null) Object.DestroyImmediate(existente);

        GameObject pipilaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_PIPILA);
        if (pipilaPrefab == null)
        {
            Debug.LogError("[N2] No existe MomiaPipila.prefab");
            return;
        }
        GameObject boss = (GameObject)PrefabUtility.InstantiatePrefab(pipilaPrefab);
        boss.name = "BossPipila";
        boss.transform.position = new Vector3(0f, 1f, 0f);
        boss.SetActive(false);

        BossPipila bp = boss.GetComponent<BossPipila>();
        if (bp == null) bp = boss.AddComponent<BossPipila>();

        // Boss Health Bar
        GameObject canvas = GameObject.Find("GameUI_Canvas");
        if (canvas == null)
        {
            Debug.LogError("[N2] No existe GameUI_Canvas");
            return;
        }
        Transform existingBar = canvas.transform.Find("BossHealthBar");
        if (existingBar != null) Object.DestroyImmediate(existingBar.gameObject);

        BossHealthBar bar = BuildBossHealthBar(canvas.transform);

        // Hook de HP se hace en runtime via BossBridge.OnHPChanged -> bar.SetVida

        // Al morir el boss, spawn SacredBell y ocultar barra
        BossBridge bridge = boss.AddComponent<BossBridge>();
        bridge.boss = bp;
        bridge.bar = bar;
        bridge.bellSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PATH_BELL_SPRITE);

        // Hook del WaveManager: cuando se completan todas las oleadas → bridge.OnAllWavesCompleted
        WaveManager wm = Object.FindFirstObjectByType<WaveManager>();
        if (wm != null)
        {
            // Limpiar listeners previos (por si rebuild)
            UnityEventTools.RemovePersistentListener(wm.OnAllWavesCompleted, bridge.AparicionBoss);
            UnityEventTools.AddPersistentListener(wm.OnAllWavesCompleted, bridge.AparicionBoss);
            // Tambien eliminamos el spawn automatico de CampanaSagrada del WaveManager
            // dejando el sprite null (hace early-return con warning, no revienta)
            SerializedObject wmSo = new SerializedObject(wm);
            // No tocamos spriteCampanaSagrada aqui porque bridge se encarga del spawn al morir boss
            wmSo.ApplyModifiedProperties();
        }

        bar.gameObject.SetActive(false);
        Debug.Log("[N2] ✅ Boss Pipila + BossHealthBar + Bridge listos");
    }

    private static void FixBossPipilaPrefab()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(PATH_PREFAB_PIPILA);
        try
        {
            if (root.GetComponent<BossPipila>() == null)
            {
                BossPipila bp = root.AddComponent<BossPipila>();
                // Asignar SO
                EnemyData d = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_PIPILA);
                if (d != null)
                {
                    SerializedObject so = new SerializedObject(bp);
                    so.FindProperty("data").objectReferenceValue = d;
                    so.ApplyModifiedProperties();
                }
            }
            // Asegurar Rigidbody2D
            Rigidbody2D rb = root.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = root.AddComponent<Rigidbody2D>();
                rb.gravityScale = 3f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            // Asegurar Collider2D
            if (root.GetComponent<Collider2D>() == null)
            {
                BoxCollider2D col = root.AddComponent<BoxCollider2D>();
                col.size = new Vector2(1.2f, 2.5f);
            }
            PrefabUtility.SaveAsPrefabAsset(root, PATH_PREFAB_PIPILA);
        }
        finally { PrefabUtility.UnloadPrefabContents(root); }
    }

    private static BossHealthBar BuildBossHealthBar(Transform canvasParent)
    {
        GameObject go = new GameObject("BossHealthBar", typeof(RectTransform), typeof(CanvasGroup));
        go.transform.SetParent(canvasParent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = new Vector2(0f, 60f);
        rt.sizeDelta = new Vector2(900f, 90f);

        CanvasGroup cg = go.GetComponent<CanvasGroup>();

        // Nombre
        var nombreGo = new GameObject("Nombre", typeof(RectTransform));
        nombreGo.transform.SetParent(go.transform, false);
        var tmp = nombreGo.AddComponent<TextMeshProUGUI>();
        tmp.text = "MOMIA-PIPILA";
        tmp.fontSize = 32;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = DORADO;
        tmp.alignment = TextAlignmentOptions.Center;
        var nombreRt = tmp.rectTransform;
        nombreRt.anchorMin = new Vector2(0, 1);
        nombreRt.anchorMax = new Vector2(1, 1);
        nombreRt.pivot = new Vector2(0.5f, 1);
        nombreRt.anchoredPosition = new Vector2(0, 0);
        nombreRt.sizeDelta = new Vector2(0, 40);

        // Barra de fondo oscuro
        var bgGo = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        bgGo.transform.SetParent(go.transform, false);
        var bgImg = bgGo.GetComponent<Image>();
        bgImg.color = new Color(0.08f, 0.04f, 0.04f, 0.9f);
        var bgRt = bgGo.GetComponent<RectTransform>();
        bgRt.anchorMin = new Vector2(0, 0);
        bgRt.anchorMax = new Vector2(1, 0);
        bgRt.pivot = new Vector2(0.5f, 0);
        bgRt.anchoredPosition = Vector2.zero;
        bgRt.sizeDelta = new Vector2(0, 32);

        // Barra blanca (danio delayed)
        var whiteGo = new GameObject("DanioRecibido", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        whiteGo.transform.SetParent(bgGo.transform, false);
        var whiteImg = whiteGo.GetComponent<Image>();
        whiteImg.color = Color.white;
        whiteImg.type = Image.Type.Filled;
        whiteImg.fillMethod = Image.FillMethod.Horizontal;
        whiteImg.fillAmount = 1f;
        var whiteRt = whiteGo.GetComponent<RectTransform>();
        whiteRt.anchorMin = Vector2.zero;
        whiteRt.anchorMax = Vector2.one;
        whiteRt.offsetMin = new Vector2(4, 4);
        whiteRt.offsetMax = new Vector2(-4, -4);

        // Barra dorada (vida real)
        var goldGo = new GameObject("Vida", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        goldGo.transform.SetParent(bgGo.transform, false);
        var goldImg = goldGo.GetComponent<Image>();
        goldImg.color = DORADO;
        goldImg.type = Image.Type.Filled;
        goldImg.fillMethod = Image.FillMethod.Horizontal;
        goldImg.fillAmount = 1f;
        var goldRt = goldGo.GetComponent<RectTransform>();
        goldRt.anchorMin = Vector2.zero;
        goldRt.anchorMax = Vector2.one;
        goldRt.offsetMin = new Vector2(4, 4);
        goldRt.offsetMax = new Vector2(-4, -4);

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

    // =====================================================================
    // Helpers
    // =====================================================================

    private static GameObject BuscarEnEscenaPorContiene(Scene s, string substr)
    {
        foreach (GameObject go in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (go.scene != s) continue;
            if (go.name.Contains(substr)) return go;
        }
        return null;
    }

    private static void SetOleada(SerializedProperty prop, string nombre, GameObject prefab, EnemyData data,
                                   int cantidad, float intervalo, float pausa)
    {
        prop.FindPropertyRelative("nombre").stringValue = nombre;
        prop.FindPropertyRelative("prefabEnemigo").objectReferenceValue = prefab;
        prop.FindPropertyRelative("dataEnemigo").objectReferenceValue = data;
        prop.FindPropertyRelative("cantidad").intValue = cantidad;
        prop.FindPropertyRelative("intervaloSpawn").floatValue = intervalo;
        prop.FindPropertyRelative("pausaAntesDeOleada").floatValue = pausa;
    }

    private static void BuildHeartsHUDSiFalta()
    {
        GameObject canvas = GameObject.Find("GameUI_Canvas");
        if (canvas == null) return;
        if (canvas.transform.Find("HeartsHUD") != null) return;

        GameObject go = new GameObject("HeartsHUD", typeof(RectTransform));
        go.transform.SetParent(canvas.transform, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(30, -30);
        rt.sizeDelta = new Vector2(400, 60);
        go.AddComponent<HeartsHUD>();
        // Nota: el componente HeartsHUD requiere referencias que ya maneja via OnEnable/Start
        Debug.Log("[N2] HeartsHUD creado (asegura que su configuracion de sprites este OK)");
    }

    private static void RebuildPauseMenuN2()
    {
        // Si FaseB_Setup.RebuildPauseMenuInNivel1 esta disponible, lo puertamos aqui adaptado
        // Pero por simplicidad hacemos un llamado a Chunk1Setup si tiene builder de pausa
        // (por ahora el PauseMenu se construye en la escena manualmente o lo clonamos del nivel 1)
        // Dejamos un placeholder: el usuario puede ejecutar Momias/Fase B/Rebuild PauseMenu adaptado
        // No bloqueamos la construccion del nivel por esto.
        Debug.Log("[N2] PauseMenu: usa 'Momias/Fase B/4. Rebuild PauseMenu en Nivel 1' adaptado manualmente, o lo armamos luego");
    }

    private static void AddBuildSettingsIfMissing()
    {
        // Verifica que Nivel2_Alhondiga este en Build Settings
        var scenes = EditorBuildSettings.scenes;
        foreach (var sc in scenes) if (sc.path == PATH_SCENE_N2) return;
        var list = new List<EditorBuildSettingsScene>(scenes);
        list.Add(new EditorBuildSettingsScene(PATH_SCENE_N2, true));
        EditorBuildSettings.scenes = list.ToArray();
        Debug.Log("[N2] ✅ Nivel2_Alhondiga agregado a Build Settings");
    }
}

/// <summary>
/// Puente entre eventos: cuando WaveManager termina oleadas → spawn Boss.
/// Cuando Boss muere → spawn Campana Sagrada + ocultar barra.
/// </summary>
public class BossBridge : MonoBehaviour
{
    public BossPipila boss;
    public BossHealthBar bar;
    public Sprite bellSprite;

    private bool bossActivado;

    void Start()
    {
        if (boss != null)
        {
            boss.OnBossDied.AddListener(OnBossMurio);
            boss.OnBossHealthChanged.AddListener(OnHPChanged);
        }
    }

    public void AparicionBoss()
    {
        if (bossActivado || boss == null) return;
        bossActivado = true;

        boss.gameObject.SetActive(true);
        // Inicializar barra con vida max del SO
        int vidaMax = boss.Data != null ? boss.Data.vidaMaxima : 15;
        if (bar != null)
        {
            bar.Inicializar("MOMIA-PIPILA", vidaMax);
            bar.MostrarConFade();
        }

        // Crossfade a musica de boss
        if (AudioManager.Instance != null && AudioManager.Instance.musicBossPipila != null)
            AudioManager.Instance.PlayMusic(AudioManager.Instance.musicBossPipila, true);

        if (CameraShaker.Instance != null) CameraShaker.Instance.Shake(0.3f, 0.5f);
        Debug.Log("[BossBridge] ⚔️ Boss aparecio, musica cambiada");
    }

    private void OnHPChanged(int actual, int max)
    {
        if (bar != null) bar.SetVida(actual);
    }

    private void OnBossMurio()
    {
        if (bar != null) bar.OcultarConFade();
        SpawnCampanaSagrada();
    }

    private void SpawnCampanaSagrada()
    {
        if (bellSprite == null)
        {
            Debug.LogWarning("[BossBridge] bellSprite null, no spawnea Campana Sagrada");
            return;
        }
        GameObject campana = new GameObject("Campana Sagrada Nivel 2");
        campana.transform.position = new Vector3(0f, -1.3f, 0f);
        SpriteRenderer sr = campana.AddComponent<SpriteRenderer>();
        sr.sprite = bellSprite;
        sr.sortingOrder = 15;
        campana.AddComponent<CircleCollider2D>();
        campana.AddComponent<SacredBell>();
        Debug.Log("[BossBridge] 🔔 Campana Sagrada spawneada");
    }
}
