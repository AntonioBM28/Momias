using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Menu "Momias/Fase C/..." - instala CameraShaker + HitPause + DustBurst
/// como singletons persistentes en la escena del Menu (DontDestroyOnLoad
/// los lleva al resto) y agrega ButtonHover a todos los botones de la escena
/// activa. Tambien configura el sprite de polvo.
/// </summary>
public static class FaseC_Setup
{
    private const string MENU = "Momias/Fase C/";
    private const string PATH_SCENE_MENU = "Assets/Scenes/Menu.unity";
    private const string PATH_DUST_SPRITE = "Assets/Sprites/VFX/Dust.png";

    [MenuItem(MENU + "🚀 Build ALL Fase C", priority = 1)]
    public static void BuildAll()
    {
        if (!EditorUtility.DisplayDialog("Fase C Setup",
            "Esto va a:\n\n" +
            "  1. Configurar el sprite Dust.png como Sprite\n" +
            "  2. Abrir la escena Menu.unity\n" +
            "  3. Agregar CameraShaker, HitPause y DustBurst como singletons\n" +
            "     persistentes (sobreviven cambios de escena via DontDestroyOnLoad)\n" +
            "  4. Agregar ButtonHover a todos los Button de Menu.unity y Nivel1_Museo\n\n" +
            "¿Continuar?", "Si, hazlo", "Cancelar"))
            return;

        Step1_ConfigDustSprite();
        Step2_SetupManagersInMenu();
        Step3_AddButtonHoverEverywhere();

        EditorUtility.DisplayDialog("Fase C",
            "✅ Fase C lista!\n\n" +
            "Prueba:\n" +
            "  • Pasa el mouse por botones del menu -> crecen suave 1.1x\n" +
            "  • Juega Nivel 1: al recibir daño la camara tiembla + freeze\n" +
            "  • Al matar momias: freeze-frame + mini shake + nube de polvo\n" +
            "  • Al aterrizar de un salto: polvo levanta de los pies",
            "OK");
    }

    [MenuItem(MENU + "1. Config sprite Dust", priority = 20)]
    public static void Step1_ConfigDustSprite()
    {
        TextureImporter ti = AssetImporter.GetAtPath(PATH_DUST_SPRITE) as TextureImporter;
        if (ti == null) { Debug.LogError($"[FaseC] No existe {PATH_DUST_SPRITE}"); return; }
        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Single;
        ti.filterMode = FilterMode.Point;
        ti.textureCompression = TextureImporterCompression.Uncompressed;
        ti.spritePixelsPerUnit = 12;
        ti.mipmapEnabled = false;
        ti.alphaIsTransparency = true;
        ti.SaveAndReimport();
        Debug.Log("[FaseC] ✅ Dust.png configurado como Sprite");
    }

    [MenuItem(MENU + "2. Instalar Managers en Menu", priority = 21)]
    public static void Step2_SetupManagersInMenu()
    {
        Scene current = EditorSceneManager.GetActiveScene();
        if (current.path != PATH_SCENE_MENU)
        {
            if (current.isDirty) EditorSceneManager.SaveScene(current);
            EditorSceneManager.OpenScene(PATH_SCENE_MENU);
        }

        Scene scene = EditorSceneManager.GetActiveScene();

        // CameraShaker
        if (GameObject.Find("CameraShaker") == null)
        {
            var go = new GameObject("CameraShaker");
            go.AddComponent<CameraShaker>();
            Debug.Log("[FaseC] ✅ CameraShaker creado en Menu");
        }

        // HitPause
        if (GameObject.Find("HitPause") == null)
        {
            var go = new GameObject("HitPause");
            go.AddComponent<HitPause>();
            Debug.Log("[FaseC] ✅ HitPause creado en Menu");
        }

        // DustBurst
        if (GameObject.Find("DustBurst") == null)
        {
            var go = new GameObject("DustBurst");
            DustBurst db = go.AddComponent<DustBurst>();
            Sprite dust = AssetDatabase.LoadAssetAtPath<Sprite>(PATH_DUST_SPRITE);
            var so = new SerializedObject(db);
            so.FindProperty("dustSprite").objectReferenceValue = dust;
            so.ApplyModifiedProperties();
            Debug.Log("[FaseC] ✅ DustBurst creado en Menu con sprite asignado");
        }
        else
        {
            // Asegurarnos que tiene el sprite asignado
            DustBurst db = GameObject.Find("DustBurst").GetComponent<DustBurst>();
            if (db != null)
            {
                var so = new SerializedObject(db);
                if (so.FindProperty("dustSprite").objectReferenceValue == null)
                {
                    Sprite dust = AssetDatabase.LoadAssetAtPath<Sprite>(PATH_DUST_SPRITE);
                    so.FindProperty("dustSprite").objectReferenceValue = dust;
                    so.ApplyModifiedProperties();
                }
            }
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    [MenuItem(MENU + "3. Add ButtonHover a todos los botones", priority = 22)]
    public static void Step3_AddButtonHoverEverywhere()
    {
        int totalEnScenes = 0;
        totalEnScenes += AddButtonHoverInScene("Assets/Scenes/Menu.unity");
        totalEnScenes += AddButtonHoverInScene("Assets/Scenes/Nivel1_Museo.unity");

        Debug.Log($"[FaseC] ✅ ButtonHover agregado a {totalEnScenes} botones en total");
    }

    private static int AddButtonHoverInScene(string scenePath)
    {
        Scene current = EditorSceneManager.GetActiveScene();
        if (current.path != scenePath)
        {
            if (current.isDirty) EditorSceneManager.SaveScene(current);
            EditorSceneManager.OpenScene(scenePath);
        }

        Scene scene = EditorSceneManager.GetActiveScene();
        int count = 0;
        Button[] botones = Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button b in botones)
        {
            if (b.GetComponent<ButtonHover>() == null)
            {
                b.gameObject.AddComponent<ButtonHover>();
                count++;
            }
        }

        if (count > 0)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
        Debug.Log($"[FaseC] {scenePath}: {count} ButtonHover agregados");
        return count;
    }
}
