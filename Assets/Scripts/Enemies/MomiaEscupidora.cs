using System.Collections;
using UnityEngine;

/// <summary>
/// Momia Escupidora: enemigo estatico que dispara proyectiles parabolicos
/// hacia la posicion actual de Diego, cada cierto tiempo, con un wind-up
/// visible (animacion Attack) como telegraph.
///
/// - No camina: Mover() vacio.
/// - Cada 2.5s dispara: 0.4s wind-up (anim Attack) -> spawn proyectil parabolico.
/// - El sprite hace flip hacia Diego en cada tick.
/// - RecibirDanio solo flashea, NO corta el disparo (mantiene amenaza).
/// Stats sugeridos en SO: vida=3, vel=1, danio=2, escala=1.3.
/// </summary>
public class MomiaEscupidora : EnemyBase
{
    [Header("Disparo")]
    [SerializeField] private float cadenciaDisparo = 2.5f;
    [SerializeField] private float windUp = 0.4f;
    [SerializeField] private GameObject proyectilPrefab;
    [SerializeField] private Transform puntoDisparo;      // child "PuntoDisparo"
    [SerializeField] private float gravedadProyectil = 1.5f;
    [SerializeField] private float velocidadProyectilBase = 6f;

    [Header("Audio")]
    [SerializeField] private AudioClip sfxEscupir;

    private bool tieneParametroAtacar;
    private float proximoDisparo;
    private bool disparando;

    protected override void Start()
    {
        base.Start();
        proximoDisparo = Time.time + 1.5f;  // delay inicial para que no dispare apenas spawnee

        if (anim != null)
        {
            foreach (var p in anim.parameters)
                if (p.name == "Atacar" && p.type == AnimatorControllerParameterType.Trigger)
                { tieneParametroAtacar = true; break; }
        }

        // Buscar PuntoDisparo si no esta asignado
        if (puntoDisparo == null)
        {
            Transform pd = transform.Find("PuntoDisparo");
            if (pd != null) puntoDisparo = pd;
        }
    }

    protected override void Mover()
    {
        if (playerTransform == null || estaMuerto) return;

        // Flip hacia Diego
        float dx = playerTransform.position.x - transform.position.x;
        if (dx > 0.01f) sr.flipX = false;
        else if (dx < -0.01f) sr.flipX = true;

        // Mantener velocidad X en 0, solo Y de gravedad
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        // Disparo por tiempo
        if (!disparando && Time.time >= proximoDisparo)
            StartCoroutine(RutinaDisparo());
    }

    private IEnumerator RutinaDisparo()
    {
        disparando = true;
        if (tieneParametroAtacar) anim.SetTrigger("Atacar");

        // Wind-up visible como telegraph
        yield return new WaitForSeconds(windUp);

        if (!estaMuerto && playerTransform != null && proyectilPrefab != null)
            Disparar();

        proximoDisparo = Time.time + cadenciaDisparo;
        disparando = false;
    }

    private void Disparar()
    {
        Vector3 origen = puntoDisparo != null ? puntoDisparo.position : transform.position + Vector3.up * 0.5f;
        Vector2 destino = playerTransform.position;

        // Calculo balistico sencillo: dado Vx = velocidadProyectilBase * signo(dx),
        // calcular Vy para que caiga sobre el destino con gravedad g.
        float dx = destino.x - origen.x;
        float dy = destino.y - origen.y;
        float dirX = Mathf.Sign(dx);
        if (Mathf.Abs(dx) < 0.1f) dirX = sr.flipX ? -1f : 1f;

        float vx = velocidadProyectilBase * dirX;
        float t = Mathf.Abs(dx) > 0.1f ? Mathf.Abs(dx) / Mathf.Abs(vx) : 0.4f;
        float g = Physics2D.gravity.magnitude * gravedadProyectil;
        float vy = (dy + 0.5f * g * t * t) / Mathf.Max(t, 0.01f);
        // Clamp vy para que siempre haga arco bonito
        vy = Mathf.Clamp(vy, 2f, 10f);

        GameObject proj = Instantiate(proyectilPrefab, origen, Quaternion.identity);
        EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
        if (ep != null) ep.Lanzar(new Vector2(vx, vy));

        if (AudioManager.Instance != null && sfxEscupir != null)
            AudioManager.Instance.PlaySFX(sfxEscupir);
    }
}
