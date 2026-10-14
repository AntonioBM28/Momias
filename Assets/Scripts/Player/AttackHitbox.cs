using UnityEngine;

/// <summary>
/// Zona de daño temporal que spawnea el campanazo.
/// Se autoconfigura con tamaño/duracion y se autodestruye.
/// Daña a cualquier GameObject con un componente que herede de EnemyBase.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class AttackHitbox : MonoBehaviour
{
    private int danio;
    private BoxCollider2D col;
    private System.Collections.Generic.HashSet<EnemyBase> yaDanio = new();

    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    public void Configurar(int danio, Vector2 tamanio, float duracion)
    {
        this.danio = danio;
        col.size = tamanio;
        Destroy(gameObject, duracion);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyBase enemigo = other.GetComponent<EnemyBase>();
        if (enemigo == null) enemigo = other.GetComponentInParent<EnemyBase>();
        if (enemigo == null || yaDanio.Contains(enemigo)) return;

        yaDanio.Add(enemigo);
        enemigo.RecibirDanio(danio, transform.position);
    }
}
