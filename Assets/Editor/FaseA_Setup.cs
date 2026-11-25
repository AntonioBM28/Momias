using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Menu "Momias/Fase A/..." en la barra superior de Unity que automatiza
/// todo el setup de la Fase A (pulir Nivel 1): stats de SOs, fix MomiaBebe prefab,
/// config sprites nuevos, build HeartPickup prefab, asigna refs y reconfigura oleadas.
/// Replica el patron de Chunk1Setup.cs.
/// </summary>
public static class FaseA_Setup
{
    private const string MENU = "Momias/Fase A/";

    private const string PATH_HEART_SPRITE = "Assets/Sprites/Items/HeartPickup.png";
    private const string PATH_HEART_PREFAB = "Assets/Prefabs/Items/HeartPickup.prefab";
    private const string PATH_SHOCK_DIR = "Assets/Sprites/VFX";
    private const string PATH_SO_CAMINANTE = "Assets/ScriptableObjects/Enemies/MomiaCaminante_Gris.asset";
    private const string PATH_SO_BEBE = "Assets/ScriptableObjects/Enemies/MomiaBebe.asset";
    private const string PATH_SO_PIPILA = "Assets/ScriptableObjects/Enemies/MomiaPipila.asset";
    private const string PATH_PREFAB_WALKING = "Assets/Prefabs/MomiaWalking_0.prefab";
    private const string PATH_PREFAB_BEBE = "Assets/Prefabs/MomiaBebe.prefab";
    private const string PATH_SCENE_NIVEL1 = "Assets/Scenes/Nivel1_Museo.unity";
    private const string PATH_BEBE_SPRITES_DIR = "Assets/Enemies/MomiaBebe";

    // =====================================================================
    // MENU PRINCIPAL
    // =====================================================================

