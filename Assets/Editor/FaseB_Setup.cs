using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Menu "Momias/Fase B/..." — arma el Menu Principal completo:
/// Managers persistentes, fondo animado, 4 botones, panel de creditos con
/// scroll, panel de opciones con sliders de volumen.
/// </summary>
public static class FaseB_Setup
{
    private const string MENU = "Momias/Fase B/";
    private const string PATH_SCENE_MENU = "Assets/Scenes/Menu.unity";
    private const string PATH_SCENE_NIVEL1 = "Assets/Scenes/Nivel1_Museo.unity";
    private const string PATH_BG_SPRITE = "Assets/Sprites/UI/Menu/Menu_Background.png";

    private const string TITULO = "MOMIAS DEL GRITO";
    private const string SUBTITULO = "Un guia, cuatro campanas, una noche eterna";

    // Colores
    private static readonly Color DORADO = new Color(1f, 0.78f, 0.28f, 1f);
    private static readonly Color DORADO_CLARO = new Color(1f, 0.92f, 0.65f, 1f);
    private static readonly Color PANEL_OSCURO = new Color(0.08f, 0.06f, 0.12f, 0.90f);
    private static readonly Color BTN_IDLE = new Color(0.12f, 0.08f, 0.18f, 0.88f);
    private static readonly Color BTN_TEXT = new Color(1f, 0.92f, 0.70f, 1f);

    // =====================================================================
    // MENU PRINCIPAL
    // =====================================================================

    [MenuItem(MENU + "🚀 Build ALL Fase B", priority = 1)]
    public static void BuildAll()
    {
        if (!EditorUtility.DisplayDialog("Fase B Setup",
            "Esto va a:\n\n" +
            "  1. Abrir la escena Menu.unity\n" +
            "  2. Configurar el sprite del fondo como UI\n" +
            "  3. Borrar todo el contenido actual y crear:\n" +
            "     • Managers (GameManager, AudioManager, SceneTransition)\n" +
            "     • EventSystem + Main Camera\n" +
            "     • Canvas con fondo animado Ken Burns\n" +
            "     • Titulo \"MOMIAS DEL GRITO\" + subtitulo\n" +
            "     • 4 botones: Jugar, Opciones, Creditos, Salir\n" +
            "     • Panel Creditos con scroll automatico\n" +
            "     • Panel Opciones con sliders volumen\n" +
            "     • Fader para SceneTransition\n" +
            "  4. Rebuild PauseMenu en Nivel1_Museo (mismo diseno que MainMenu + sub-paneles Opciones/Creditos)\n\n" +
            "¿Continuar?", "Si, hazlo", "Cancelar"))
            return;

        ConfigureBackgroundSprite();
        OpenMenuScene();
        BuildMenuScene();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

        // Rebuild PauseMenu en Nivel 1 con el mismo diseno
        RebuildPauseMenuInNivel1();

        // Configurar que Play siempre arranque desde el Menu
        SetMenuAsPlayModeStartScene();

        EditorUtility.DisplayDialog("Fase B",
            "✅ Menu Principal listo!\n\n" +
            "Prueba:\n" +
            "  1. Play desde Menu.unity\n" +
            "  2. Veras el fondo animado + titulo + botones\n" +
            "  3. Clic JUGAR -> fade a negro -> Nivel 1\n" +
            "  4. Desde Pausa en Nivel 1 -> Menu funciona\n\n" +
            "La musica del menu queda silenciosa hasta que asignes el clip\n" +
            "al slot 'musicMenu' del AudioManager (cuando Antonio la entregue).",
            "OK");
    }

    [MenuItem(MENU + "1. Config sprite fondo", priority = 20)]
    public static void ConfigureBackgroundSprite()
    {
        TextureImporter ti = AssetImporter.GetAtPath(PATH_BG_SPRITE) as TextureImporter;
        if (ti == null) { Debug.LogError($"[FaseB] No existe {PATH_BG_SPRITE}"); return; }
        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Single;
        ti.filterMode = FilterMode.Bilinear;
        ti.textureCompression = TextureImporterCompression.Compressed;
        ti.maxTextureSize = 4096;
        ti.alphaIsTransparency = false;
        ti.SaveAndReimport();
        Debug.Log("[FaseB] ✅ Sprite del fondo configurado");
    }

    [MenuItem(MENU + "2. Abrir escena Menu", priority = 21)]
    public static void OpenMenuScene()
    {
        EditorSceneManager.OpenScene(PATH_SCENE_MENU);
    }

