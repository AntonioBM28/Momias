using UnityEngine;

/// <summary>
/// Momia Bebe: hereda toda la IA de MomiaCaminante pero solo recibe
/// daño si Diego ataca estando agachado. El campanazo parado se ignora.
/// </summary>
public class MomiaBebe : MomiaCaminante
{
    private PlayerController cachedPlayerController;

    protected override void Start()
    {
        base.Start();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) cachedPlayerController = p.GetComponent<PlayerController>();
        else Debug.LogWarning("[MomiaBebe] No encontre al Player para cachear su controller.");
    }

    public override void RecibirDanio(int cantidad, Vector2 origen)
    {
        if (estaMuerto) return;

        if (cachedPlayerController != null && !cachedPlayerController.IsCrouching)
        {
            Debug.Log("[MomiaBebe] Campanazo ignorado - Diego no estaba agachado");
            return;
        }
        base.RecibirDanio(cantidad, origen);
    }

    /// <summary>
    /// La bebe es chica: sin shake de camara, hitpause mas corto, polvo mas claro.
    /// </summary>
    protected override void FeedbackMuerte()
    {
        if (HitPause.Instance != null) HitPause.Instance.Freeze(0.03f);
        DustBurst.Spawn(transform.position, new Color(0.75f, 0.65f, 0.55f, 1f));
    }
}
