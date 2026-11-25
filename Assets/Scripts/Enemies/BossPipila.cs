using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Boss Momia-Pípila, homenaje al heroe de la Alhondiga.
/// 2 fases:
///   Fase 1 (100%-50% vida): embestidas horizontales de un lado a otro.
///       Camina normal, hace pausa, luego acelera y cruza la pantalla.
///       Diego debe saltar las embestidas.
///   Fase 2 (50%-0% vida): salto alto + caida con onda expansiva que
///       daña a Diego si esta cerca del punto de impacto.
///
/// Stats sugeridos en el SO: vida=15, vel=1.2, danio=3, escala=1.0.
/// </summary>
public class BossPipila : EnemyBase
{
    public enum FaseBoss { Fase1, Fase2 }

    [Header("Fase 1 - Embestidas")]
    [SerializeField] private float velocidadEmbestida = 5f;
    [SerializeField] private float pausaEntreEmbestidas = 1.5f;
    [SerializeField] private float danioEmbestida = 3f;

    [Header("Fase 2 - Salto + Onda")]
    [SerializeField] private float fuerzaSalto = 8f;
    [SerializeField] private float tiempoEnAire = 1.2f;
    [SerializeField] private float radioOnda = 3f;
    [SerializeField] private float danioOnda = 2f;
    [SerializeField] private float pausaEntreSaltos = 3f;
    [SerializeField] private Sprite[] shockwaveFrames;
    [SerializeField] private Color colorOnda = new Color(0.6f, 0.4f, 0.2f, 0.9f);

    [Header("Limites de arena")]
    [SerializeField] private float limiteIzquierdo = -2.8f;
    [SerializeField] private float limiteDerecho = 2.8f;

    [Header("Eventos")]
    public UnityEvent<int, int> OnBossHealthChanged;
    public UnityEvent OnBossSpawned;
    public UnityEvent OnBossDied;
    public UnityEvent<FaseBoss> OnBossFaseChanged;

    private FaseBoss faseActual = FaseBoss.Fase1;
    private float danioAcumulado;

    protected override void Start()
    {
        base.Start();
        rb.interpolation = RigidbodyInterpolation2D.None;  // fix: con escala grande extrapolacion se nota fuera de limites
        OnBossSpawned?.Invoke();
        OnBossHealthChanged?.Invoke(vidaActual, data != null ? data.vidaMaxima : 15);
        StartCoroutine(RutinaIA());
    }

    protected override void Mover()
    {
        // La IA se maneja en la coroutine, Mover() del base no corre
    }

    public override void RecibirDanio(int cantidad, Vector2 origen)
    {
        if (estaMuerto) return;
        base.RecibirDanio(cantidad, origen);
        int maxVida = data != null ? data.vidaMaxima : 15;
        Debug.Log($"[BossPipila] RecibirDanio -> HP: {vidaActual}/{maxVida}");  // TMP debug
        OnBossHealthChanged?.Invoke(vidaActual, maxVida);

        // Chequeo de transicion a Fase 2 al llegar a 50%
        float porcentaje = vidaActual / (float)(data != null ? data.vidaMaxima : 15);
        if (faseActual == FaseBoss.Fase1 && porcentaje <= 0.5f)
        {
            faseActual = FaseBoss.Fase2;
            OnBossFaseChanged?.Invoke(FaseBoss.Fase2);
            Debug.Log("[BossPipila] Fase 2 activada");
        }
    }

    protected override void Morir()
    {
        base.Morir();
        OnBossDied?.Invoke();
    }

    // Boss merece shake grande + hitpause pesado
    protected override void FeedbackMuerte()
    {
        if (CameraShaker.Instance != null) CameraShaker.Instance.Shake(0.4f, 0.5f);
        if (HitPause.Instance != null) HitPause.Instance.Freeze(0.15f);
        DustBurst.Spawn(transform.position, new Color(0.5f, 0.35f, 0.2f, 1f));
        DustBurst.Spawn(transform.position + Vector3.right * 0.5f, new Color(0.5f, 0.35f, 0.2f, 1f));
        DustBurst.Spawn(transform.position + Vector3.left * 0.5f, new Color(0.5f, 0.35f, 0.2f, 1f));
        if (AudioManager.Instance != null) AudioManager.Instance.PlayMuerteMomia();
    }

    private IEnumerator RutinaIA()
    {
        yield return new WaitForSeconds(1f); // delay de entrada
        while (!estaMuerto)
        {
            if (faseActual == FaseBoss.Fase1)
                yield return EmbestidaHorizontal();
            else
                yield return SaltoConOnda();
        }
    }

