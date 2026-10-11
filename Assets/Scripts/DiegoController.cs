using UnityEngine;

public class DiegoController : MonoBehaviour
{
    public float velocidad = 5f;
    private float movimientoX;
    private Rigidbody2D rb;
    
    // 1. Declaramos la variable del Animator
    private Animator anim; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // 2. Conectamos la variable al componente de Diego
        anim = GetComponent<Animator>(); 
    }

    void Update()
    {
        movimientoX = Input.GetAxisRaw("Horizontal");

        if (movimientoX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (movimientoX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // 3. Le enviamos el valor al Animator. 
        // Usamos Mathf.Abs para que siempre sea positivo (si vas a la izquierda es -1, pero se vuelve 1).
        anim.SetFloat("velocidadAbsoluta", Mathf.Abs(movimientoX));
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movimientoX * velocidad, rb.linearVelocity.y);
    }
}