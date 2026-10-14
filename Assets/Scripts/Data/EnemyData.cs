using UnityEngine;

/// <summary>
/// Datos de una variante de momia. Se crea desde:
/// Project → clic derecho → Create → Momias → Enemy Data
///
/// Nos deja ajustar vida/velocidad/daño/color sin tocar codigo
/// y reutilizar el mismo sprite base cambiando solo el tinte (palette swap).
/// </summary>
[CreateAssetMenu(fileName = "EnemyData", menuName = "Momias/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identificacion")]
    public string nombre = "Momia Caminante";

    [Header("Stats")]
    [Tooltip("Vida en unidades. Un campanazo basico quita 1.")]
    public int vidaMaxima = 2;

    [Tooltip("Velocidad horizontal en unidades/seg.")]
    public float velocidad = 1.5f;

    [Tooltip("Daño al jugador en medios corazones. 1=medio corazon, 2=corazon completo.")]
    public int danio = 1;

    [Header("Aspecto (palette swap)")]
    [Tooltip("Color del sprite. Gris=caminante, Rojo=corredora, Verde=escupidora.")]
    public Color tinte = Color.white;

    [Tooltip("Escala del sprite. 1=normal, 0.5=bebe.")]
    public float escala = 1f;

    [Header("Recompensas (futuro)")]
    public int chispasEco = 1;
}