    [MenuItem(MENU + "🚀 Build ALL Fase A", priority = 1)]
    public static void BuildAll()
    {
        if (!EditorUtility.DisplayDialog("Fase A Setup",
            "Esto ejecutara en orden:\n\n" +
            "  1. Config sprites (shockwave + corazon) como Sprite+PointFilter\n" +
            "  2. Fix stats de ScriptableObjects (Caminante, Bebe, Pipila)\n" +
            "  3. Fix script del MomiaBebe prefab (MomiaCaminante → MomiaBebe)\n" +
            "  4. Build HeartPickup prefab\n" +
            "  5. Asignar heartPickupPrefab a MomiaWalking_0 y MomiaBebe\n" +
            "  6. Asignar shockwaveFrames a PlayerAttack de Diego (escena activa)\n" +
            "  7. Reconfigurar oleadas Nivel 1 (requiere escena Nivel1_Museo abierta)\n" +
            "  8. Fix pivot de sprites de MomiaBebe a Bottom-center (para que no flote)\n\n" +
            "¿Continuar?", "Si, hazlo", "Cancelar"))
            return;

        Step1_ConfigSprites();
        Step2_FixScriptableObjects();
        Step3_FixMomiaBebePrefabScript();
        Step4_BuildHeartPickupPrefab();
        Step5_AssignHeartPickupToEnemies();
        Step6_AssignShockwaveToDiego();
        Step7_ReconfigureWavesNivel1();
        Step8_FixBebeSpritesPivot();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Fase A Setup",
            "✅ Fase A completada!\n\n" +
            "Siguiente:\n" +
            "  1. Ctrl+S para guardar la escena\n" +
            "  2. Entra en Play desde Nivel1_Museo\n" +
            "  3. Checklist:\n" +
            "     • Ataca con J → ves onda dorada\n" +
            "     • Momias sueltan corazones (20%)\n" +
            "     • Corazon bobea, al tocarlo cura\n" +
            "     • Oleada 2 mezcla Caminantes + Bebes\n" +
            "     • Bebes parado invencibles, agachado mueren",
            "OK");
    }

    // =====================================================================
    // ITEMS INDIVIDUALES
    // =====================================================================

    [MenuItem(MENU + "1. Config sprites nuevos", priority = 20)]
    public static void Step1_ConfigSprites()
    {
        ConfigureAsSprite(PATH_HEART_SPRITE, 24);
        for (int i = 0; i < 4; i++)
            ConfigureAsSprite($"{PATH_SHOCK_DIR}/Shockwave_{i}.png", 32);
        Debug.Log("[FaseA] ✅ Sprites configurados (Sprite + Point filter + ppu)");
    }

    [MenuItem(MENU + "2. Fix stats ScriptableObjects", priority = 21)]
    public static void Step2_FixScriptableObjects()
    {
        SetEnemyData(PATH_SO_CAMINANTE, "Momia Caminante", 2, 1.5f, 1f, 1.3f, new Color(0.62f, 0.62f, 0.62f, 1f));
        SetEnemyData(PATH_SO_BEBE, "Momia Bebe", 1, 2.0f, 1f, 0.8f, Color.white);
        SetEnemyData(PATH_SO_PIPILA, "Momia Pipila", 15, 1.2f, 3f, 1.0f, Color.white);
        Debug.Log("[FaseA] ✅ ScriptableObjects actualizados segun brief");
    }

    [MenuItem(MENU + "3. Fix MomiaBebe prefab script", priority = 22)]
    public static void Step3_FixMomiaBebePrefabScript()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_BEBE);
        if (prefab == null) { Debug.LogError($"[FaseA] No encontre {PATH_PREFAB_BEBE}"); return; }

        string tempPath = "Assets/_temp_prefab_edit.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(PATH_PREFAB_BEBE);
        try
        {
            MomiaCaminante vieja = root.GetComponent<MomiaCaminante>();
            MomiaBebe yaExiste = root.GetComponent<MomiaBebe>();

            if (yaExiste != null)
            {
                Debug.Log("[FaseA] MomiaBebe ya tiene script MomiaBebe, nada que hacer");
                return;
            }

            // Guardar EnemyData existente antes de borrar el componente viejo
            EnemyData dataCache = null;
            if (vieja != null)
            {
                var so = new SerializedObject(vieja);
                dataCache = so.FindProperty("data").objectReferenceValue as EnemyData;
                Object.DestroyImmediate(vieja, true);
            }

            MomiaBebe nueva = root.AddComponent<MomiaBebe>();
            if (dataCache != null)
            {
                var so = new SerializedObject(nueva);
                so.FindProperty("data").objectReferenceValue = dataCache;
                so.ApplyModifiedProperties();
            }
            else
            {
                // Fallback: asigna el SO de bebe si existe
                var dataBebe = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_BEBE);
                if (dataBebe != null)
                {
                    var so = new SerializedObject(nueva);
                    so.FindProperty("data").objectReferenceValue = dataBebe;
                    so.ApplyModifiedProperties();
                }
            }

            PrefabUtility.SaveAsPrefabAsset(root, PATH_PREFAB_BEBE);
            Debug.Log("[FaseA] ✅ MomiaBebe prefab: script MomiaCaminante → MomiaBebe");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    [MenuItem(MENU + "4. Build HeartPickup prefab", priority = 23)]
    public static void Step4_BuildHeartPickupPrefab()
    {
        // Asegura carpeta
        string dir = Path.GetDirectoryName(PATH_HEART_PREFAB);
        if (!AssetDatabase.IsValidFolder(dir))
        {
            string parent = Path.GetDirectoryName(dir).Replace("\\", "/");
            string leaf = Path.GetFileName(dir);
            AssetDatabase.CreateFolder(parent, leaf);
        }

        GameObject go = new GameObject("HeartPickup");
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(PATH_HEART_SPRITE);
        if (sprite == null)
            Debug.LogWarning($"[FaseA] No encontre sprite en {PATH_HEART_SPRITE} - re-ejecuta Step 1 primero");
        sr.sprite = sprite;
        sr.sortingOrder = 5;
        sr.color = Color.white;

        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.4f;
        col.offset = Vector2.zero;

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        go.AddComponent<HeartPickup>();

        // Guarda y destruye instancia temporal
        PrefabUtility.SaveAsPrefabAsset(go, PATH_HEART_PREFAB);
        Object.DestroyImmediate(go);
        Debug.Log($"[FaseA] ✅ HeartPickup prefab creado en {PATH_HEART_PREFAB}");
    }

    [MenuItem(MENU + "5. Asignar heartPickup refs", priority = 24)]
    public static void Step5_AssignHeartPickupToEnemies()
    {
        GameObject heartPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_HEART_PREFAB);
        if (heartPrefab == null) { Debug.LogError("[FaseA] No encontre HeartPickup prefab - corre Step 4 primero"); return; }

        AssignHeartRef(PATH_PREFAB_WALKING, heartPrefab);
        AssignHeartRef(PATH_PREFAB_BEBE, heartPrefab);
        Debug.Log("[FaseA] ✅ heartPickupPrefab asignado a MomiaWalking_0 y MomiaBebe");
    }

    [MenuItem(MENU + "6. Asignar shockwave a Diego", priority = 25)]
    public static void Step6_AssignShockwaveToDiego()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) { Debug.LogError("[FaseA] No hay GameObject con tag 'Player' en la escena activa. Abre Nivel1_Museo primero."); return; }
        PlayerAttack pa = player.GetComponent<PlayerAttack>();
        if (pa == null) { Debug.LogError("[FaseA] El Player no tiene PlayerAttack."); return; }

        Sprite[] frames = new Sprite[4];
        for (int i = 0; i < 4; i++)
        {
            frames[i] = AssetDatabase.LoadAssetAtPath<Sprite>($"{PATH_SHOCK_DIR}/Shockwave_{i}.png");
            if (frames[i] == null) Debug.LogWarning($"[FaseA] Falta Shockwave_{i}.png - corre Step 1 primero");
        }

        var so = new SerializedObject(pa);
        var arr = so.FindProperty("shockwaveFrames");
        arr.arraySize = 4;
        for (int i = 0; i < 4; i++)
            arr.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(pa);
        Debug.Log("[FaseA] ✅ shockwaveFrames[] asignados a PlayerAttack de Diego");
    }

    [MenuItem(MENU + "7. Reconfigurar oleadas Nivel 1", priority = 26)]
    public static void Step7_ReconfigureWavesNivel1()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (!scene.path.EndsWith("Nivel1_Museo.unity"))
        {
            if (!EditorUtility.DisplayDialog("Fase A", "La escena activa no es Nivel1_Museo. ¿La abro?", "Si", "Cancelar"))
                return;
            EditorSceneManager.OpenScene(PATH_SCENE_NIVEL1);
        }

        WaveManager wm = Object.FindFirstObjectByType<WaveManager>();
        if (wm == null) { Debug.LogError("[FaseA] No hay WaveManager en Nivel1_Museo"); return; }

        GameObject prefCam = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_WALKING);
        GameObject prefBebe = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_BEBE);
        EnemyData dataCam = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_CAMINANTE);
        EnemyData dataBebe = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_BEBE);

        var so = new SerializedObject(wm);
        var list = so.FindProperty("oleadas");
        list.arraySize = 5;

        SetOleada(list.GetArrayElementAtIndex(0), "Oleada 1 - Caminantes", prefCam, dataCam, 3, 1.5f, 2f);
        SetOleada(list.GetArrayElementAtIndex(1), "Oleada 2a - Caminantes", prefCam, dataCam, 4, 1.3f, 3f);
        SetOleada(list.GetArrayElementAtIndex(2), "Oleada 2b - Bebe", prefBebe, dataBebe, 1, 0.8f, 0.5f);
        SetOleada(list.GetArrayElementAtIndex(3), "Oleada 3a - Caminantes", prefCam, dataCam, 5, 1.2f, 3f);
        SetOleada(list.GetArrayElementAtIndex(4), "Oleada 3b - Bebes", prefBebe, dataBebe, 3, 0.9f, 0.5f);

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(wm);
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("[FaseA] ✅ Oleadas Nivel 1 reconfiguradas: 3C → 4C+1B → 5C+3B");
    }

    [MenuItem(MENU + "8. Fix pivot sprites MomiaBebe", priority = 27)]
    public static void Step8_FixBebeSpritesPivot()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { PATH_BEBE_SPRITES_DIR });
        int fixedCount = 0;
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti == null) continue;

            // Pivot global del asset (usado si spriteMode = Single)
            TextureImporterSettings settings = new TextureImporterSettings();
            ti.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
            settings.spritePivot = new Vector2(0.5f, 0f);
            ti.SetTextureSettings(settings);

            // Si es sliced (Multiple), tocar cada slice individual
            if (ti.spriteImportMode == SpriteImportMode.Multiple)
            {
                SpriteMetaData[] sheet = ti.spritesheet;
                for (int i = 0; i < sheet.Length; i++)
                {
                    sheet[i].alignment = (int)SpriteAlignment.BottomCenter;
                    sheet[i].pivot = new Vector2(0.5f, 0f);
                }
                ti.spritesheet = sheet;
            }

            ti.SaveAndReimport();
            fixedCount++;
            Debug.Log($"[FaseA] Pivot bottom aplicado: {path}");
        }
        Debug.Log($"[FaseA] ✅ Pivot fix a {fixedCount} texturas de MomiaBebe");
    }

    // =====================================================================
    // HELPERS
    // =====================================================================

    private static void ConfigureAsSprite(string path, int ppu)
    {
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti == null) { Debug.LogWarning($"[FaseA] No existe asset en {path} (¿corriste Python? ¿Refrescaste AssetDatabase?)"); return; }
        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Single;
        ti.filterMode = FilterMode.Point;
        ti.textureCompression = TextureImporterCompression.Uncompressed;
        ti.spritePixelsPerUnit = ppu;
        ti.mipmapEnabled = false;
        ti.alphaIsTransparency = true;
        ti.SaveAndReimport();
    }

    private static void SetEnemyData(string path, string nombre, int vida, float vel, float danio, float escala, Color tinte)
    {
        EnemyData d = AssetDatabase.LoadAssetAtPath<EnemyData>(path);
        if (d == null) { Debug.LogWarning($"[FaseA] No encontre SO en {path}"); return; }
        var so = new SerializedObject(d);
        so.FindProperty("nombre").stringValue = nombre;
        so.FindProperty("vidaMaxima").intValue = vida;
        so.FindProperty("velocidad").floatValue = vel;
        so.FindProperty("danio").floatValue = danio;
        so.FindProperty("escala").floatValue = escala;
        so.FindProperty("tinte").colorValue = tinte;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(d);
    }

    private static void AssignHeartRef(string prefabPath, GameObject heartPrefab)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            EnemyBase eb = root.GetComponent<EnemyBase>();
            if (eb == null) { Debug.LogWarning($"[FaseA] {prefabPath} no tiene EnemyBase"); return; }
            var so = new SerializedObject(eb);
            so.FindProperty("heartPickupPrefab").objectReferenceValue = heartPrefab;
            so.FindProperty("heartDropChance").floatValue = 0.12f;
            so.ApplyModifiedProperties();
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void SetOleada(SerializedProperty prop, string nombre, GameObject prefab, EnemyData data, int cantidad, float intervalo, float pausa)
    {
        prop.FindPropertyRelative("nombre").stringValue = nombre;
        prop.FindPropertyRelative("prefabEnemigo").objectReferenceValue = prefab;
        prop.FindPropertyRelative("dataEnemigo").objectReferenceValue = data;
        prop.FindPropertyRelative("cantidad").intValue = cantidad;
        prop.FindPropertyRelative("intervaloSpawn").floatValue = intervalo;
        prop.FindPropertyRelative("pausaAntesDeOleada").floatValue = pausa;
    }
}
