using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Asigna los 7 audios nuevos de Antonio a los slots correctos del AudioManager
/// usando los GUIDs del .md. Toca el AudioManager en Nivel1_Museo.
/// </summary>
public static class Audios_Setup
{
    private const string MENU = "Momias/Audios/";

    // GUIDs del .md de Antonio
    private const string GUID_MUSIC_MENU = "7635ef1d13bb43a6bd0ecba82062436d";
    private const string GUID_MUSIC_N2 = "d941dc6f3ac24af29be178c576b8ea18";
    private const string GUID_MUSIC_N3 = "1176420ada9a4eeca3c8e9951523f615";
    private const string GUID_MUSIC_BOSS = "b9804f39506347beb6459f6937c7ade4";
    private const string GUID_SFX_GAMEOVER = "217bd4c264b544bd962ed111185c8747";
    private const string GUID_SFX_VICTORIA = "bfa52ab38825496984bf0bd166ffb19d";
    private const string GUID_SFX_MUERTE = "5cae12120dea4c21829566ca209de12b";

    [MenuItem(MENU + "🚀 Asignar todos los clips al AudioManager", priority = 1)]
    public static void AssignAll()
    {
        // Debe abrir Nivel1 primero (donde vive el AudioManager)
        Scene current = EditorSceneManager.GetActiveScene();
        if (!current.path.EndsWith("Nivel1_Museo.unity"))
        {
            if (current.isDirty) EditorSceneManager.SaveScene(current);
            EditorSceneManager.OpenScene("Assets/Scenes/Nivel1_Museo.unity");
        }

        GameObject amGo = GameObject.Find("AudioManager");
        if (amGo == null)
        {
            EditorUtility.DisplayDialog("Audios", "No existe GameObject 'AudioManager' en Nivel1_Museo.", "OK");
            return;
        }
        AudioManager am = amGo.GetComponent<AudioManager>();
        if (am == null)
        {
            EditorUtility.DisplayDialog("Audios", "GameObject AudioManager no tiene el componente.", "OK");
            return;
        }

        SerializedObject so = new SerializedObject(am);

        AssignClip(so, "musicMenu", GUID_MUSIC_MENU);
        AssignClip(so, "musicNivel2", GUID_MUSIC_N2);
        AssignClip(so, "musicNivel3", GUID_MUSIC_N3);
        AssignClip(so, "musicBossPipila", GUID_MUSIC_BOSS);
        AssignClip(so, "musicGameOver", GUID_SFX_GAMEOVER);
        AssignClip(so, "musicVictoria", GUID_SFX_VICTORIA);
        AssignClip(so, "sfxMuerteMomia", GUID_SFX_MUERTE);

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(am);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Audios",
            "✅ Los 7 clips asignados al AudioManager:\n\n" +
            "  • musicMenu → MusicaMenuPrincipal.mp3\n" +
            "  • musicNivel2 → MusicaNivel2.mp3\n" +
            "  • musicNivel3 → MusicaNivel3.mp3\n" +
            "  • musicBossPipila → MusicaBossPipila.mp3\n" +
            "  • musicGameOver → GameOver.mp3\n" +
            "  • musicVictoria → JingleVictoria.mp3\n" +
            "  • sfxMuerteMomia → sfx_muerte_momia.mp3\n\n" +
            "Ahora todos los niveles reproducen su musica, y GameOver/Victoria\n" +
            "suenan una sola vez (no loop).",
            "OK");
    }

    private static void AssignClip(SerializedObject so, string propName, string guid)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning($"[Audios] No encontre asset con GUID {guid} ({propName})");
            return;
        }
        AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        if (clip == null)
        {
            Debug.LogWarning($"[Audios] No pude cargar AudioClip en {path} ({propName})");
            return;
        }
        var prop = so.FindProperty(propName);
        if (prop == null)
        {
            Debug.LogWarning($"[Audios] Slot '{propName}' no existe en AudioManager (¿recompilo?)");
            return;
        }
        prop.objectReferenceValue = clip;
        Debug.Log($"[Audios] ✅ {propName} ← {path}");
    }
}
