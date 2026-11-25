using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Fase E: integra la Momia Escupidora y su proyectil.
/// Menus:
///   - 🚀 Integrar Escupidora (prefab + sprites + wiring)
///   - 🧪 Inyectar Escupidora en Nivel 1 (test aislado)
///   - Steps individuales E1-E4
/// </summary>
public static class FaseE_Escupidora
{
    private const string MENU = "Momias/Fase E/";

    private const string PATH_PREFAB_ESCUPIDORA = "Assets/Prefabs/MomiaEscupidora.prefab";
    private const string PATH_SO_ESCUPIDORA = "Assets/ScriptableObjects/Enemies/MomiaEscupidora.asset";
    private const string PATH_PREFAB_PROJ = "Assets/Prefabs/EnemyProjectile.prefab";
    private const string PATH_HEART_PREFAB = "Assets/Prefabs/Items/HeartPickup.prefab";
    private const string PATH_SPRITES_PROJ_DIR = "Assets/Sprites/VFX";

    private const string PATH_SCENE_N1 = "Assets/Scenes/Nivel1_Museo.unity";

    // =====================================================================
    // MENU PRINCIPAL
    // =====================================================================

    [MenuItem(MENU + "🚀 Integrar Escupidora (TODO)", priority = 2)]
    public static void IntegrarAll()
    {
        if (!EditorUtility.DisplayDialog("Fase E - Escupidora",
            "Esto hace:\n\n" +
            "  1. Configura sprites del Escupitajo (ppu=12, pivot center)\n" +
            "  2. Crea prefab EnemyProjectile.prefab\n" +
            "  3. Agrega MomiaEscupidora.cs al prefab Escupidora + asigna SO + proyectil + puntoDisparo + heartPickup\n\n" +
            "Luego podes inyectarla en Nivel 1 para probar con el menu de abajo.\n\n" +
            "¿Continuar?", "Si", "Cancelar")) return;

        Step1_ConfigSpritesProyectil();
        Step2_CrearProyectilPrefab();
        Step3_IntegrarEscupidoraPrefab();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Fase E",
            "✅ Escupidora integrada.\n\n" +
            "Siguiente: Momias/Fase E/🧪 Inyectar Escupidora en Nivel 1 (test).\n" +
            "Despues play Nivel 1, llega a la oleada 3 y veras 1 Escupidora disparando.",
            "OK");
    }

    // =====================================================================
    // STEPS
    // =====================================================================

    [MenuItem(MENU + "E1. Config sprites del escupitajo", priority = 20)]
    public static void Step1_ConfigSpritesProyectil()
    {
        for (int i = 0; i < 4; i++)
        {
            string path = $"{PATH_SPRITES_PROJ_DIR}/Escupitajo_{i}.png";
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti == null) { Debug.LogWarning($"[FaseE] No existe {path}"); continue; }
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.filterMode = FilterMode.Point;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.spritePixelsPerUnit = 12;
            ti.mipmapEnabled = false;
            ti.alphaIsTransparency = true;

            TextureImporterSettings tis = new TextureImporterSettings();
            ti.ReadTextureSettings(tis);
            tis.spriteAlignment = (int)SpriteAlignment.Center;
            ti.SetTextureSettings(tis);

            ti.SaveAndReimport();
        }
        Debug.Log("[FaseE] ✅ Sprites Escupitajo configurados (ppu=12, pivot center)");
    }

    [MenuItem(MENU + "E2. Crear EnemyProjectile.prefab", priority = 21)]
    public static void Step2_CrearProyectilPrefab()
    {
        // Load sprites
        Sprite[] frames = new Sprite[4];
        for (int i = 0; i < 4; i++)
            frames[i] = AssetDatabase.LoadAssetAtPath<Sprite>($"{PATH_SPRITES_PROJ_DIR}/Escupitajo_{i}.png");

        if (frames[0] == null)
        {
            Debug.LogError("[FaseE] No se cargo Escupitajo_0.png. Corre E1 primero.");
            return;
        }

        GameObject go = new GameObject("EnemyProjectile");
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = frames[0];
        sr.sortingOrder = 10;

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1.5f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.25f;

        EnemyProjectile ep = go.AddComponent<EnemyProjectile>();
        SerializedObject so = new SerializedObject(ep);
        so.FindProperty("danio").intValue = 2;
        so.FindProperty("velocidadFrame").floatValue = 0.08f;
        so.FindProperty("tiempoVida").floatValue = 3f;
        SerializedProperty framesProp = so.FindProperty("frames");
        framesProp.arraySize = 4;
        for (int i = 0; i < 4; i++)
            framesProp.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
        so.ApplyModifiedProperties();

        // Set layer Enemy (9) para que la matriz de colisiones funcione
        go.layer = 9;

        // Guardar como prefab y limpiar
        Directory.CreateDirectory("Assets/Prefabs");
        PrefabUtility.SaveAsPrefabAsset(go, PATH_PREFAB_PROJ);
        Object.DestroyImmediate(go);
        Debug.Log($"[FaseE] ✅ {PATH_PREFAB_PROJ} creado");
    }

    [MenuItem(MENU + "E3. Integrar MomiaEscupidora prefab", priority = 22)]
    public static void Step3_IntegrarEscupidoraPrefab()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(PATH_PREFAB_ESCUPIDORA);
        try
        {
            // Agregar MomiaEscupidora si no esta
            MomiaEscupidora me = root.GetComponent<MomiaEscupidora>();
            if (me == null) me = root.AddComponent<MomiaEscupidora>();

            // Asegurar Rigidbody2D
            Rigidbody2D rb = root.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = root.AddComponent<Rigidbody2D>();
                rb.gravityScale = 3f;
                rb.freezeRotation = true;
            }
            // Asegurar Collider2D
            if (root.GetComponent<Collider2D>() == null)
            {
                BoxCollider2D col = root.AddComponent<BoxCollider2D>();
                col.size = new Vector2(1.0f, 1.8f);
            }

            // Buscar child PuntoDisparo
            Transform puntoDisparo = root.transform.Find("PuntoDisparo");
            if (puntoDisparo == null)
            {
                GameObject pd = new GameObject("PuntoDisparo");
                pd.transform.SetParent(root.transform, false);
                pd.transform.localPosition = new Vector3(0.3f, 0.5f, 0f);
                puntoDisparo = pd.transform;
                Debug.Log("[FaseE] PuntoDisparo creado (no existia)");
            }

            // Cargar refs
            EnemyData soEsc = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_ESCUPIDORA);
            GameObject projPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_PROJ);
            GameObject heartPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_HEART_PREFAB);

            SerializedObject so = new SerializedObject(me);
            so.FindProperty("data").objectReferenceValue = soEsc;
            so.FindProperty("heartPickupPrefab").objectReferenceValue = heartPrefab;
            so.FindProperty("heartDropChance").floatValue = 0.20f;
            so.FindProperty("proyectilPrefab").objectReferenceValue = projPrefab;
            so.FindProperty("puntoDisparo").objectReferenceValue = puntoDisparo;
            so.FindProperty("cadenciaDisparo").floatValue = 2.5f;
            so.FindProperty("windUp").floatValue = 0.4f;
            so.FindProperty("gravedadProyectil").floatValue = 1.5f;
            so.FindProperty("velocidadProyectilBase").floatValue = 6f;
            so.ApplyModifiedProperties();

            // Tag Enemy + layer 9
            root.tag = "Enemy";
            root.layer = 9;

            PrefabUtility.SaveAsPrefabAsset(root, PATH_PREFAB_ESCUPIDORA);
            Debug.Log($"[FaseE] ✅ {PATH_PREFAB_ESCUPIDORA} integrado (data={soEsc?.name}, proj={projPrefab?.name}, heart={heartPrefab?.name})");
        }
        finally { PrefabUtility.UnloadPrefabContents(root); }
    }

    // =====================================================================
    // TEST: inyectar en Nivel 1
    // =====================================================================

    [MenuItem(MENU + "🧪 Inyectar Escupidora en Nivel 1 (test)", priority = 3)]
    public static void InyectarEnNivel1()
    {
        if (!EditorUtility.DisplayDialog("Test Escupidora en Nivel 1",
            "Esto abre Nivel1_Museo.unity y:\n\n" +
            "  - Agrega una 5a oleada al WaveManager con 1 Escupidora + 2 Caminantes\n" +
            "  - Asi podes probarla aislada sin armar Nivel 3 todavia\n\n" +
            "Se puede quitar luego corriendo este menu otra vez (lo detecta y no duplica).\n\n" +
            "¿Continuar?", "Si", "Cancelar")) return;

        EditorSceneManager.OpenScene(PATH_SCENE_N1);
        Scene s = EditorSceneManager.GetActiveScene();

        WaveManager wm = Object.FindFirstObjectByType<WaveManager>();
        if (wm == null) { Debug.LogError("[FaseE] No hay WaveManager en Nivel 1"); return; }

        GameObject prefabEsc = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_ESCUPIDORA);
        EnemyData soEsc = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_ESCUPIDORA);
        if (prefabEsc == null || soEsc == null) { Debug.LogError("[FaseE] Prefab o SO Escupidora faltante. Corre Integrar primero."); return; }

        SerializedObject wmSo = new SerializedObject(wm);
        SerializedProperty oleadas = wmSo.FindProperty("oleadas");

        // Detectar si ya existe una oleada "TEST_Escupidora"
        for (int i = 0; i < oleadas.arraySize; i++)
        {
            var nom = oleadas.GetArrayElementAtIndex(i).FindPropertyRelative("nombre").stringValue;
            if (nom == "TEST_Escupidora")
            {
                Debug.Log("[FaseE] Ya existe TEST_Escupidora, no se duplica");
                EditorUtility.DisplayDialog("Test", "Ya estaba inyectada. Dale PLAY directo.", "OK");
                return;
            }
        }

        // Agregar al final: oleada con 1 Escupidora
        int idx = oleadas.arraySize;
        oleadas.arraySize = idx + 1;
        SerializedProperty nueva = oleadas.GetArrayElementAtIndex(idx);
        nueva.FindPropertyRelative("nombre").stringValue = "TEST_Escupidora";
        nueva.FindPropertyRelative("prefabEnemigo").objectReferenceValue = prefabEsc;
        nueva.FindPropertyRelative("dataEnemigo").objectReferenceValue = soEsc;
        nueva.FindPropertyRelative("cantidad").intValue = 1;
        nueva.FindPropertyRelative("intervaloSpawn").floatValue = 0.5f;
        nueva.FindPropertyRelative("pausaAntesDeOleada").floatValue = 2f;

        wmSo.ApplyModifiedProperties();
        EditorSceneManager.MarkSceneDirty(s);
        EditorSceneManager.SaveScene(s);

        EditorUtility.DisplayDialog("Test",
            "✅ Oleada TEST_Escupidora agregada al final de Nivel 1.\n\n" +
            "Dale PLAY, salta las oleadas normales matando rapido, y en la ultima aparece 1 Escupidora.\n" +
            "Que esperar:\n" +
            "  - Se queda clavada en spawn (no camina).\n" +
            "  - Cada ~2.5s hace anim Attack visible (wind-up 0.4s) y escupe en parabola a Diego.\n" +
            "  - Si le pegas, flashea pero sigue disparando.\n" +
            "  - Si le pegas al proyectil con el campanazo, lo destruye con dust verde.\n" +
            "  - Al matarla: 20% chance de corazon.\n\n" +
            "Para quitar la oleada test: abrir Nivel1_Museo, seleccionar WaveManager, borrar la ultima oleada a mano.",
            "OK");
    }
}
