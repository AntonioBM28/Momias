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
    private const string PATH_BELL_SPRITE = "Assets/Sprites/Items/CampanaSagrada.png";

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
        AddBuildSettingsIfMissing(PATH_SCENE_N2);

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

    [MenuItem(MENU + "🔧 FIX BOSS PIPILA (A+B+C: camara/escala/barra)", priority = 3)]
    public static void FixBossPipilaAll()
    {
        if (!EditorUtility.DisplayDialog("Fix Boss Pipila",
            "Aplica los 3 fixes del Nivel 2:\n\n" +
            "  A) Pipila se salia de camara -> vel bajada a 5, clamp cada FixedUpdate\n" +
            "  B) Pipila chica -> escala 2.3 (reimporta SO + recrea boss en escena)\n" +
            "  C) Barra no bajaba -> reasigna data al prefab, mueve suscripcion a AparicionBoss, agrega logs\n\n" +
            "Despues de esto: abre Nivel2_Alhondiga, play, pegale al boss y mira la consola.\n" +
            "Deberian salir logs [BossPipila] [BossBridge] [BossHealthBar].\n\n" +
            "¿Continuar?", "Si, aplicar", "Cancelar"))
            return;

        // 1. Reimportar SO (para que Unity vea escala 2.3)
        AssetDatabase.ImportAsset(PATH_SO_PIPILA, ImportAssetOptions.ForceUpdate);
        Debug.Log("[FixPipila] SO reimportado");

        // 2. Re-fix prefab (reasigna data + collider nuevo)
        FixBossPipilaPrefab();
        Debug.Log("[FixPipila] Prefab reparado");

        // 3. Abrir escena Nivel 2 y recrear boss+bar+bridge con la nueva escala/limites
        OpenScene();
        SpawnBossYHealthBar();

        // 4. Guardar escena
        Scene s = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(s);
        EditorSceneManager.SaveScene(s);

        EditorUtility.DisplayDialog("Fix Boss Pipila",
            "✅ Fixes aplicados.\n\n" +
            "Como probar:\n" +
            "  1. Dale PLAY (escena Nivel2_Alhondiga ya abierta).\n" +
            "  2. Elimina las oleadas rapido (o salta directo si tienes atajo) hasta que aparezca Pipila.\n" +
            "  3. Pipila debe verse MUCHO mas grande (escala 2.3x).\n" +
            "  4. Pegale con J. En consola debes ver:\n" +
            "       [BossPipila] RecibirDanio -> HP: 14/15\n" +
            "       [BossBridge] OnHPChanged recibido -> 14/15\n" +
            "       [BossHealthBar] SetVida -> 14/15 fill=0.93 barraVida=OK\n" +
            "  5. La barra DEBE bajar visualmente.\n" +
            "  6. Durante embestidas, Pipila NO debe pasarse de los limites de camara (+-3.5 world).\n\n" +
            "Si algo falla pegame el log.",
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

    [MenuItem(MENU + "N2.0 Apagar gizmos de Collider2D (quita lineas verdes)", priority = 39)]
    public static void ApagarGizmosColliders()
    {
        // Usa GizmoUtility para apagar la visualizacion de Colliders 2D en Scene+Game.
        // NO borra nada, solo oculta las lineas verdes. Reversible desde Edit > Preferences > Gizmos.
        System.Type[] tiposCollider = {
            typeof(BoxCollider2D),
            typeof(CircleCollider2D),
            typeof(CapsuleCollider2D),
            typeof(PolygonCollider2D),
            typeof(EdgeCollider2D),
            typeof(CompositeCollider2D),
        };
        int apagados = 0;
        foreach (var t in tiposCollider)
        {
            try
            {
                // Unity 6 API: solo Type + bool (sin GizmoType enum)
                GizmoUtility.SetGizmoEnabled(t, false, true);
                GizmoUtility.SetIconEnabled(t, false);
                apagados++;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[N2] No pude apagar gizmo de {t.Name}: {e.Message}");
            }
        }
        Debug.Log($"[N2] ✅ Gizmos de {apagados} tipos de Collider2D apagados.");
        EditorUtility.DisplayDialog("Momias",
            "✅ Listo!\n\nLas lineas verdes de los colliders ya no se muestran en Scene y Game views.\n\n" +
            "Los colliders siguen funcionando normal (es solo visualizacion).\n" +
            "Para revertir: Edit > Preferences > Gizmos.",
            "OK");
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
        SpawnManagersEnEscenaActual();
    }

    /// <summary>Crea/valida los managers en la escena ACTUAL. Caller responsable de abrir escena.</summary>
    public static void SpawnManagersEnEscenaActual()
    {
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

        AsignarAudiosAlAudioManager();
        Debug.Log("[Managers] ✅ Managers verificados en escena actual");
    }

    public static void AsignarAudiosAlAudioManager()
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
            // Nuevos audios de Antonio (Nov 24)
            { "musicNivel4",           "b7e66099704079e49bc9a519e793fc29" },
            { "musicBossMomiaMadre",   "33706be2353b1af4b8826b1c3cb2433f" },
            { "sfxDanoDiego",          "cb39fb9bd8bb6f1408c1701e2d63abb9" },
            { "sfxSalto",              "68cc3e9be06af4b40bff8d0d8a7faeed" },
            { "sfxUIClick",            "160ce2d877d5a35458f242b648dc830c" },
            { "sfxPickupCorazon",      "01b24a95dc16d814e9839b859b6beb5e" },
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

    public static void BuildPauseMenuInline(GameObject canvas)
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

        // Panel Opciones con sliders funcionales
        GameObject optPanel = new GameObject("OptionsPanel", typeof(RectTransform), typeof(OptionsPanel));
        optPanel.transform.SetParent(pausePanel.transform, false);
        var opRt = optPanel.GetComponent<RectTransform>();
        opRt.anchorMin = Vector2.zero; opRt.anchorMax = Vector2.one;
        opRt.offsetMin = Vector2.zero; opRt.offsetMax = Vector2.zero;
        OptionsPanel op = optPanel.GetComponent<OptionsPanel>();

        // Titulo OPCIONES
        var tituloOpc = new GameObject("TituloOpc", typeof(RectTransform));
        tituloOpc.transform.SetParent(optPanel.transform, false);
        var tmpOpc = tituloOpc.AddComponent<TextMeshProUGUI>();
        tmpOpc.text = "OPCIONES"; tmpOpc.fontSize = 90; tmpOpc.fontStyle = FontStyles.Bold;
        tmpOpc.color = DORADO; tmpOpc.alignment = TextAlignmentOptions.Center;
        var tOcRt = tmpOpc.rectTransform;
        tOcRt.anchorMin = new Vector2(0.5f, 0.5f); tOcRt.anchorMax = new Vector2(0.5f, 0.5f);
        tOcRt.pivot = new Vector2(0.5f, 0.5f);
        tOcRt.anchoredPosition = new Vector2(0, 220); tOcRt.sizeDelta = new Vector2(800, 120);

        // Sliders volumen (reuso del helper de FaseB_Setup)
        Slider sliderMusica = FaseB_Setup.BuildVolumeSliderPublic(optPanel.transform, "Musica", 0, 80, 0.6f);
        Slider sliderSFX = FaseB_Setup.BuildVolumeSliderPublic(optPanel.transform, "Efectos (SFX)", 0, 0, 1.0f);

        // Placeholder
        var ph = new GameObject("Placeholder", typeof(RectTransform));
        ph.transform.SetParent(optPanel.transform, false);
        var phTmp = ph.AddComponent<TextMeshProUGUI>();
        phTmp.text = "Mas opciones proximamente en Avance 4";
        phTmp.fontSize = 28; phTmp.fontStyle = FontStyles.Italic;
        phTmp.color = new Color(1, 1, 1, 0.7f);
        phTmp.alignment = TextAlignmentOptions.Center;
        var phRt = phTmp.rectTransform;
        phRt.anchorMin = new Vector2(0.5f, 0.5f); phRt.anchorMax = new Vector2(0.5f, 0.5f);
        phRt.pivot = new Vector2(0.5f, 0.5f);
        phRt.anchoredPosition = new Vector2(0, -80); phRt.sizeDelta = new Vector2(1200, 60);

        // Boton volver del panel opciones
        var btnBack = CrearBotonMenu(optPanel.transform, "BtnVolver", "VOLVER", 0, -220);
        UnityEventTools.AddPersistentListener(btnBack.onClick, pm.MostrarPrincipal);

        // Wire sliders al OptionsPanel
        UnityEventTools.AddPersistentListener<float>(sliderMusica.onValueChanged, op.OnMusicaChanged);
        UnityEventTools.AddPersistentListener<float>(sliderSFX.onValueChanged, op.OnSFXChanged);
        var opSo = new SerializedObject(op);
        opSo.FindProperty("sliderMusica").objectReferenceValue = sliderMusica;
        opSo.FindProperty("sliderSFX").objectReferenceValue = sliderSFX;
        opSo.ApplyModifiedProperties();

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

    public static Button CrearBotonMenu(Transform parent, string nombre, string texto, float x, float y)
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
        boss.transform.position = new Vector3(0f, -1f, 0f);
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

        // BossBridge en GameObject SEPARADO (siempre activo), NO dentro del Boss disabled.
        // Si estuviera en el Boss disabled, los UnityEvents no podrian invocar sus metodos.
        GameObject existingBridge = GameObject.Find("BossBridge");
        if (existingBridge != null) Object.DestroyImmediate(existingBridge);
        GameObject bridgeGo = new GameObject("BossBridge");
        BossBridge bridge = bridgeGo.AddComponent<BossBridge>();
        bridge.boss = bp;
        bridge.bossName = "MOMIA-PIPILA";
        bridge.bar = bar;
        bridge.bellSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PATH_BELL_SPRITE);
        if (bridge.bellSprite == null)
            Debug.LogWarning($"[N2] Sprite de Campana no encontrado en {PATH_BELL_SPRITE}");

        // Hook del WaveManager: cuando se completan todas las oleadas → bridge.AparicionBoss
        // IMPORTANTE: BossBridge.Start() ya hace auto-hook runtime, pero tambien limpiamos
        // persistent listeners fantasmas (del tipo viejo Assembly-CSharp-Editor que ya no existe)
        // y agregamos el nuevo como backup.
        WaveManager wm = Object.FindFirstObjectByType<WaveManager>();
        if (wm != null)
        {
            // Limpiar TODOS los persistent listeners fantasmas del evento
            int count = wm.OnAllWavesCompleted.GetPersistentEventCount();
            for (int i = count - 1; i >= 0; i--)
                UnityEventTools.RemovePersistentListener(wm.OnAllWavesCompleted, i);
            Debug.Log($"[N2] Limpiados {count} persistent listener(s) fantasma(s) de OnAllWavesCompleted");

            UnityEventTools.AddPersistentListener(wm.OnAllWavesCompleted, bridge.AparicionBoss);

            // CRITICO: el WaveManager por default spawnea su propia Campana al terminar.
            // En Nivel 2 NO queremos eso — bridge se encarga despues de matar al boss.
            SerializedObject wmSo = new SerializedObject(wm);
            wmSo.FindProperty("spriteCampanaSagrada").objectReferenceValue = null;
            wmSo.ApplyModifiedProperties();
        }

        bar.gameObject.SetActive(false);
        Debug.Log("[N2] ✅ Boss Pipila + BossHealthBar + Bridge (GO separado) listos");
    }

    private static void FixBossPipilaPrefab()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(PATH_PREFAB_PIPILA);
        try
        {
            // Asegurar BossPipila component
            BossPipila bp = root.GetComponent<BossPipila>();
            if (bp == null) bp = root.AddComponent<BossPipila>();

            // SIEMPRE reasignar data (fix bug: antes se saltaba si el script ya existia)
            EnemyData d = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_PIPILA);
            if (d != null)
            {
                SerializedObject so = new SerializedObject(bp);
                so.FindProperty("data").objectReferenceValue = d;
                so.ApplyModifiedProperties();
                Debug.Log($"[N2] FixBossPipila: data reasignado -> {d.name} (vida={d.vidaMaxima} escala={d.escala})");
            }
            else Debug.LogError($"[N2] FixBossPipila: no se encontro SO en {PATH_SO_PIPILA}");

            // Asegurar Rigidbody2D
            Rigidbody2D rb = root.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = root.AddComponent<Rigidbody2D>();
                rb.gravityScale = 3f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            // Collider2D: siempre ajustar a tamano local 1.0x2.2 (con escala 2.3 world = 2.3x5.06)
            BoxCollider2D col = root.GetComponent<BoxCollider2D>();
            if (col == null) col = root.AddComponent<BoxCollider2D>();
            // Pivot del sprite esta al 17% desde abajo (pies). Alinear bottom del collider con los pies.
            col.size = new Vector2(0.9f, 1.9f);
            col.offset = new Vector2(0f, 0.95f);

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
        whiteImg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");  // sprite built-in para que fillAmount funcione
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
        goldImg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");  // sprite built-in para que fillAmount funcione
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

    public static void BuildHeartsHUDSiFalta()
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
        HeartsHUD hud = go.AddComponent<HeartsHUD>();

        // Asignar los 3 sprites desde CorazonesHUD.png (sliced como Multiple)
        string path = "Assets/Sprites/UI/CorazonesHUD.png";
        Object[] subassets = AssetDatabase.LoadAllAssetsAtPath(path);
        Sprite lleno = null, medio = null, vacio = null;
        foreach (Object obj in subassets)
        {
            if (obj is Sprite sp)
            {
                if (sp.name.Contains("Lleno")) lleno = sp;
                else if (sp.name.Contains("Medio")) medio = sp;
                else if (sp.name.Contains("Vacio")) vacio = sp;
            }
        }
        if (lleno != null && medio != null && vacio != null)
        {
            SerializedObject so = new SerializedObject(hud);
            so.FindProperty("corazonLleno").objectReferenceValue = lleno;
            so.FindProperty("corazonMedio").objectReferenceValue = medio;
            so.FindProperty("corazonVacio").objectReferenceValue = vacio;
            so.ApplyModifiedProperties();
            Debug.Log("[N2] ✅ HeartsHUD creado con 3 sprites de corazones asignados");
        }
        else
        {
            Debug.LogWarning($"[N2] No encontre los 3 sprites de corazon en {path} (Lleno/Medio/Vacio)");
        }
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

    public static void AddBuildSettingsIfMissing(string scenePath)
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
