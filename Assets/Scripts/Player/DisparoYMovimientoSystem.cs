using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using Zombies;
using Unity.Collections;
using UnityEngine.EventSystems;

[BurstCompile]
public partial struct DisparoYMovimientoSystem : ISystem
{
    private EntityManager entityManager;
    private Entity playerEntity;
    private Entity inputEntity;

    private DisparoData playerComponent;
    private InputMono inputComponent;

    public float sensibilityX;
    public float sensibilityY;
    float xRotation, yRotation;

    // Limites del mapa
    private float3 limiteMin;
    private float3 limiteMax;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Inicializar los límites del mapa
        limiteMin = new float3(-75f, 0f, -75f);
        limiteMax = new float3(75f, 0f, 75f);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Referencias
        entityManager = state.EntityManager;
        playerEntity = SystemAPI.GetSingletonEntity<DisparoData>();
        inputEntity = SystemAPI.GetSingletonEntity<InputMono>();

        // Componentes
        playerComponent = entityManager.GetComponentData<DisparoData>(playerEntity);
        inputComponent = entityManager.GetComponentData<InputMono>(inputEntity);

        Move(ref state);
        Disparar(ref state);
    }

    private void Move(ref SystemState state)
    {
        // Obtener el transform del jugador
        LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);

        sensibilityX = 10f;
        sensibilityY = 4f;

        float mouseX = Input.GetAxisRaw("Mouse X") * SystemAPI.Time.DeltaTime * sensibilityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * SystemAPI.Time.DeltaTime * sensibilityY;

        yRotation += mouseX;
        xRotation -= mouseY;

        // Limitar la rotación en el eje X entre -90 y 90 grados
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

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

        // Calcular la dirección de movimiento utilizando los ejes Right y Forward del transform del jugador
        float3 moveDirection = math.mul(quaternion.Euler(0, yRotation, 0), new float3(horizontalInput + strafeInput, 0, verticalInput));

        // Verificar si la dirección de movimiento es un vector válido
        if (math.lengthsq(moveDirection) > 0f)
        {
            // Normalizar la dirección de movimiento para mantener una velocidad constante en todas las direcciones
            moveDirection = math.normalize(moveDirection);
        }

        // Detectar si la tecla LeftShift está siendo presionada
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Ajustar la velocidad de movimiento
        float currentSpeed = isSprinting ? playerComponent.velocidadJugador + playerComponent.incrementoVelocidad + 7f : playerComponent.velocidadJugador + playerComponent.incrementoVelocidad;

        // Calcular el desplazamiento basado en la dirección de movimiento y la velocidad
        float3 nuevoMovimiento = moveDirection * currentSpeed * SystemAPI.Time.DeltaTime;
        float3 nuevaPosicion = playerTransform.Position + nuevoMovimiento;

        // Comprobar límites del mapa
        nuevaPosicion = ComprobarLimitesMapa(nuevaPosicion);

        // Actualizar la posición del jugador sumando el desplazamiento
        playerTransform.Position = nuevaPosicion;

        // Actualizar la rotación del jugador
        playerTransform.Rotation = quaternion.Euler(0, yRotation, 0);

        // Actualizar el componente de transformación de la entidad del jugador
        entityManager.SetComponentData(playerEntity, playerTransform);
        // Me faltaba poner el set DisparoData
        entityManager.SetComponentData(playerEntity, playerComponent);

        /**
        // Actualizar la posición y rotación de la cámara
        CameraSingleton cameraSingleton = CameraSingleton.Instance;

        if (cameraSingleton != null)
        {
            // La cámara sigue la posición y rotación del jugador, añadiendo el efecto de bobbing
            cameraSingleton.transform.position = playerTransform.Position;
            cameraSingleton.transform.rotation = quaternion.Euler(xRotation, yRotation, 0);
        }
        */
    }

    [BurstCompile]
    private void Disparar(ref SystemState state)
    {
        // Actualizar el temporizador de disparo
        playerComponent.temporizadorDisparo -= SystemAPI.Time.DeltaTime;

        // Verificar si es tiempo de disparar
        if (inputComponent.disparoIniciar && playerComponent.temporizadorDisparo <= 0f)
        {
            // Crear un NativeArray para las balas
            var balasEntities = new NativeArray<Entity>(playerComponent.numeroBalasPorDisparo, Allocator.Temp);
            entityManager.Instantiate(playerComponent.balaPrefab, balasEntities);

            for (int i = 0; i < playerComponent.numeroBalasPorDisparo; i++)
            {
                Entity bulletEntity = balasEntities[i];

                // Inicializar BalasData y BalasTiempoMono
                entityManager.AddComponentData(bulletEntity, new BalasData
                {
                    velocidadBala = 30f,
                    dañoBala = 2f
                });

                entityManager.AddComponentData(bulletEntity, new BalasTiempoMono
                {
                    balasTiempoDesaparicion = 1.8f
                });

                LocalTransform balasTransform = entityManager.GetComponentData<LocalTransform>(bulletEntity);
                LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);

                float offsetY = 1.3f;

                // Colocar la bala en la posición del punto de disparo y dirigirla hacia adelante
                balasTransform.Position.x = playerTransform.Position.x;
                balasTransform.Position.y = playerTransform.Position.y + offsetY;
                balasTransform.Position.z = playerTransform.Position.z;
                balasTransform.Rotation = playerTransform.Rotation;

                // Agregar un desplazamiento aleatorio en el plano XY
                float2 randomOffset = UnityEngine.Random.insideUnitCircle * playerComponent.balasSpread;
                float3 offset = new(randomOffset.x, randomOffset.y, 0);

                // Sumar el desplazamiento al punto de disparo para dispersar las balas
                balasTransform.Position += math.mul(playerTransform.Rotation, offset);

                entityManager.SetComponentData(bulletEntity, balasTransform);
            }

            // Liberar el NativeArray
            // Liberar el NativeArray
            balasEntities.Dispose();

            // Sonido de Disaparo
            //GameManager.Instance.PlayDisparoSonido();

            // Reiniciar el temporizador de disparo
            playerComponent.temporizadorDisparo = playerComponent.cooldownDisparo;
        }

        // Guardar los cambios en el componente
        entityManager.SetComponentData(playerEntity, playerComponent);
    }

    // Comprueba los limites del mapa en un Cuadrado (lo que quiero)
    private readonly float3 ComprobarLimitesMapa(float3 posicion)
    {
        // Limitar la posición del jugador a los límites del mapa
        posicion.x = math.clamp(posicion.x, limiteMin.x, limiteMax.x);
        posicion.y = math.clamp(posicion.y, limiteMin.y, limiteMax.y);
        posicion.z = math.clamp(posicion.z, limiteMin.z, limiteMax.z);
        return posicion;
    }
}
