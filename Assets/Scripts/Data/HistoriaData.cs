using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Una secuencia de la historia (intro, interludio o final): lista de diapositivas
/// con imagen a pantalla completa y texto. Se crea desde:
/// Project → clic derecho → Create → Momias → Historia
///
/// Los assets viven en Assets/Resources/Historia/ y se cargan por nombre
/// (Intro, Interludio_1, Interludio_2, Interludio_3, Final) con Historia.Reproducir().
/// Para cambiar una imagen o un texto basta con editar el asset en el inspector.
/// </summary>
[CreateAssetMenu(fileName = "Historia", menuName = "Momias/Historia")]
public class HistoriaData : ScriptableObject
{
    [System.Serializable]
    public class Diapositiva
    {
        [Tooltip("Imagen a pantalla completa (16:9). Si esta vacia se muestra fondo negro.")]
        public Sprite imagen;

        [TextArea(2, 4)]
        [Tooltip("Texto que aparece abajo, letra por letra.")]
        public string texto;
    }

    public List<Diapositiva> diapositivas = new List<Diapositiva>();

    [Tooltip("Opcional: musica durante esta historia. Si esta vacia se usa la musica del menu.")]
    public AudioClip musica;
}
