using UnityEngine;

/// <summary>
/// Momia Corredora: hereda toda la IA de MomiaCaminante pero es MAS RAPIDA.
/// Siempre arranca corriendo hacia Diego (no tiene estado idle real).
/// Su gracia es la velocidad: llega rapido, pega mas duro, pero es fragil.
///
/// Stats recomendados en el SO: vida=2, vel=3.5, danio=2, escala=1.0.
/// </summary>
public class MomiaCorredora : MomiaCaminante
{
    protected override void Start()
    {
        base.Start();
        // La corredora arranca directo en "Corriendo" para que se vea su sprint
        if (anim != null)
        {
            foreach (var p in anim.parameters)
            {
                if (p.name == "Corriendo" && p.type == AnimatorControllerParameterType.Bool)
                {
                    anim.SetBool("Corriendo", true);
                    break;
                }
            }
        }
    }
}