    [MenuItem(MENU + "3. Build Menu scene", priority = 22)]
    public static void BuildMenuScene()
    {
        Scene scene = EditorSceneManager.GetActiveScene();
        if (!scene.path.EndsWith("Menu.unity"))
        {
            EditorSceneManager.OpenScene(PATH_SCENE_MENU);
            scene = EditorSceneManager.GetActiveScene();
        }

        // Borrar todo lo actual (excepto Main Camera, que la reutilizamos)
        foreach (GameObject go in scene.GetRootGameObjects())
        {
            if (go.name != "Main Camera") Object.DestroyImmediate(go);
        }

        // Main Camera ajustada
        Camera cam = Camera.main;
        if (cam == null)
        {
            var camGo = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cam = camGo.GetComponent<Camera>();
            cam.tag = "MainCamera";
        }
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.orthographic = true;

        // ---- Managers ----
        CreateManagers();
        AsignarClipsAlAudioManager();

        // ---- EventSystem ----
        EnsureEventSystem();

        // ---- Canvas de UI del Menu ----
        GameObject canvasGo = CreateCanvas("MenuCanvas", 10);
        Canvas canvas = canvasGo.GetComponent<Canvas>();

        // Fondo animado
        GameObject bgGo = CreateMenuBackground(canvasGo.transform);

        // MainMenu controller
        GameObject mainMenuGo = new GameObject("MainMenu");
        mainMenuGo.transform.SetParent(canvasGo.transform, false);
        RectTransform mmRt = mainMenuGo.AddComponent<RectTransform>();
        SetStretchFull(mmRt);
        MainMenu mainMenu = mainMenuGo.AddComponent<MainMenu>();

        // Paneles
        GameObject mainPanel = BuildMainPanel(mainMenuGo.transform, mainMenu);
        GameObject creditsPanel = BuildCreditsPanel(mainMenuGo.transform, mainMenu);
        GameObject optionsPanel = BuildOptionsPanel(mainMenuGo.transform, mainMenu);

        // Wire MainMenu refs
        var mmSo = new SerializedObject(mainMenu);
        mmSo.FindProperty("mainPanel").objectReferenceValue = mainPanel;
        mmSo.FindProperty("creditsPanel").objectReferenceValue = creditsPanel;
        mmSo.FindProperty("optionsPanel").objectReferenceValue = optionsPanel;
        mmSo.ApplyModifiedProperties();

        creditsPanel.SetActive(false);
        optionsPanel.SetActive(false);

        // ---- Fader (para SceneTransition) ----
        GameObject faderCanvasGo = CreateCanvas("FaderCanvas", 100);
        GameObject faderGo = new GameObject("Fader", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Fader));
        faderGo.transform.SetParent(faderCanvasGo.transform, false);
        Image faderImg = faderGo.GetComponent<Image>();
        faderImg.color = Color.black;
        faderImg.raycastTarget = false;
        SetStretchFull(faderGo.GetComponent<RectTransform>());

        // Conecta Fader a SceneTransition
        GameObject stGo = GameObject.Find("SceneTransition");
        if (stGo != null)
        {
            SceneTransition st = stGo.GetComponent<SceneTransition>();
            var stSo = new SerializedObject(st);
            stSo.FindProperty("fader").objectReferenceValue = faderGo.GetComponent<Fader>();
            stSo.ApplyModifiedProperties();
        }

