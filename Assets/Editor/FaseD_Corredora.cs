using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Setup de la MomiaCorredora: importa los frames que puso Antonio/PixelLab,
/// crea las animaciones .anim, el Animator Controller, el ScriptableObject
/// de stats y el prefab final listo para usar en oleadas.
/// </summary>
public static class FaseD_Corredora
{
    private const string MENU = "Momias/Fase D/";
    private const string DIR_SPRITES = "Assets/Enemies/MomiaCorredora";
    private const string PATH_SO = "Assets/ScriptableObjects/Enemies/MomiaCorredora.asset";
    private const string PATH_CONTROLLER = "Assets/Animations/MomiaCorredora.controller";
    private const string PATH_PREFAB = "Assets/Prefabs/MomiaCorredora.prefab";
    private const string PATH_HEART_PREFAB = "Assets/Prefabs/Items/HeartPickup.prefab";

    [MenuItem(MENU + "🚀 Integrar MomiaCorredora (todo)", priority = 1)]
    public static void BuildAll()
    {
        if (!EditorUtility.DisplayDialog("Fase D - Momia Corredora",
            "Esto va a:\n\n" +
            "  1. Configurar los 31 frames como Sprite con pivot Bottom-center\n" +
            "  2. Crear 3 clips .anim (Run / Attack / Die)\n" +
            "  3. Crear Animator Controller con estados y transitions\n" +
            "  4. Crear ScriptableObject MomiaCorredora.asset\n" +
            "  5. Crear prefab MomiaCorredora.prefab\n\n" +
            "¿Continuar?", "Si, hazlo", "Cancelar"))
            return;

        Step1_ConfigureSprites();
        Step2_CreateAnimClips();
        Step3_CreateAnimatorController();
        Step4_CreateScriptableObject();
        Step5_CreatePrefab();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Fase D",
            "✅ MomiaCorredora integrada!\n\n" +
            "Para probarla rapido:\n" +
            "  1. Abre Nivel1_Museo\n" +
            "  2. En el WaveManager, cambia una oleada temporalmente para\n" +
            "     usar MomiaCorredora.prefab + MomiaCorredora.asset\n" +
            "  3. Play y veras la corredora corriendo mas rapido que la caminante\n\n" +
            "Despues armaremos Nivel 2 completo donde ya se incluyen en las oleadas 3 y 4.",
            "OK");
    }

    [MenuItem(MENU + "1. Config sprites Corredora", priority = 20)]
    public static void Step1_ConfigureSprites()
    {
        string[] subdirs = { "Walk", "Run", "Attack", "Die" };
        int count = 0;
        foreach (string sub in subdirs)
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { $"{DIR_SPRITES}/{sub}" });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti == null) continue;

                ti.textureType = TextureImporterType.Sprite;
                ti.spriteImportMode = SpriteImportMode.Single;
                ti.filterMode = FilterMode.Point;
                ti.textureCompression = TextureImporterCompression.Uncompressed;
                ti.spritePixelsPerUnit = 48;
                ti.mipmapEnabled = false;
                ti.alphaIsTransparency = true;

                // Pivot Custom en Y=0.15 (donde estan los pies reales en sprites PixelLab 48x48)
                TextureImporterSettings s = new TextureImporterSettings();
                ti.ReadTextureSettings(s);
                s.spriteAlignment = (int)SpriteAlignment.Custom;
                s.spritePivot = new Vector2(0.5f, 0.15f);
                ti.SetTextureSettings(s);

                ti.SaveAndReimport();
                count++;
            }
        }
        Debug.Log($"[FaseD-Corredora] ✅ {count} sprites configurados");
    }

    [MenuItem(MENU + "2. Build anim clips", priority = 21)]
    public static void Step2_CreateAnimClips()
    {
        BuildClip("Run", loop: true, frameRate: 14f);
        BuildClip("Attack", loop: false, frameRate: 16f);
        BuildClip("Die", loop: false, frameRate: 10f);
        Debug.Log("[FaseD-Corredora] ✅ 3 anim clips creados (Run, Attack, Die)");
    }

    private static AnimationClip BuildClip(string nombre, bool loop, float frameRate)
    {
        string dir = $"{DIR_SPRITES}/{nombre}";
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { dir });
        List<Sprite> sprites = new List<Sprite>();
        foreach (string g in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(g);
            Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sp != null) sprites.Add(sp);
        }
        // Ordenar por nombre (frame_000, frame_001, ...)
        sprites.Sort((a, b) => string.Compare(a.name, b.name, System.StringComparison.Ordinal));

        AnimationClip clip = new AnimationClip();
        clip.frameRate = frameRate;

        EditorCurveBinding binding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };

        ObjectReferenceKeyframe[] keys = new ObjectReferenceKeyframe[sprites.Count];
        float dt = 1f / frameRate;
        for (int i = 0; i < sprites.Count; i++)
        {
            keys[i] = new ObjectReferenceKeyframe { time = i * dt, value = sprites[i] };
        }
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);

        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        string path2 = $"Assets/Animations/MomiaCorredora_{nombre}.anim";
        AssetDatabase.CreateAsset(clip, path2);
        Debug.Log($"[FaseD-Corredora] Clip {nombre}: {sprites.Count} frames @ {frameRate} fps, loop={loop}");
        return clip;
    }

    [MenuItem(MENU + "3. Build Animator Controller", priority = 22)]
    public static void Step3_CreateAnimatorController()
    {
        // Borrar si existe
        if (AssetDatabase.LoadAssetAtPath<AnimatorController>(PATH_CONTROLLER) != null)
            AssetDatabase.DeleteAsset(PATH_CONTROLLER);

        AnimatorController ac = AnimatorController.CreateAnimatorControllerAtPath(PATH_CONTROLLER);
        ac.AddParameter("Corriendo", AnimatorControllerParameterType.Bool);
        ac.AddParameter("Atacar", AnimatorControllerParameterType.Trigger);
        ac.AddParameter("Morir", AnimatorControllerParameterType.Trigger);

        AnimatorStateMachine sm = ac.layers[0].stateMachine;

        AnimationClip runClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/MomiaCorredora_Run.anim");
        AnimationClip attackClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/MomiaCorredora_Attack.anim");
        AnimationClip dieClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/MomiaCorredora_Die.anim");

        AnimatorState sRun = sm.AddState("Run");
        sRun.motion = runClip;
        AnimatorState sAttack = sm.AddState("Attack");
        sAttack.motion = attackClip;
        AnimatorState sDie = sm.AddState("Die");
        sDie.motion = dieClip;

        sm.defaultState = sRun;

        // Transitions:
        // Any -> Attack (trigger Atacar)
        AnimatorStateTransition tAttack = sm.AddAnyStateTransition(sAttack);
        tAttack.AddCondition(AnimatorConditionMode.If, 0, "Atacar");
        tAttack.duration = 0f;
        tAttack.hasExitTime = false;

        // Any -> Die (trigger Morir)
        AnimatorStateTransition tDie = sm.AddAnyStateTransition(sDie);
        tDie.AddCondition(AnimatorConditionMode.If, 0, "Morir");
        tDie.duration = 0f;
        tDie.hasExitTime = false;

        // Attack -> Run (al terminar la anim)
        AnimatorStateTransition tAttackDone = sAttack.AddTransition(sRun);
        tAttackDone.hasExitTime = true;
        tAttackDone.exitTime = 0.95f;
        tAttackDone.duration = 0.05f;

        Debug.Log("[FaseD-Corredora] ✅ Animator Controller creado");
    }

    [MenuItem(MENU + "4. Build ScriptableObject", priority = 23)]
    public static void Step4_CreateScriptableObject()
    {
        // Si ya existia un .asset (posiblemente con script roto), lo borramos y rehacemos limpio
        if (System.IO.File.Exists(PATH_SO))
        {
            AssetDatabase.DeleteAsset(PATH_SO);
        }
        EnemyData data = ScriptableObject.CreateInstance<EnemyData>();
        AssetDatabase.CreateAsset(data, PATH_SO);
        bool esNuevo = true;
        SerializedObject so = new SerializedObject(data);
        so.FindProperty("nombre").stringValue = "Momia Corredora";
        so.FindProperty("vidaMaxima").intValue = 2;
        so.FindProperty("velocidad").floatValue = 3.5f;
        so.FindProperty("danio").floatValue = 2f;
        so.FindProperty("escala").floatValue = 1.3f;
        so.FindProperty("tinte").colorValue = Color.white;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(data);
        Debug.Log($"[FaseD-Corredora] ✅ SO MomiaCorredora.asset {(esNuevo?"creado":"actualizado")}");
    }

    [MenuItem(MENU + "5. Build Prefab", priority = 24)]
    public static void Step5_CreatePrefab()
    {
        // Borrar prefab existente si hay
        if (AssetDatabase.LoadAssetAtPath<GameObject>(PATH_PREFAB) != null)
            AssetDatabase.DeleteAsset(PATH_PREFAB);

        GameObject go = new GameObject("MomiaCorredora");

        // Rigidbody2D
        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // BoxCollider2D
        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.5f, 0.8f);
        col.offset = new Vector2(0f, 0.4f); // pivot bottom, collider arriba

        // SpriteRenderer con el primer frame del Run como preview
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        Sprite preview = AssetDatabase.LoadAssetAtPath<Sprite>($"{DIR_SPRITES}/Run/frame_000.png");
        if (preview != null) sr.sprite = preview;
        sr.sortingOrder = 10;

        // Animator con el controller
        Animator anim = go.AddComponent<Animator>();
        anim.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(PATH_CONTROLLER);

        // Script MomiaCorredora + data + heartPickup
        MomiaCorredora mc = go.AddComponent<MomiaCorredora>();
        SerializedObject so = new SerializedObject(mc);
        EnemyData data = AssetDatabase.LoadAssetAtPath<EnemyData>(PATH_SO);
        so.FindProperty("data").objectReferenceValue = data;
        GameObject heart = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_HEART_PREFAB);
        if (heart != null)
        {
            so.FindProperty("heartPickupPrefab").objectReferenceValue = heart;
            so.FindProperty("heartDropChance").floatValue = 0.20f;
        }
        so.ApplyModifiedProperties();

        // Layer Enemy (9)
        go.layer = 9;

        // Tag Enemy si existe (no fallamos si no esta definido)
        try { go.tag = "Enemy"; } catch { /* tag Enemy quiza no existe, ignorar */ }

        PrefabUtility.SaveAsPrefabAsset(go, PATH_PREFAB);
        Object.DestroyImmediate(go);
        Debug.Log($"[FaseD-Corredora] ✅ Prefab creado en {PATH_PREFAB}");
    }
}
