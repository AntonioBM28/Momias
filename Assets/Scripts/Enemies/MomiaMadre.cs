using System.Collections;
using UnityEngine;

/// <summary>
/// Boss final del Nivel 4: Momia Madre con sombrero de charro.
///
/// 3 fases segun HP:
///   Fase 1 (100-66%): camina hacia Diego + golpe cruzado cuando esta cerca.
///   Fase 2 (66-33%): + embestida (Run) cuando Diego esta lejos.
///   Fase 3 (33-0%):  + invoca 2-3 mini-momias cada 8s (Caminantes).
///
/// Usa los parametros del Animator de Antonio:
///   Caminando (Bool), Corriendo (Bool), Atacar (Trigger), Golpeado (Trigger), Morir (Trigger)
///
/// Stats del SO MomiaMadre.asset: vida=30, vel=1.2, danio=4 (2 corazones), escala=2.5.
/// </summary>
public class MomiaMadre : EnemyBase
{
    public enum Fase { Fase1, Fase2, Fase3 }

    [Header("Punto de ataque (child PuntoAtaque)")]
    [SerializeField] private Transform puntoAtaque;

    [Header("Golpe cercano")]
    [SerializeField] private float distanciaGolpe = 2.5f;
    [SerializeField] private float radioImpactoGolpe = 2.0f;
    [SerializeField] private float tiempoImpactoGolpe = 0.333f;
    [SerializeField] private float cadenciaGolpe = 1.8f;

    [Header("Embestida (Fase 2+)")]
    [SerializeField] private float velocidadEmbestida = 5f;
    [SerializeField] private float duracionEmbestida = 1.2f;
    [SerializeField] private float distanciaMinimaEmbestida = 3f;

    [Header("Invocar (Fase 3)")]
    [SerializeField] private GameObject prefabMiniMomia;
    [SerializeField] private EnemyData dataMiniMomia;
    [SerializeField] private int miniMomiasPorInvocar = 2;
    [SerializeField] private float cadenciaInvocar = 8f;

    [Header("Limites arena")]
    [SerializeField] private float limiteIzquierdo = -4.5f;
    [SerializeField] private float limiteDerecho = 4.5f;

    [Header("Eventos")]
    public UnityEngine.Events.UnityEvent<int, int> OnBossHealthChanged;
    public UnityEngine.Events.UnityEvent OnBossSpawned;
    public UnityEngine.Events.UnityEvent OnBossDied;
    public UnityEngine.Events.UnityEvent<Fase> OnFaseChanged;

    private Vector3 puntoAtaqueLocal;
    private bool ocupada;
    private float proximoGolpe;
    private float proximaInvocacion;
    private Fase faseActual = Fase.Fase1;
    private float danioAcumulado;

    protected override void Awake()
    {
        base.Awake();
        if (puntoAtaque != null) puntoAtaqueLocal = puntoAtaque.localPosition;
    }

    protected override void Start()
    {
        base.Start();
        rb.interpolation = RigidbodyInterpolation2D.None;
        proximaInvocacion = Time.time + cadenciaInvocar;
        OnBossSpawned?.Invoke();
        OnBossHealthChanged?.Invoke(vidaActual, data != null ? data.vidaMaxima : 30);
    }

    public override void RecibirDanio(int cantidad, Vector2 origen)
    {
        if (estaMuerto) return;
        int antes = vidaActual;
        base.RecibirDanio(cantidad, origen);
        int maxVida = data != null ? data.vidaMaxima : 30;
        Debug.Log($"[MomiaMadre] RecibirDanio -> HP: {vidaActual}/{maxVida}");
        OnBossHealthChanged?.Invoke(vidaActual, maxVida);

        // Transiciones de fase
        float pct = vidaActual / (float)maxVida;
        Fase nueva = faseActual;
        if (pct <= 0.33f) nueva = Fase.Fase3;
        else if (pct <= 0.66f) nueva = Fase.Fase2;
        if (nueva != faseActual)
        {
            faseActual = nueva;
            OnFaseChanged?.Invoke(faseActual);
            Debug.Log($"[MomiaMadre] Cambio a {faseActual}");
        }
    }

    protected override void Morir()
    {
        base.Morir();
        OnBossDied?.Invoke();
    }

    protected override void FeedbackMuerte()
    {
        if (CameraShaker.Instance != null) CameraShaker.Instance.Shake(0.5f, 0.6f);
        if (HitPause.Instance != null) HitPause.Instance.Freeze(0.2f);
        DustBurst.Spawn(transform.position, new Color(0.6f, 0.4f, 0.3f, 1f));
        DustBurst.Spawn(transform.position + Vector3.right * 0.5f, new Color(0.6f, 0.4f, 0.3f, 1f));
        DustBurst.Spawn(transform.position + Vector3.left * 0.5f, new Color(0.6f, 0.4f, 0.3f, 1f));
        if (AudioManager.Instance != null) AudioManager.Instance.PlayMuerteMomia();
    }

