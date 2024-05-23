using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Zombies;

public partial class CamaraSystem : SystemBase
{
    private EntityManager entityManager;
    private float bobFrecuencia = 2f;
    private float bobAmplitudVertical = 0.08f;
    private float bobAmplitudHorizontal = 0.05f;
    private float bobbingTiempo = 0f;
    float xRotation, yRotation;



    protected override void OnCreate()
    {
        // Inicialización si es necesaria
    }

    protected override void OnUpdate()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var jugadorEntidad = SystemAPI.GetSingletonEntity<DisparoData>();

        float mouseX = Input.GetAxisRaw("Mouse X") * SystemAPI.Time.DeltaTime * 10f;
        float mouseY = Input.GetAxisRaw("Mouse Y") * SystemAPI.Time.DeltaTime * 4f;

        yRotation += mouseX;
        xRotation -= mouseY;

        // Obtener la entrada del teclado para movimiento
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        float strafeInput = 0f;

        // Detectar input para strafe (movimiento lateral)
        if (Input.GetKey(KeyCode.Q))
        {
            strafeInput = -1f;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            strafeInput = 1f;
        }

        float3 moveDirection = math.mul(quaternion.Euler(0, yRotation, 0), new float3(horizontalInput + strafeInput, 0, verticalInput));


        LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(jugadorEntidad);

        var cameraSingleton = CameraSingleton.Instance;

        if (cameraSingleton == null)
            return;

        // Incrementar el tiempo de bobbing
        bobbingTiempo += SystemAPI.Time.DeltaTime;

        // Verificar si se está moviendo el jugador
        if (math.lengthsq(moveDirection) > 0f)
        {
            // Calcular el desplazamiento de bobbing vertical y horizontal
            float bobbingOffsetY = Mathf.Sin(bobbingTiempo * bobFrecuencia) * bobAmplitudVertical;
            float bobbingOffsetX = Mathf.Cos(bobbingTiempo * bobFrecuencia) * bobAmplitudHorizontal;

            // Ajustar la posición de la cámara para estar a la altura del jugador con efecto de bobbing
            Vector3 cameraPosition = playerTransform.Position + new float3(bobbingOffsetX, cameraSingleton.AlturaSobreJugador + bobbingOffsetY, -cameraSingleton.DistanciaDetrasJugador);
            cameraSingleton.transform.SetPositionAndRotation(cameraPosition, playerTransform.Rotation);
        }
        else
        {
            // Ajustar la posición de la cámara para estar a la altura del jugador, si no se está moviendo
            Vector3 cameraPosition = playerTransform.Position + new float3(0f, cameraSingleton.AlturaSobreJugador, -cameraSingleton.DistanciaDetrasJugador);
            cameraSingleton.transform.SetPositionAndRotation(cameraPosition, playerTransform.Rotation);
        }
    }
}
