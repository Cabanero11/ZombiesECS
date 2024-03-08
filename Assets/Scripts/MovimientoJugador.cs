using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    public float velocidadMovimiento = 5f; // Velocidad de movimiento del jugador

    void Update()
    {
        // Obtener la entrada del teclado
        float movimientoHorizontal = Input.GetAxis("Horizontal");
        float movimientoVertical = Input.GetAxis("Vertical");

        // Calcular el vector de movimiento
        Vector3 movimiento = new Vector3(movimientoHorizontal, 0f, movimientoVertical) * velocidadMovimiento * Time.deltaTime;

        // Aplicar el movimiento al jugador relativo al espacio local
        transform.Translate(movimiento);
    }
}