    protected override void Mover()
    {
        if (ocupada || playerTransform == null) return;

        float dx = playerTransform.position.x - transform.position.x;
        float absDx = Mathf.Abs(dx);

        // Flip sprite + flip PuntoAtaque local
        bool mirandoIzquierda = dx < 0f;
        sr.flipX = mirandoIzquierda;
        if (puntoAtaque != null)
            puntoAtaque.localPosition = new Vector3(mirandoIzquierda ? -Mathf.Abs(puntoAtaqueLocal.x) : Mathf.Abs(puntoAtaqueLocal.x), puntoAtaqueLocal.y, 0f);

        // Prioridad: invocar (Fase 3) > embestida (Fase 2+) > golpe cercano > caminar
        if (faseActual == Fase.Fase3 && Time.time >= proximaInvocacion && prefabMiniMomia != null)
        {
            StartCoroutine(InvocarMiniMomias());
            return;
        }

        bool cerca = absDx <= distanciaGolpe;
        if (cerca && Time.time >= proximoGolpe)
        {
            StartCoroutine(GolpeCercano());
            return;
        }

        if (!cerca && faseActual >= Fase.Fase2 && absDx > distanciaMinimaEmbestida && Time.time >= proximoGolpe)
        {
            StartCoroutine(Embestida(Mathf.Sign(dx)));
            return;
        }

        // Caminar hacia Diego
        if (anim != null) { anim.SetBool("Caminando", !cerca); anim.SetBool("Corriendo", false); }
        float velX = cerca ? 0f : Mathf.Sign(dx) * data.velocidad;
        rb.linearVelocity = new Vector2(velX, rb.linearVelocity.y);

        // Clamp posicion a limites
        Vector2 pos = rb.position;
        pos.x = Mathf.Clamp(pos.x, limiteIzquierdo, limiteDerecho);
        rb.position = pos;
    }

    private IEnumerator GolpeCercano()
    {
        ocupada = true;
        proximoGolpe = Time.time + cadenciaGolpe;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        if (anim != null) { anim.SetBool("Caminando", false); anim.SetTrigger("Atacar"); }

        yield return new WaitForSeconds(tiempoImpactoGolpe);

        if (!estaMuerto && playerTransform != null && puntoAtaque != null)
        {
            float dist = Vector2.Distance(puntoAtaque.position, playerTransform.position);
            if (dist <= radioImpactoGolpe)
                HacerDanioAPlayer(data != null ? data.danio : 4f);
        }

        yield return new WaitForSeconds(0.25f);
        ocupada = false;
    }

    private IEnumerator Embestida(float dir)
    {
        ocupada = true;
        proximoGolpe = Time.time + cadenciaGolpe * 1.5f;
        if (anim != null) { anim.SetBool("Caminando", true); anim.SetBool("Corriendo", true); }

        float fin = Time.time + duracionEmbestida;
        while (Time.time < fin && !estaMuerto)
        {
            rb.linearVelocity = new Vector2(dir * velocidadEmbestida, rb.linearVelocity.y);
            Vector2 pos = rb.position;
            pos.x = Mathf.Clamp(pos.x, limiteIzquierdo, limiteDerecho);
            rb.position = pos;

            // Impacto durante la embestida
            if (playerTransform != null)
            {
                float dx = Mathf.Abs(playerTransform.position.x - transform.position.x);
                float dy = Mathf.Abs(playerTransform.position.y - transform.position.y);
                if (dx < 1.5f && dy < 2.5f) HacerDanioAPlayer(data != null ? data.danio : 4f);
            }
            yield return new WaitForFixedUpdate();
        }
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        if (anim != null) anim.SetBool("Corriendo", false);
        ocupada = false;
    }

    private IEnumerator InvocarMiniMomias()
    {
        ocupada = true;
        proximaInvocacion = Time.time + cadenciaInvocar;
        if (anim != null) { anim.SetBool("Caminando", false); anim.SetTrigger("Atacar"); }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < miniMomiasPorInvocar; i++)
        {
            float sign = (i % 2 == 0) ? -1f : 1f;
            Vector3 pos = transform.position + new Vector3(sign * 2f, 0f, 0f);
            GameObject mm = Instantiate(prefabMiniMomia, pos, Quaternion.identity);
            EnemyBase eb = mm.GetComponent<EnemyBase>();
            if (eb != null && dataMiniMomia != null) eb.SetData(dataMiniMomia);
            DustBurst.Spawn(pos, new Color(0.6f, 0.4f, 0.3f, 1f));
        }
        Debug.Log($"[MomiaMadre] Invoco {miniMomiasPorInvocar} mini-momias");

        yield return new WaitForSeconds(0.5f);
        ocupada = false;
    }

    private void HacerDanioAPlayer(float cantidad)
    {
        if (playerTransform == null) return;
        PlayerHealth ph = playerTransform.GetComponent<PlayerHealth>();
        if (ph == null || ph.EsInvulnerable) return;

        danioAcumulado += cantidad;
        int aplicable = Mathf.FloorToInt(danioAcumulado);
        if (aplicable > 0)
        {
            danioAcumulado -= aplicable;
            ph.RecibirDanio(aplicable, transform.position);
        }
    }
}
