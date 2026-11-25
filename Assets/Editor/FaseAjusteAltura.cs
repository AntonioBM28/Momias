using UnityEditor;
using UnityEngine;

/// <summary>
/// Ajusta los colliders de MomiaBebe y MomiaCorredora para que al caer al piso
/// queden a la altura visual correcta (alineada con MomiaCaminante).
/// Bebe aparecia "pelin abajo" -> collider offset.y negativo -> transform queda mas arriba.
/// Corredora aparecia "arriba" -> collider offset.y positivo -> transform queda mas abajo.
/// </summary>
public static class FaseAjusteAltura
{
    [MenuItem("Momias/Ajuste/🔧 Ajustar altura Bebe + Corredora", priority = 1)]
    public static void AjustarAltura()
    {
        AjustarCollider("Assets/Prefabs/MomiaBebe.prefab", deltaY: -0.15f);
        AjustarCollider("Assets/Prefabs/MomiaCorredora.prefab", deltaY: 0.25f);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Ajuste altura",
            "✅ Colliders de Bebe y Corredora ajustados.\n\nSi todavía se ven desalineadas, cambia los valores: Bebe.deltaY (actual -0.15), Corredora.deltaY (actual 0.25) y vuelve a correr este menú.",
            "OK");
    }

    private static void AjustarCollider(string prefabPath, float deltaY)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            var col = root.GetComponent<BoxCollider2D>();
            if (col == null)
            {
                Debug.LogWarning($"[AjusteAltura] {prefabPath} no tiene BoxCollider2D");
                return;
            }
            Vector2 off = col.offset;
            off.y += deltaY;
            col.offset = off;
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            Debug.Log($"[AjusteAltura] {prefabPath}: collider.offset.y += {deltaY} → {col.offset}");
        }
        finally { PrefabUtility.UnloadPrefabContents(root); }
    }
}
