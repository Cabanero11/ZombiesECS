using UnityEngine;

namespace Zombies
{

    // CLASE PARA USAR EN CAMARA SYSTEM Y TENER UN GAMEOBJECT QUE SIGA A UNA ENTIDAD COMO UNA CAMARA
    public class CameraSingleton : MonoBehaviour
    {
        public static CameraSingleton Instance;

        [SerializeField] private float distanciaDetrasJugador = 0.5f; 
        [SerializeField] private float alturaSobreJugador;
        [SerializeField] private float velocidadCamara;

        public float DistanciaDetrasJugador => distanciaDetrasJugador;
        public float AlturaSobreJugador => alturaSobreJugador;
        public float VelocidadCamara => velocidadCamara;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
    }
}