        Debug.Log("[FaseB] ✅ Menu scene armado");
    }

    // =====================================================================
    // BUILDERS
    // =====================================================================

    private static void CreateManagers()
    {
        // GameManager
        if (GameObject.Find("GameManager") == null)
        {
            var go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
        }
        // AudioManager
        if (GameObject.Find("AudioManager") == null)
        {
            var go = new GameObject("AudioManager");
            go.AddComponent<AudioManager>();
        }
        // SceneTransition
        if (GameObject.Find("SceneTransition") == null)
        {
            var go = new GameObject("SceneTransition");
            go.AddComponent<SceneTransition>();
        }
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
    }

    private static GameObject CreateCanvas(string name, int sortingOrder)
    {
        var go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var c = go.GetComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = sortingOrder;
        var cs = go.GetComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cs.matchWidthOrHeight = 0.5f;
        return go;
    }

    private static GameObject CreateMenuBackground(Transform parent)
    {
        var go = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(MenuBackground));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(2100, 1180);  // un poco mayor que canvas para que no muestre bordes al panear

        Image img = go.GetComponent<Image>();
        Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(PATH_BG_SPRITE);
        if (sp == null) Debug.LogWarning($"[FaseB] No cargue sprite {PATH_BG_SPRITE} - corre Step 1 primero");
        img.sprite = sp;
        img.preserveAspect = true;
        img.raycastTarget = false;

        // Capa de vignette oscuro adicional encima (para integrar los botones)
        var vign = new GameObject("VignetteOverlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        vign.transform.SetParent(parent, false);
        SetStretchFull(vign.GetComponent<RectTransform>());
        Image vImg = vign.GetComponent<Image>();
        vImg.color = new Color(0f, 0f, 0f, 0.35f);
        vImg.raycastTarget = false;

        return go;
    }

    private static GameObject BuildMainPanel(Transform parent, MainMenu mainMenu)
    {
        var go = new GameObject("MainPanel", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        SetStretchFull(go.GetComponent<RectTransform>());

        // Titulo
        var titulo = CreateTMP("Titulo", go.transform, TITULO, 180, DORADO, FontStyles.Bold);
        var tituloRt = titulo.rectTransform;
        SetAnchor(tituloRt, 0.5f, 1f);
        tituloRt.anchoredPosition = new Vector2(0, -200);
        tituloRt.sizeDelta = new Vector2(1800, 220);
        AddShadow(titulo);

        // Subtitulo
        var sub = CreateTMP("Subtitulo", go.transform, SUBTITULO, 44, DORADO_CLARO, FontStyles.Italic);
        var subRt = sub.rectTransform;
        SetAnchor(subRt, 0.5f, 1f);
        subRt.anchoredPosition = new Vector2(0, -400);
        subRt.sizeDelta = new Vector2(1600, 80);
        AddShadow(sub);

        // 4 botones
        float startY = -60f;
        float dy = 100f;

        var btnJugar = CreateMenuButton("BtnJugar", go.transform, "JUGAR");
        PlaceCentered(btnJugar, 0, startY);
        UnityEventTools.AddPersistentListener(btnJugar.onClick, mainMenu.OnJugar);

        var btnOpciones = CreateMenuButton("BtnOpciones", go.transform, "OPCIONES");
        PlaceCentered(btnOpciones, 0, startY - dy);
        UnityEventTools.AddPersistentListener(btnOpciones.onClick, mainMenu.MostrarOpciones);

        var btnCreditos = CreateMenuButton("BtnCreditos", go.transform, "CREDITOS");
        PlaceCentered(btnCreditos, 0, startY - dy * 2);
        UnityEventTools.AddPersistentListener(btnCreditos.onClick, mainMenu.MostrarCreditos);

        var btnSalir = CreateMenuButton("BtnSalir", go.transform, "SALIR");
        PlaceCentered(btnSalir, 0, startY - dy * 3);
        UnityEventTools.AddPersistentListener(btnSalir.onClick, mainMenu.OnSalir);

        return go;
    }

    private static GameObject BuildCreditsPanel(Transform parent, MainMenu mainMenu)
    {
        var go = new GameObject("CreditsPanel", typeof(RectTransform), typeof(CreditsPanel));
        go.transform.SetParent(parent, false);
        SetStretchFull(go.GetComponent<RectTransform>());

        // Fondo oscuro semitransparente
        var fondo = CreateImageGO("Fondo", go.transform, PANEL_OSCURO);
        SetStretchFull(fondo.GetComponent<RectTransform>());

        // Contenedor del texto scrolleable (mascara)
        var mask = new GameObject("Mask", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(RectMask2D));
        mask.transform.SetParent(go.transform, false);
        var maskRt = mask.GetComponent<RectTransform>();
        SetAnchor(maskRt, 0.5f, 0.5f);
        maskRt.anchoredPosition = new Vector2(0, 20);
        maskRt.sizeDelta = new Vector2(1400, 800);
        mask.GetComponent<Image>().color = new Color(0, 0, 0, 0.001f); // casi transparente, requerido para mask

        // Contenido que scrollea
        var content = new GameObject("ScrollingContent", typeof(RectTransform));
        content.transform.SetParent(mask.transform, false);
        var contentRt = content.GetComponent<RectTransform>();
        SetAnchor(contentRt, 0.5f, 0.5f);
        contentRt.sizeDelta = new Vector2(1400, 2200);
        contentRt.anchoredPosition = new Vector2(0, -400);

        string creditos =
            "<size=120><color=#FFC847><b>MOMIAS DEL GRITO</b></color></size>\n\n\n" +
            "<size=42>Un juego de</size>\n" +
            "<size=56><b>Cristian E. Oropeza</b>  &  <b>Jose A. Balderas</b></size>\n\n\n\n" +
            "<size=38><color=#FFEBA6>PROGRAMACION Y SISTEMAS</color></size>\n" +
            "<size=50>Cristian Eduardo Oropeza</size>\n\n\n" +
            "<size=38><color=#FFEBA6>ARTE, ANIMACION, AUDIO Y LEVEL DESIGN</color></size>\n" +
            "<size=50>Jose Antonio Balderas Melchor</size>\n\n\n" +
            "<size=38><color=#FFEBA6>MOTOR</color></size>\n" +
            "<size=50>Unity 6.6 — URP 2D</size>\n\n\n" +
            "<size=38><color=#FFEBA6>PROYECTO ACADEMICO</color></size>\n" +
            "<size=46>Creacion de Videojuegos</size>\n" +
            "<size=42>Universidad de Guanajuato — 2026</size>\n\n\n\n" +
            "<size=60><color=#FFC847>Gracias por jugar</color> 🔔</size>";

        var txt = CreateTMP("Texto", content.transform, creditos, 40, Color.white, FontStyles.Normal);
        txt.alignment = TextAlignmentOptions.Center;
        var txtRt = txt.rectTransform;
        SetStretchFull(txtRt);
        txt.textWrappingMode = TextWrappingModes.Normal;

        // Boton Volver
        var btnVolver = CreateMenuButton("BtnVolverCredits", go.transform, "VOLVER");
        PlaceCentered(btnVolver, 0, -460);
        CreditsPanel cp = go.GetComponent<CreditsPanel>();
        UnityEventTools.AddPersistentListener(btnVolver.onClick, cp.OnVolver);

        // Wire refs
        var so = new SerializedObject(cp);
        so.FindProperty("scrollingContent").objectReferenceValue = contentRt;
        so.FindProperty("mainMenu").objectReferenceValue = mainMenu;
        so.ApplyModifiedProperties();

        return go;
    }

    private static GameObject BuildOptionsPanel(Transform parent, MainMenu mainMenu)
    {
        var go = new GameObject("OptionsPanel", typeof(RectTransform), typeof(OptionsPanel));
        go.transform.SetParent(parent, false);
        SetStretchFull(go.GetComponent<RectTransform>());

        // Fondo
        var fondo = CreateImageGO("Fondo", go.transform, PANEL_OSCURO);
        SetStretchFull(fondo.GetComponent<RectTransform>());

        // Titulo
        var titulo = CreateTMP("Titulo", go.transform, "OPCIONES", 100, DORADO, FontStyles.Bold);
        var tituloRt = titulo.rectTransform;
        SetAnchor(tituloRt, 0.5f, 1f);
        tituloRt.anchoredPosition = new Vector2(0, -180);
        tituloRt.sizeDelta = new Vector2(1000, 140);

        // Sliders de volumen
        Slider sliderMusica = BuildVolumeSlider(go.transform, "Musica", 0, 60, 0.6f);
        Slider sliderSFX = BuildVolumeSlider(go.transform, "Efectos (SFX)", 0, -60, 1.0f);

        // Placeholder: proximamente
        var ph = CreateTMP("Placeholder", go.transform, "Mas opciones proximamente en Avance 4", 32, new Color(1, 1, 1, 0.7f), FontStyles.Italic);
        var phRt = ph.rectTransform;
        SetAnchor(phRt, 0.5f, 0.5f);
        phRt.anchoredPosition = new Vector2(0, -230);
        phRt.sizeDelta = new Vector2(1200, 60);

        // Boton Volver
        var btnVolver = CreateMenuButton("BtnVolverOptions", go.transform, "VOLVER");
        PlaceCentered(btnVolver, 0, -400);

        OptionsPanel op = go.GetComponent<OptionsPanel>();
        UnityEventTools.AddPersistentListener(btnVolver.onClick, op.OnVolver);
        UnityEventTools.AddPersistentListener<float>(sliderMusica.onValueChanged, op.OnMusicaChanged);
        UnityEventTools.AddPersistentListener<float>(sliderSFX.onValueChanged, op.OnSFXChanged);

        // Wire refs
        var so = new SerializedObject(op);
        so.FindProperty("sliderMusica").objectReferenceValue = sliderMusica;
        so.FindProperty("sliderSFX").objectReferenceValue = sliderSFX;
        so.FindProperty("mainMenu").objectReferenceValue = mainMenu;
        so.ApplyModifiedProperties();

        return go;
    }

    private static Slider BuildVolumeSlider(Transform parent, string label, float x, float y, float valorInicial)
    {
        // Contenedor
        var cont = new GameObject($"Slider_{label}", typeof(RectTransform));
        cont.transform.SetParent(parent, false);
        var contRt = cont.GetComponent<RectTransform>();
        SetAnchor(contRt, 0.5f, 0.5f);
        contRt.anchoredPosition = new Vector2(x, y);
        contRt.sizeDelta = new Vector2(900, 60);

        // Label
        var txt = CreateTMP("Label", cont.transform, label, 36, BTN_TEXT, FontStyles.Normal);
        var txtRt = txt.rectTransform;
        SetAnchor(txtRt, 0, 0.5f);
        txtRt.anchoredPosition = new Vector2(220, 0);
        txtRt.sizeDelta = new Vector2(400, 50);
        txt.alignment = TextAlignmentOptions.MidlineLeft;

        // Slider (siguiente setup manual para que funcione sin fallos)
        var sGo = new GameObject("Slider", typeof(RectTransform), typeof(Slider));
        sGo.transform.SetParent(cont.transform, false);
        var sRt = sGo.GetComponent<RectTransform>();
        SetAnchor(sRt, 1, 0.5f);
        sRt.anchoredPosition = new Vector2(-50, 0);
        sRt.sizeDelta = new Vector2(400, 20);
        Slider slider = sGo.GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;

        // Background
        var bg = CreateImageGO("Background", sGo.transform, new Color(0.2f, 0.15f, 0.25f, 1f));
        SetStretchFull(bg.GetComponent<RectTransform>());

        // Fill Area
        var fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sGo.transform, false);
        var faRt = fillArea.GetComponent<RectTransform>();
        faRt.anchorMin = new Vector2(0, 0);
        faRt.anchorMax = new Vector2(1, 1);
        faRt.offsetMin = new Vector2(10, 0);
        faRt.offsetMax = new Vector2(-10, 0);

        var fill = CreateImageGO("Fill", fillArea.transform, DORADO);
        var fillRt = fill.GetComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;

        // Handle
        var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleArea.transform.SetParent(sGo.transform, false);
        var haRt = handleArea.GetComponent<RectTransform>();
        haRt.anchorMin = new Vector2(0, 0);
        haRt.anchorMax = new Vector2(1, 1);
        haRt.offsetMin = new Vector2(10, 0);
        haRt.offsetMax = new Vector2(-10, 0);

        var handle = CreateImageGO("Handle", handleArea.transform, DORADO_CLARO);
        var hRt = handle.GetComponent<RectTransform>();
        hRt.sizeDelta = new Vector2(24, 40);

        slider.targetGraphic = handle.GetComponent<Image>();
        slider.fillRect = fillRt;
        slider.handleRect = hRt;
        slider.direction = Slider.Direction.LeftToRight;
        slider.value = valorInicial;

        return slider;
    }

    // =====================================================================
    // UI HELPERS
    // =====================================================================

    private static GameObject CreateImageGO(string name, Transform parent, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
        return go;
    }

    private static TMP_Text CreateTMP(string name, Transform parent, string text, float fontSize, Color color, FontStyles style)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        return tmp;
    }

    private static void AddShadow(TMP_Text tmp)
    {
        var shadow = tmp.gameObject.AddComponent<UnityEngine.UI.Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.75f);
        shadow.effectDistance = new Vector2(3, -3);
    }

    private static Button CreateMenuButton(string name, Transform parent, string label)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.color = BTN_IDLE;

        var txtGo = new GameObject("Text", typeof(RectTransform));
        txtGo.transform.SetParent(go.transform, false);
        var txt = txtGo.AddComponent<TextMeshProUGUI>();
        txt.text = label;
        txt.fontSize = 44;
        txt.fontStyle = FontStyles.Bold;
        txt.color = BTN_TEXT;
        txt.alignment = TextAlignmentOptions.Center;
        SetStretchFull(txt.rectTransform);

        var btn = go.GetComponent<Button>();
        btn.targetGraphic = img;
        var colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.1f, 1.1f, 1.1f, 1f);
        colors.pressedColor = new Color(0.75f, 0.75f, 0.75f, 1f);
        btn.colors = colors;

        return btn;
    }

    private static void PlaceCentered(Button b, float x, float y)
    {
        var rt = b.GetComponent<RectTransform>();
        SetAnchor(rt, 0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(x, y);
        rt.sizeDelta = new Vector2(480, 80);
    }

    private static void SetStretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static void AsignarClipsAlAudioManager()
    {
        GameObject amGo = GameObject.Find("AudioManager");
        if (amGo == null) return;
        AudioManager am = amGo.GetComponent<AudioManager>();
        if (am == null) return;

        SerializedObject so = new SerializedObject(am);
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
        for (int i = 0; i < mapping.GetLength(0); i++)
        {
            string prop = mapping[i, 0], guid = mapping[i, 1];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path)) continue;
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            var p = so.FindProperty(prop);
            if (p != null) p.objectReferenceValue = clip;
        }

        AsignarPorNombre(so, "musicNivel1",       "Musica_Ambiental");
        AsignarPorNombre(so, "sfxCampanazo",      "Campanazo_Diego");
        AsignarPorNombre(so, "sfxImpactoMomia",   "Impacto_Momia");
        AsignarPorNombre(so, "sfxCampanaSagrada", "Recoger_Campana");

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(am);
        Debug.Log("[FaseB] ✅ AudioManager de Menu cableado con 11 clips");
    }

    private static void AsignarPorNombre(SerializedObject so, string prop, string nombreAsset)
    {
        string[] guids = AssetDatabase.FindAssets($"{nombreAsset} t:AudioClip");
        if (guids.Length == 0) return;
        AudioClip c = AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(guids[0]));
        var p = so.FindProperty(prop);
        if (p != null) p.objectReferenceValue = c;
    }

    private static void SetAnchor(RectTransform rt, float x, float y)
    {
        rt.anchorMin = new Vector2(x, y);
        rt.anchorMax = new Vector2(x, y);
        rt.pivot = new Vector2(0.5f, 0.5f);
    }

    // =====================================================================
    // REBUILD PAUSE MENU (MISMO DISEÑO QUE EL MAIN MENU)
    // =====================================================================

    [MenuItem(MENU + "5. Set Play Mode -> Menu", priority = 24)]
    public static void SetMenuAsPlayModeStartScene()
    {
        SceneAsset menuAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(PATH_SCENE_MENU);
        if (menuAsset == null)
        {
            Debug.LogError($"[FaseB] No encontre {PATH_SCENE_MENU} para setear como Play Mode Start Scene");
            return;
        }
        EditorSceneManager.playModeStartScene = menuAsset;
        Debug.Log("[FaseB] ✅ Play Mode Start Scene configurado: Menu.unity. Ahora cualquier Play arranca desde el menu principal.");
    }

    [MenuItem(MENU + "4. Rebuild PauseMenu en Nivel 1", priority = 23)]
    public static void RebuildPauseMenuInNivel1()
    {
        Scene current = EditorSceneManager.GetActiveScene();
        bool guardarYRegresar = current.path != PATH_SCENE_NIVEL1;
        if (guardarYRegresar)
        {
            if (current.isDirty) EditorSceneManager.SaveScene(current);
            EditorSceneManager.OpenScene(PATH_SCENE_NIVEL1);
        }

        Scene scene = EditorSceneManager.GetActiveScene();

        // Localizar el GameUI_Canvas que creo Chunk1Setup
        GameObject canvas = GameObject.Find("GameUI_Canvas");
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Fase B", "No encuentro GameUI_Canvas en Nivel1_Museo. Corre primero Momias/Chunk 1/Build ALL.", "OK");
            return;
        }

        // Borrar PauseMenu anterior si existe
        Transform existing = canvas.transform.Find("PauseMenu");
        if (existing != null) Object.DestroyImmediate(existing.gameObject);
        // Si quedo un PauseMenu suelto en la raiz, borrarlo tambien
        foreach (GameObject go in scene.GetRootGameObjects())
        {
            if (go.name == "PauseMenu" && go != canvas) Object.DestroyImmediate(go);
        }

        // Crear PauseMenu container en el canvas
        GameObject pmRoot = new GameObject("PauseMenu", typeof(RectTransform));
        pmRoot.transform.SetParent(canvas.transform, false);
        SetStretchFull(pmRoot.GetComponent<RectTransform>());
        PauseMenu pm = pmRoot.AddComponent<PauseMenu>();

        // Panel contenedor que se muestra/oculta
        GameObject pausePanel = CreateImageGO("PausePanelFondo", pmRoot.transform, new Color(0.03f, 0.02f, 0.06f, 0.85f));
        SetStretchFull(pausePanel.GetComponent<RectTransform>());

        // Titulo "PAUSA" estilo main menu
        var titulo = CreateTMP("TituloPausa", pausePanel.transform, "PAUSA", 160, DORADO, FontStyles.Bold);
        var tituloRt = titulo.rectTransform;
        SetAnchor(tituloRt, 0.5f, 1f);
        tituloRt.anchoredPosition = new Vector2(0, -180);
        tituloRt.sizeDelta = new Vector2(1200, 200);
        AddShadow(titulo);

        // Sub-paneles
        GameObject mainPanel = BuildPauseMainPanel(pausePanel.transform, pm);
        GameObject optionsPanel = BuildPauseOptionsPanel(pausePanel.transform, pm);

        // Wire refs del PauseMenu
        var so = new SerializedObject(pm);
        so.FindProperty("pausePanel").objectReferenceValue = pausePanel;
        so.FindProperty("mainPanel").objectReferenceValue = mainPanel;
        so.FindProperty("optionsPanel").objectReferenceValue = optionsPanel;
        so.FindProperty("creditsPanel").objectReferenceValue = null;
        so.ApplyModifiedProperties();

        optionsPanel.SetActive(false);
        pausePanel.SetActive(false);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[FaseB] ✅ PauseMenu rebuildeado en Nivel1_Museo con diseno coincidente");
    }

    private static GameObject BuildPauseMainPanel(Transform parent, PauseMenu pm)
    {
        var go = new GameObject("MainPanel", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        SetStretchFull(go.GetComponent<RectTransform>());

        // 4 botones apilados (sin creditos en pausa)
        float startY = 100f;
        float dy = 100f;

        var btnCont = CreateMenuButton("BtnContinuar", go.transform, "CONTINUAR");
        PlaceCentered(btnCont, 0, startY);
        UnityEventTools.AddPersistentListener(btnCont.onClick, pm.OnContinuar);

        var btnOpc = CreateMenuButton("BtnOpciones", go.transform, "OPCIONES");
        PlaceCentered(btnOpc, 0, startY - dy);
        UnityEventTools.AddPersistentListener(btnOpc.onClick, pm.MostrarOpciones);

        var btnMenu = CreateMenuButton("BtnMenuPrincipal", go.transform, "MENU PRINCIPAL");
        PlaceCentered(btnMenu, 0, startY - dy * 2);
        UnityEventTools.AddPersistentListener(btnMenu.onClick, pm.OnMenu);

        var btnSalir = CreateMenuButton("BtnSalir", go.transform, "SALIR");
        PlaceCentered(btnSalir, 0, startY - dy * 3);
        UnityEventTools.AddPersistentListener(btnSalir.onClick, pm.OnSalir);

        return go;
    }

    private static GameObject BuildPauseOptionsPanel(Transform parent, PauseMenu pm)
    {
        var go = new GameObject("OptionsPanel", typeof(RectTransform), typeof(OptionsPanel));
        go.transform.SetParent(parent, false);
        SetStretchFull(go.GetComponent<RectTransform>());

        // Titulo
        var titulo = CreateTMP("Titulo", go.transform, "OPCIONES", 90, DORADO, FontStyles.Bold);
        var tituloRt = titulo.rectTransform;
        SetAnchor(tituloRt, 0.5f, 0.5f);
        tituloRt.anchoredPosition = new Vector2(0, 220);
        tituloRt.sizeDelta = new Vector2(800, 120);

        // Sliders volumen
        Slider sliderMusica = BuildVolumeSlider(go.transform, "Musica", 0, 80, 0.6f);
        Slider sliderSFX = BuildVolumeSlider(go.transform, "Efectos (SFX)", 0, 0, 1.0f);

        // Placeholder
        var ph = CreateTMP("Placeholder", go.transform, "Mas opciones proximamente en Avance 4", 28, new Color(1, 1, 1, 0.7f), FontStyles.Italic);
        var phRt = ph.rectTransform;
        SetAnchor(phRt, 0.5f, 0.5f);
        phRt.anchoredPosition = new Vector2(0, -80);
        phRt.sizeDelta = new Vector2(1200, 60);

        // Boton volver (al main del pause, NO al main menu del juego)
        var btnVolver = CreateMenuButton("BtnVolver", go.transform, "VOLVER");
        PlaceCentered(btnVolver, 0, -220);

        OptionsPanel op = go.GetComponent<OptionsPanel>();
        // Aqui el boton Volver debe ir al mainPanel del pause, no al mainMenu
        UnityEventTools.AddPersistentListener(btnVolver.onClick, pm.MostrarPrincipal);
        UnityEventTools.AddPersistentListener<float>(sliderMusica.onValueChanged, op.OnMusicaChanged);
        UnityEventTools.AddPersistentListener<float>(sliderSFX.onValueChanged, op.OnSFXChanged);

        // Wire refs
        var so = new SerializedObject(op);
        so.FindProperty("sliderMusica").objectReferenceValue = sliderMusica;
        so.FindProperty("sliderSFX").objectReferenceValue = sliderSFX;
        // mainMenu puede quedar null, OnVolver no se usa aqui
        so.ApplyModifiedProperties();

        return go;
    }

    private static GameObject BuildPauseCreditsPanel(Transform parent, PauseMenu pm)
    {
        var go = new GameObject("CreditsPanel", typeof(RectTransform), typeof(CreditsPanel));
        go.transform.SetParent(parent, false);
        SetStretchFull(go.GetComponent<RectTransform>());

        // Mask + scrolling content (igual que el del MainMenu)
        var mask = new GameObject("Mask", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(RectMask2D));
        mask.transform.SetParent(go.transform, false);
        var maskRt = mask.GetComponent<RectTransform>();
        SetAnchor(maskRt, 0.5f, 0.5f);
        maskRt.anchoredPosition = new Vector2(0, 40);
        maskRt.sizeDelta = new Vector2(1300, 650);
        mask.GetComponent<Image>().color = new Color(0, 0, 0, 0.001f);

        var content = new GameObject("ScrollingContent", typeof(RectTransform));
        content.transform.SetParent(mask.transform, false);
        var contentRt = content.GetComponent<RectTransform>();
        SetAnchor(contentRt, 0.5f, 0.5f);
        contentRt.sizeDelta = new Vector2(1300, 2000);
        contentRt.anchoredPosition = new Vector2(0, -400);

        string creditos =
            "<size=100><color=#FFC847><b>MOMIAS DEL GRITO</b></color></size>\n\n\n" +
            "<size=36>Un juego de</size>\n" +
            "<size=50><b>Cristian E. Oropeza</b>  &  <b>Jose A. Balderas</b></size>\n\n\n\n" +
            "<size=34><color=#FFEBA6>PROGRAMACION Y SISTEMAS</color></size>\n" +
            "<size=44>Cristian Eduardo Oropeza</size>\n\n\n" +
            "<size=34><color=#FFEBA6>ARTE, ANIMACION, AUDIO Y LEVEL DESIGN</color></size>\n" +
            "<size=44>Jose Antonio Balderas Melchor</size>\n\n\n" +
            "<size=34><color=#FFEBA6>MOTOR</color></size>\n" +
            "<size=44>Unity 6.6 — URP 2D</size>\n\n\n" +
            "<size=34><color=#FFEBA6>PROYECTO ACADEMICO</color></size>\n" +
            "<size=40>Creacion de Videojuegos</size>\n" +
            "<size=36>Universidad de Guanajuato — 2026</size>\n\n\n\n" +
            "<size=52><color=#FFC847>Gracias por jugar</color> 🔔</size>";

        var txt = CreateTMP("Texto", content.transform, creditos, 36, Color.white, FontStyles.Normal);
        txt.alignment = TextAlignmentOptions.Center;
        var txtRt = txt.rectTransform;
        SetStretchFull(txtRt);
        txt.textWrappingMode = TextWrappingModes.Normal;

        // Boton Volver
        var btnVolver = CreateMenuButton("BtnVolver", go.transform, "VOLVER");
        PlaceCentered(btnVolver, 0, -380);
        UnityEventTools.AddPersistentListener(btnVolver.onClick, pm.MostrarPrincipal);

        // Wire refs del CreditsPanel
        CreditsPanel cp = go.GetComponent<CreditsPanel>();
        var so = new SerializedObject(cp);
        so.FindProperty("scrollingContent").objectReferenceValue = contentRt;
        so.ApplyModifiedProperties();

        return go;
    }
}
