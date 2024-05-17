using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering.Universal;


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

public class CameraSingletonBaker : Baker<CameraSingleton>
{
    public override void Bake(CameraSingleton authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new CameraSingletonData
        {
            distanciaDetrasJugador = authoring.DistanciaDetrasJugador,
            alturaSobreJugador = authoring.AlturaSobreJugador,
            velocidadCamara = authoring.VelocidadCamara
        });
    }
}