    private IEnumerator EmbestidaHorizontal()
    {
        if (playerTransform == null) yield break;

        // Mira hacia Diego
        float dirX = Mathf.Sign(playerTransform.position.x - transform.position.x);
        sr.flipX = dirX < 0;

        // Pausa visual
        if (anim != null) anim.SetBool("Caminando", false);
        yield return new WaitForSeconds(0.6f);

        // Embiste con clamp duro cada FixedUpdate sobre rb.position
        if (anim != null) anim.SetBool("Caminando", true);
        float destino = dirX > 0 ? limiteDerecho : limiteIzquierdo;
        while (!estaMuerto && Mathf.Abs(rb.position.x - destino) > 0.5f)
        {
            // Si ya llego al limite en la direccion de avance, cortar
            if (rb.position.x <= limiteIzquierdo && dirX < 0) break;
            if (rb.position.x >= limiteDerecho && dirX > 0) break;

            // Set velocity + clamp posicion duro cada tick de fisica
            rb.linearVelocity = new Vector2(dirX * velocidadEmbestida, rb.linearVelocity.y);
            Vector2 clamped = rb.position;
            clamped.x = Mathf.Clamp(clamped.x, limiteIzquierdo, limiteDerecho);
            rb.position = clamped;

            // Si colisiona con Diego en el camino
            if (playerTransform != null)
            {
                float dx = Mathf.Abs(playerTransform.position.x - transform.position.x);
                float dy = Mathf.Abs(playerTransform.position.y - transform.position.y);
                if (dx < 1.5f && dy < 2.8f) HacerDanioAPlayer(danioEmbestida);
            }
            yield return new WaitForFixedUpdate();
        }
        // Hard clamp final + freno
        Vector2 finalPos = rb.position;
        finalPos.x = Mathf.Clamp(finalPos.x, limiteIzquierdo, limiteDerecho);
        rb.position = finalPos;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        yield return new WaitForSeconds(pausaEntreEmbestidas);
    }

    private IEnumerator SaltoConOnda()
    {
        if (playerTransform == null) yield break;

        // Mira al jugador
        float dirX = Mathf.Sign(playerTransform.position.x - transform.position.x);
        sr.flipX = dirX < 0;

        // Preparacion
        if (anim != null) anim.SetTrigger("Saltar");
        yield return new WaitForSeconds(0.3f);

        // Salto fuerte con movimiento hacia el jugador
        rb.linearVelocity = new Vector2(dirX * 2f, fuerzaSalto);

        // Tiempo en aire
        yield return new WaitForSeconds(tiempoEnAire);

        // Esperar a que caiga (detectar Y vel cerca de 0 o rb en suelo)
        float timeout = 0f;
        while (rb.linearVelocity.y < -0.1f && timeout < 2f && !estaMuerto)
        {
            timeout += Time.deltaTime;
            yield return null;
        }

        // Impacto: onda expansiva
        if (!estaMuerto)
        {
            // VFX
            if (shockwaveFrames != null && shockwaveFrames.Length > 0)
            {
                var vfx = ShockwaveVFX.Spawn(shockwaveFrames, transform.position + Vector3.down * 0.3f, colorOnda, 15);
            }
            if (CameraShaker.Instance != null) CameraShaker.Instance.Shake(0.4f, 0.3f);
            if (HitPause.Instance != null) HitPause.Instance.Freeze(0.08f);
            DustBurst.Spawn(transform.position + Vector3.down * 0.3f, new Color(0.5f, 0.35f, 0.2f, 1f));

            // Daño si Diego esta cerca del punto de impacto
            if (playerTransform != null)
            {
                float dist = Vector2.Distance(playerTransform.position, transform.position);
                if (dist < radioOnda) HacerDanioAPlayer(danioOnda);
            }
        }

        yield return new WaitForSeconds(pausaEntreSaltos);
    }

    private void HacerDanioAPlayer(float cantidad)
    {
        if (playerTransform == null) return;
        PlayerHealth ph = playerTransform.GetComponent<PlayerHealth>();
        if (ph == null || ph.EsInvulnerable) return;

        danioAcumulado += cantidad;
        int danioAplicable = Mathf.FloorToInt(danioAcumulado);
        if (danioAplicable > 0)
        {
            danioAcumulado -= danioAplicable;
            ph.RecibirDanio(danioAplicable, transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 a = new Vector3(limiteIzquierdo, transform.position.y, 0);
        Vector3 b = new Vector3(limiteDerecho, transform.position.y, 0);
        Gizmos.DrawLine(a + Vector3.up * 2, a + Vector3.down * 2);
        Gizmos.DrawLine(b + Vector3.up * 2, b + Vector3.down * 2);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioOnda);
    }
}
