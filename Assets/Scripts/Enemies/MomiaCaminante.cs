using UnityEngine;

/// <summary>
/// Momia Caminante base.
/// - Camina horizontalmente hacia Diego.
/// - En rango horizontal, se detiene y dispara "Atacar".
/// - El daño se aplica solo si Diego esta cerca EN 2D (X y Y). Asi, si saltas
///   encima de la momia, no te hiere.
/// </summary>
public class MomiaCaminante : EnemyBase
{
    [Header("Combate")]
    [SerializeField] private float distanciaAtaque = 1.0f;
    [SerializeField] private float distanciaDanio = 1.3f;      // radio circular para aplicar daño
    [SerializeField] private float duracionAtaque = 0.7f;
    [SerializeField] private float momentoDeImpacto = 0.4f;
    [SerializeField] private float cooldownAtaque = 0.6f;

    private bool estaAtacando;
    private float tiempoProximoAtaque;

    protected override void Mover()
    {
        if (estaAtacando) return;

        float dx = playerTransform.position.x - transform.position.x;
        float distanciaX = Mathf.Abs(dx);

        // Flip visual
        if (dx > 0.01f) sr.flipX = false;
        else if (dx < -0.01f) sr.flipX = true;

        if (distanciaX > distanciaAtaque)
        {
            // Caminar hacia Diego
            float dirX = Mathf.Sign(dx);
            rb.linearVelocity = new Vector2(dirX * data.velocidad, rb.linearVelocity.y);
        }
        else
        {
            // En rango horizontal: detenerse y preparar ataque
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (Time.time >= tiempoProximoAtaque)
                StartCoroutine(RutinaAtaque());
        }
    }

    private System.Collections.IEnumerator RutinaAtaque()
    {
        estaAtacando = true;
        tiempoProximoAtaque = Time.time + duracionAtaque + cooldownAtaque;

        if (anim != null) anim.SetTrigger("Atacar");

        yield return new WaitForSeconds(momentoDeImpacto);

        if (!estaMuerto && playerTransform != null)
        {
            // Chequeo en 2D: si Diego saltó encima o está lejos verticalmente, no le pega
            float distancia2D = Vector2.Distance(playerTransform.position, transform.position);
            if (distancia2D <= distanciaDanio)
            {
                PlayerHealth ph = playerTransform.GetComponent<PlayerHealth>();
                if (ph != null && data != null)
                    ph.RecibirDanio(data.danio, transform.position);
            }
        }

        yield return new WaitForSeconds(duracionAtaque - momentoDeImpacto);
        estaAtacando = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, distanciaDanio);
    }
}
