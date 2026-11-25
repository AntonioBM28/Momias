using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Fase E: integra la Momia Escupidora usando el prefab ProyectilEscupidora de Antonio.
/// </summary>
public static class FaseE_Escupidora
{
    private const string MENU = "Momias/Fase E/";

    private const string PATH_PREFAB_ESCUPIDORA = "Assets/Prefabs/MomiaEscupidora.prefab";
    private const string PATH_SO_ESCUPIDORA = "Assets/ScriptableObjects/Enemies/MomiaEscupidora.asset";
    private const string PATH_PREFAB_PROJ = "Assets/Prefabs/ProyectilEscupidora.prefab";  // de Antonio
    private const string PATH_HEART_PREFAB = "Assets/Prefabs/Items/HeartPickup.prefab";

    // Placeholders viejos (de mi version previa, antes de que Antonio entregara el real)
    private const string PATH_PREFAB_PROJ_OLD = "Assets/Prefabs/EnemyProjectile.prefab";
    private const string PATH_SPRITES_OLD_DIR = "Assets/Sprites/VFX";

    private const string PATH_SCENE_N1 = "Assets/Scenes/Nivel1_Museo.unity";

    [MenuItem(MENU + "🚀 Integrar Escupidora (prefab Antonio)", priority = 2)]
    public static void IntegrarAll()
    {
        if (!EditorUtility.DisplayDialog("Fase E - Escupidora",
            "Esto hace:\n\n" +
            "  1. Borra placeholders viejos (EnemyProjectile.prefab + Escupitajo_*.png)\n" +
            "  2. Agrega EnemyProjectile.cs al prefab ProyectilEscupidora de Antonio\n" +
            "  3. Agrega MomiaEscupidora.cs al prefab + wiring (SO, proyectil, PuntoDisparo, heart)\n\n" +
            "¿Continuar?", "Si", "Cancelar")) return;

        LimpiarPlaceholders();
        IntegrarProyectilPrefabAntonio();
        IntegrarEscupidoraPrefab();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Fase E",
            "✅ Escupidora integrada con prefab real de Antonio.\n\n" +
            "Siguiente: Momias/Fase E/🧪 Inyectar en Nivel 1 (test).",
            "OK");
    }

    [MenuItem(MENU + "E1. Limpiar placeholders viejos", priority = 20)]
    public static void LimpiarPlaceholders()
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_PROJ_OLD) != null)
        {
            AssetDatabase.DeleteAsset(PATH_PREFAB_PROJ_OLD);
            Debug.Log($"[FaseE] Borrado placeholder viejo: {PATH_PREFAB_PROJ_OLD}");
        }
        for (int i = 0; i < 4; i++)
        {
            string path = $"{PATH_SPRITES_OLD_DIR}/Escupitajo_{i}.png";
            if (AssetDatabase.LoadAssetAtPath<Sprite>(path) != null)
            {
                AssetDatabase.DeleteAsset(path);
                Debug.Log($"[FaseE] Borrado placeholder viejo: {path}");
            }
        }
    }

    [MenuItem(MENU + "E2. Agregar EnemyProjectile.cs al prefab ProyectilEscupidora", priority = 21)]
    public static void IntegrarProyectilPrefabAntonio()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(PATH_PREFAB_PROJ);
        try
        {
            EnemyProjectile ep = root.GetComponent<EnemyProjectile>();
            if (ep == null) ep = root.AddComponent<EnemyProjectile>();

            SerializedObject so = new SerializedObject(ep);
            so.FindProperty("danio").intValue = 2;
            so.FindProperty("velocidadFrame").floatValue = 0.08f;
            so.FindProperty("tiempoVida").floatValue = 3f;
            // frames[] vacio: la animacion la maneja el Animator de Antonio
            so.FindProperty("frames").arraySize = 0;
            so.ApplyModifiedProperties();

            // Asegurar RB2D con gravedad 1 (Antonio lo puso asi)
            Rigidbody2D rb = root.GetComponent<Rigidbody2D>();
            if (rb != null && rb.gravityScale <= 0f) rb.gravityScale = 1f;

            // Layer 0 (Default) - necesario para colisionar con Player (Enemy<->Player esta OFF en matriz)
            root.layer = 0;

            PrefabUtility.SaveAsPrefabAsset(root, PATH_PREFAB_PROJ);
            Debug.Log($"[FaseE] ✅ EnemyProjectile.cs agregado a {PATH_PREFAB_PROJ}");
        }
        finally { PrefabUtility.UnloadPrefabContents(root); }
    }

    [MenuItem(MENU + "E3. Integrar MomiaEscupidora prefab", priority = 22)]
    public static void IntegrarEscupidoraPrefab()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(PATH_PREFAB_ESCUPIDORA);
        try
        {
            MomiaEscupidora me = root.GetComponent<MomiaEscupidora>();
            if (me == null) me = root.AddComponent<MomiaEscupidora>();

            Rigidbody2D rb = root.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = root.AddComponent<Rigidbody2D>();
                rb.gravityScale = 3f;
                rb.freezeRotation = true;
            }
            if (root.GetComponent<Collider2D>() == null)
            {
                BoxCollider2D col = root.AddComponent<BoxCollider2D>();
                col.size = new Vector2(1.0f, 1.8f);
            }

            Transform puntoDisparo = root.transform.Find("PuntoDisparo");
            if (puntoDisparo == null)
            {
                GameObject pd = new GameObject("PuntoDisparo");
                pd.transform.SetParent(root.transform, false);
                pd.transform.localPosition = new Vector3(0.3f, 0.5f, 0f);
                puntoDisparo = pd.transform;
            }

            EnemyData soEsc = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_ESCUPIDORA);
            GameObject projPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_PROJ);
            GameObject heartPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_HEART_PREFAB);

            if (projPrefab == null) { Debug.LogError($"[FaseE] Falta {PATH_PREFAB_PROJ}"); return; }

            SerializedObject so = new SerializedObject(me);
            so.FindProperty("data").objectReferenceValue = soEsc;
            so.FindProperty("heartPickupPrefab").objectReferenceValue = heartPrefab;
            so.FindProperty("heartDropChance").floatValue = 0.20f;
            so.FindProperty("proyectilPrefab").objectReferenceValue = projPrefab;
            so.FindProperty("puntoDisparo").objectReferenceValue = puntoDisparo;
            so.FindProperty("cadenciaDisparo").floatValue = 2.5f;
            so.FindProperty("windUp").floatValue = 0.4f;
            so.FindProperty("gravedadProyectil").floatValue = 1.0f;  // match Antonio prefab
            so.FindProperty("velocidadProyectilBase").floatValue = 5f;
            so.ApplyModifiedProperties();

            root.tag = "Enemy";
            root.layer = 9;

            PrefabUtility.SaveAsPrefabAsset(root, PATH_PREFAB_ESCUPIDORA);
            Debug.Log($"[FaseE] ✅ {PATH_PREFAB_ESCUPIDORA} integrado");
        }
        finally { PrefabUtility.UnloadPrefabContents(root); }
    }

    [MenuItem(MENU + "🧪 Inyectar Escupidora en Nivel 1 (test)", priority = 3)]
    public static void InyectarEnNivel1()
    {
        if (!EditorUtility.DisplayDialog("Test Escupidora en Nivel 1",
            "Agrega una 5a oleada al WaveManager de Nivel 1 con 1 Escupidora, para probar aislada.\n\n¿Continuar?",
            "Si", "Cancelar")) return;

        EditorSceneManager.OpenScene(PATH_SCENE_N1);
        Scene s = EditorSceneManager.GetActiveScene();

        WaveManager wm = Object.FindFirstObjectByType<WaveManager>();
        if (wm == null) { Debug.LogError("[FaseE] No hay WaveManager en Nivel 1"); return; }

        GameObject prefabEsc = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB_ESCUPIDORA);
        EnemyData soEsc = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO_ESCUPIDORA);
        if (prefabEsc == null || soEsc == null) { Debug.LogError("[FaseE] Falta integrar primero"); return; }

        SerializedObject wmSo = new SerializedObject(wm);
        SerializedProperty oleadas = wmSo.FindProperty("oleadas");

        for (int i = 0; i < oleadas.arraySize; i++)
        {
            if (oleadas.GetArrayElementAtIndex(i).FindPropertyRelative("nombre").stringValue == "TEST_Escupidora")
            {
                Debug.Log("[FaseE] Ya existe TEST_Escupidora, no duplica");
                EditorUtility.DisplayDialog("Test", "Ya estaba inyectada. Dale PLAY directo.", "OK");
                return;
            }
        }

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
            "✅ Oleada TEST_Escupidora agregada.\n\nPLAY Nivel 1, mata las 4 oleadas, la 5a es 1 Escupidora clavada.\n\nQue esperar:\n  - Wind-up 0.4s (anim Attack)\n  - Parabola a Diego\n  - Si le pegas al escupitajo con el campanazo, lo destruye",
            "OK");
    }
}
