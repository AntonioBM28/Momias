using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Menu "Momias" en la barra superior de Unity que automatiza el setup
/// del Chunk 1 (pasos 3.3 a 3.5 de la guia). Un clic y todo creado.
/// </summary>
public static class Chunk1Setup
{
    private const string MENU_ROOT = "Momias/Chunk 1/";

    // =====================================================================
    // MENU PRINCIPAL
    // =====================================================================

    [MenuItem(MENU_ROOT + "🚀 Build ALL remaining (pasos 3.3 → 3.5)", priority = 1)]
    public static void BuildAll()
    {
        if (!EditorUtility.DisplayDialog("Chunk 1 Setup",
            "Esto creara (o reemplazara) en la escena actual:\n\n" +
            "  • GameOverPanel + hijos y botones\n" +
            "  • VictorySystem completo (flash, letterbox, onda dorada, Victory panel)\n" +
            "  • LevelTitleCard (cartel NIVEL 1)\n\n" +
            "Si ya existian con esos nombres, se borran y se vuelven a crear limpios.\n\n" +
            "¿Continuar?", "Si, hazlo", "Cancelar"))
            return;

        BuildGameOverPanel(silent: true);
        BuildVictorySystem(silent: true);
        BuildLevelTitleCard(silent: true);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Chunk 1 Setup",
            "✅ Listo bro!\n\n" +
            "Siguiente:\n" +
            "  1. Ctrl+S para guardar la escena\n" +
            "  2. Entra en Play\n" +
            "  3. Prueba Esc (pausa), muerte (GameOver), y menu Momias → Test Victory Cinematic para la cinematica.",
            "OK");
    }

    // =====================================================================
    // ITEMS INDIVIDUALES
    // =====================================================================

    [MenuItem(MENU_ROOT + "Build GameOverPanel (3.3)", priority = 20)]
    public static void BuildGameOverPanel_Menu() => BuildGameOverPanel(silent: false);

    [MenuItem(MENU_ROOT + "Build VictorySystem (3.4)", priority = 21)]
    public static void BuildVictorySystem_Menu() => BuildVictorySystem(silent: false);

    [MenuItem(MENU_ROOT + "Build LevelTitleCard (3.5)", priority = 22)]
    public static void BuildLevelTitleCard_Menu() => BuildLevelTitleCard(silent: false);

    [MenuItem(MENU_ROOT + "▶️ Test Victory Cinematic (play mode)", priority = 100)]
    public static void TestVictoryCinematic()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("Momias", "Entra en Play primero (boton ▶ arriba).", "OK");
            return;
        }
        var vs = GameObject.Find("VictorySystem");
        if (vs == null) { Debug.LogError("[Chunk1Setup] No se encontro VictorySystem en la escena."); return; }
        var cine = vs.GetComponent<VictoryCinematic>();
        if (cine == null) { Debug.LogError("[Chunk1Setup] VictorySystem no tiene componente VictoryCinematic."); return; }
        cine.PlayCinematic(Vector3.zero);
    }

    // =====================================================================
    // BUILDERS
    // =====================================================================

    public static void BuildGameOverPanel(bool silent)
    {
        var canvas = FindOrCreateGameUICanvas();

        var existing = canvas.transform.Find("GameOverPanel");
        if (existing != null) Undo.DestroyObjectImmediate(existing.gameObject);

        var go = new GameObject("GameOverPanel");
        Undo.RegisterCreatedObjectUndo(go, "Create GameOverPanel");
        go.transform.SetParent(canvas.transform, false);
        var goRt = go.AddComponent<RectTransform>();
        SetStretchFull(goRt);
        var script = go.AddComponent<GameOverPanel>();

        // Panel visible rojo
        var panel = CreateImage("GOPanel", go.transform, new Color(100f/255f, 0f, 0f, 220f/255f));
        SetStretchFull(panel.rectTransform);
        var canvasGroup = panel.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        // Titulo
        var title = CreateTMP("TituloGO", panel.transform, "GAME OVER", 160, new Color(1f, 100f/255f, 100f/255f));
        var titleRt = title.rectTransform;
        SetAnchor(titleRt, 0.5f, 1f);
        titleRt.anchoredPosition = new Vector2(0, -220);
        titleRt.sizeDelta = new Vector2(1500, 220);

        // Subtitulo
        var sub = CreateTMP("SubtituloGO", panel.transform, "Las momias te alcanzaron...", 42, Color.white);
        var subRt = sub.rectTransform;
        SetAnchor(subRt, 0.5f, 1f);
        subRt.anchoredPosition = new Vector2(0, -420);
        subRt.sizeDelta = new Vector2(1500, 80);

        // Botones
        var btnReintentar = CreateButton("BtnReintentar", panel.transform, "Reintentar");
        PlaceButton(btnReintentar, 0, 50);
        var btnMenu = CreateButton("BtnMenuGO", panel.transform, "Menu Principal");
        PlaceButton(btnMenu, 0, -50);

        // Wire references
        var so = new SerializedObject(script);
        so.FindProperty("gameOverPanel").objectReferenceValue = panel.gameObject;
        so.FindProperty("panelGroup").objectReferenceValue = canvasGroup;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var ph = player.GetComponent<PlayerHealth>();
            if (ph != null) so.FindProperty("playerHealth").objectReferenceValue = ph;
        }
        so.ApplyModifiedProperties();

        UnityEventTools.AddPersistentListener(btnReintentar.onClick, script.OnReintentar);
        UnityEventTools.AddPersistentListener(btnMenu.onClick, script.OnMenu);

        panel.gameObject.SetActive(false);
        EditorUtility.SetDirty(go);
        Debug.Log("[Chunk1Setup] ✅ GameOverPanel creado");
        if (!silent) EditorUtility.DisplayDialog("Momias", "GameOverPanel listo!", "OK");
    }

    public static void BuildVictorySystem(bool silent)
    {
        var canvas = FindOrCreateGameUICanvas();

        var existing = canvas.transform.Find("VictorySystem");
        if (existing != null) Undo.DestroyObjectImmediate(existing.gameObject);

        var go = new GameObject("VictorySystem");
        Undo.RegisterCreatedObjectUndo(go, "Create VictorySystem");
        go.transform.SetParent(canvas.transform, false);
        var rtGo = go.AddComponent<RectTransform>();
        SetStretchFull(rtGo);

        // a) FlashBlanco
        var flashColor = Color.white; flashColor.a = 0f;
        var flash = CreateImage("FlashBlanco", go.transform, flashColor);
        SetStretchFull(flash.rectTransform);
        flash.raycastTarget = false;

        // b) GoldFade
        var gold = CreateImage("GoldFade", go.transform, new Color(1f, 200f/255f, 50f/255f, 0f));
        SetStretchFull(gold.rectTransform);
        gold.raycastTarget = false;

        // c) LetterboxArriba
        var barUp = CreateImage("LetterboxArriba", go.transform, Color.black);
        var barUpRt = barUp.rectTransform;
        barUpRt.anchorMin = new Vector2(0, 1);
        barUpRt.anchorMax = new Vector2(1, 1);
        barUpRt.pivot = new Vector2(0.5f, 1);
        barUpRt.anchoredPosition = Vector2.zero;
        barUpRt.sizeDelta = new Vector2(0, 0);
        barUp.raycastTarget = false;

        // d) LetterboxAbajo
        var barDn = CreateImage("LetterboxAbajo", go.transform, Color.black);
        var barDnRt = barDn.rectTransform;
        barDnRt.anchorMin = new Vector2(0, 0);
        barDnRt.anchorMax = new Vector2(1, 0);
        barDnRt.pivot = new Vector2(0.5f, 0);
        barDnRt.anchoredPosition = Vector2.zero;
        barDnRt.sizeDelta = new Vector2(0, 0);
        barDn.raycastTarget = false;

        // e) OndaDorada (anillo que se expande)
        var onda = CreateImage("OndaDorada", go.transform, new Color(1f, 200f/255f, 50f/255f, 0.8f));
        var ondaRt = onda.rectTransform;
        SetAnchor(ondaRt, 0.5f, 0.5f);
        ondaRt.anchoredPosition = Vector2.zero;
        ondaRt.sizeDelta = new Vector2(100, 100);
        ondaRt.localScale = Vector3.zero;
        onda.raycastTarget = false;
        // Intentar usar el sprite Knob builtin para que se vea circular
        var knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        if (knob != null) onda.sprite = knob;

        // f) VictoryPanel con VictoryPanel script
        var vpanelGo = new GameObject("VictoryPanel");
        vpanelGo.transform.SetParent(go.transform, false);
        var vpanelRt = vpanelGo.AddComponent<RectTransform>();
        SetStretchFull(vpanelRt);
        var vpanelScript = vpanelGo.AddComponent<VictoryPanel>();

        // VPanel visible dorado
        var vpanel = CreateImage("VPanel", vpanelGo.transform, new Color(180f/255f, 140f/255f, 40f/255f, 220f/255f));
        SetStretchFull(vpanel.rectTransform);

        var vtitle = CreateTMP("TituloV", vpanel.transform, "¡NIVEL 1 COMPLETADO!", 110, Color.white);
        var vtitleRt = vtitle.rectTransform;
        SetAnchor(vtitleRt, 0.5f, 1f);
        vtitleRt.anchoredPosition = new Vector2(0, -220);
        vtitleRt.sizeDelta = new Vector2(1700, 180);

        var vsub = CreateTMP("SubtituloV", vpanel.transform, "Has recuperado la Campana Sagrada del Museo", 36, Color.white);
        var vsubRt = vsub.rectTransform;
        SetAnchor(vsubRt, 0.5f, 1f);
        vsubRt.anchoredPosition = new Vector2(0, -400);
        vsubRt.sizeDelta = new Vector2(1700, 70);

        var btnSig = CreateButton("BtnSiguienteNivel", vpanel.transform, "Siguiente Nivel");
        PlaceButton(btnSig, 0, 50);
        var btnMenuV = CreateButton("BtnMenuVictoria", vpanel.transform, "Menu Principal");
        PlaceButton(btnMenuV, 0, -50);

        // Wire VictoryPanel
        var vpanelSo = new SerializedObject(vpanelScript);
        vpanelSo.FindProperty("victoryPanel").objectReferenceValue = vpanel.gameObject;
        vpanelSo.FindProperty("panelRect").objectReferenceValue = vpanel.rectTransform;
        vpanelSo.FindProperty("siguienteNivelButton").objectReferenceValue = btnSig;
        vpanelSo.ApplyModifiedProperties();

        UnityEventTools.AddPersistentListener(btnSig.onClick, vpanelScript.OnSiguienteNivel);
        UnityEventTools.AddPersistentListener(btnMenuV.onClick, vpanelScript.OnMenu);

        vpanel.gameObject.SetActive(false);

        // VictoryCinematic
        var cinematic = go.AddComponent<VictoryCinematic>();
        var cineSo = new SerializedObject(cinematic);
        cineSo.FindProperty("flashImage").objectReferenceValue = flash;
        cineSo.FindProperty("goldFadeImage").objectReferenceValue = gold;
        cineSo.FindProperty("barraArriba").objectReferenceValue = barUpRt;
        cineSo.FindProperty("barraAbajo").objectReferenceValue = barDnRt;
        cineSo.FindProperty("ondaDorada").objectReferenceValue = ondaRt;
        cineSo.FindProperty("victoryPanel").objectReferenceValue = vpanelScript;
        cineSo.FindProperty("gameCamera").objectReferenceValue = Camera.main;
        cineSo.ApplyModifiedProperties();

        EditorUtility.SetDirty(go);
        Debug.Log("[Chunk1Setup] ✅ VictorySystem creado");
        if (!silent) EditorUtility.DisplayDialog("Momias", "VictorySystem listo! Entra en Play y usa Momias → Test Victory Cinematic para verlo.", "OK");
    }

    public static void BuildLevelTitleCard(bool silent)
    {
        var canvas = FindOrCreateGameUICanvas();

        var existing = canvas.transform.Find("LevelTitleCard");
        if (existing != null) Undo.DestroyObjectImmediate(existing.gameObject);

        var go = new GameObject("LevelTitleCard");
        Undo.RegisterCreatedObjectUndo(go, "Create LevelTitleCard");
        go.transform.SetParent(canvas.transform, false);
        var rtGo = go.AddComponent<RectTransform>();
        SetStretchFull(rtGo);
        var script = go.AddComponent<LevelTitleCard>();

        var fondo = CreateImage("Fondo", go.transform, new Color(0, 0, 0, 120f/255f));
        SetStretchFull(fondo.rectTransform);
        fondo.raycastTarget = false;

        var contenido = new GameObject("Contenido", typeof(RectTransform), typeof(CanvasGroup));
        contenido.transform.SetParent(go.transform, false);
        var contenidoRt = contenido.GetComponent<RectTransform>();
        SetAnchor(contenidoRt, 0.5f, 0.5f);
        contenidoRt.anchoredPosition = Vector2.zero;
        contenidoRt.sizeDelta = new Vector2(1700, 400);
        var group = contenido.GetComponent<CanvasGroup>();
        group.alpha = 0f;

        var titulo = CreateTMP("Titulo", contenido.transform, "NIVEL 1", 180, new Color(1f, 200f/255f, 50f/255f));
        var tituloRt = titulo.rectTransform;
        SetAnchor(tituloRt, 0.5f, 0.5f);
        tituloRt.anchoredPosition = new Vector2(0, 60);
        tituloRt.sizeDelta = new Vector2(1700, 200);

        var sub = CreateTMP("Subtitulo", contenido.transform, "MUSEO DE LAS MOMIAS", 60, Color.white);
        var subRt = sub.rectTransform;
        SetAnchor(subRt, 0.5f, 0.5f);
        subRt.anchoredPosition = new Vector2(0, -80);
        subRt.sizeDelta = new Vector2(1700, 100);

        var so = new SerializedObject(script);
        so.FindProperty("group").objectReferenceValue = group;
        so.FindProperty("tituloText").objectReferenceValue = titulo;
        so.FindProperty("subtituloText").objectReferenceValue = sub;
        so.ApplyModifiedProperties();

        EditorUtility.SetDirty(go);
        Debug.Log("[Chunk1Setup] ✅ LevelTitleCard creado");
        if (!silent) EditorUtility.DisplayDialog("Momias", "LevelTitleCard listo!", "OK");
    }

    // =====================================================================
    // HELPERS
    // =====================================================================

    private static GameObject FindOrCreateGameUICanvas()
    {
        var canvas = GameObject.Find("GameUI_Canvas");
        if (canvas == null)
        {
            canvas = new GameObject("GameUI_Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(canvas, "Create GameUI_Canvas");
            var c = canvas.GetComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            c.sortingOrder = 50;
            var cs = canvas.GetComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1920, 1080);
            cs.matchWidthOrHeight = 0.5f;
            Debug.Log("[Chunk1Setup] GameUI_Canvas no existia, se creo.");
        }
        // Garantizar EventSystem para que botones funcionen
        if (GameObject.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var es = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
            Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
        }
        return canvas;
    }

    private static Image CreateImage(string name, Transform parent, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.color = color;
        return img;
    }

    private static TMP_Text CreateTMP(string name, Transform parent, string text, float fontSize, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        return tmp;
    }

    private static Button CreateButton(string name, Transform parent, string label)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.92f);

        var txtGo = new GameObject("Text", typeof(RectTransform));
        txtGo.transform.SetParent(go.transform, false);
        var txt = txtGo.AddComponent<TextMeshProUGUI>();
        txt.text = label;
        txt.fontSize = 38;
        txt.color = Color.black;
        txt.alignment = TextAlignmentOptions.Center;
        SetStretchFull(txt.rectTransform);

        var btn = go.GetComponent<Button>();
        btn.targetGraphic = img;
        return btn;
    }

    private static void PlaceButton(Button b, float x, float y)
    {
        var rt = b.GetComponent<RectTransform>();
        SetAnchor(rt, 0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(x, y);
        rt.sizeDelta = new Vector2(400, 70);
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

    private static void SetAnchor(RectTransform rt, float x, float y)
    {
        rt.anchorMin = new Vector2(x, y);
        rt.anchorMax = new Vector2(x, y);
        rt.pivot = new Vector2(0.5f, 0.5f);
    }
}
