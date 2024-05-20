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

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //references
        entityManager = state.EntityManager;
        playerEntity = SystemAPI.GetSingletonEntity<DisparoData>();
        inputEntity = SystemAPI.GetSingletonEntity<InputMono>();

        //components
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

        // Limitar la rotaci�n en el eje X entre -90 y 90 grados
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Obtener la entrada del teclado para movimiento
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Calcular la direcci�n de movimiento utilizando los ejes Right y Forward del transform del jugador
        float3 moveDirection = math.mul(quaternion.Euler(0, yRotation, 0), new float3(horizontalInput, 0, verticalInput));

        // Verificar si la direcci�n de movimiento es un vector v�lido
        if (math.lengthsq(moveDirection) > 0f)
        {
            // Normalizar la direcci�n de movimiento para mantener una velocidad constante en todas las direcciones
            moveDirection = math.normalize(moveDirection);
        }

        // Calcular el desplazamiento basado en la direcci�n de movimiento y la velocidad
        // Detectar si la tecla LeftShift est� siendo presionada
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Ajustar la velocidad de movimiento
        float currentSpeed = isSprinting ? playerComponent.velocidadJugador + playerComponent.incrementoVelocidad + 10f : playerComponent.velocidadJugador + playerComponent.incrementoVelocidad;
        //PlayerDa�oData playerDa�oData = entityManager.GetComponentData<PlayerDa�oData>(playerEntity);
        //playerDa�oData.velocidadJugador = isSprinting ? 25f : 15f;


        float3 movimiento = moveDirection * currentSpeed * SystemAPI.Time.DeltaTime;

        // Actualizar la posici�n del jugador sumando el desplazamiento
        playerTransform.Position += movimiento;

        // Actualizar la rotaci�n del jugador
        playerTransform.Rotation = quaternion.Euler(0, yRotation, 0);

        // Actualizar el componente de transformaci�n de la entidad del jugador
        entityManager.SetComponentData(playerEntity, playerTransform);
        // Me faltaba poner el set DisparoData
        entityManager.SetComponentData(playerEntity, playerComponent);

        // Actualizar la posici�n y rotaci�n de la c�mara
        CameraSingleton cameraSingleton = CameraSingleton.Instance;
        if (cameraSingleton != null)
        {
            // La c�mara sigue exactamente la posici�n y rotaci�n del jugador
            cameraSingleton.transform.position = playerTransform.Position;
            cameraSingleton.transform.rotation = quaternion.Euler(xRotation, yRotation, 0);
        }

        

    }

    [BurstCompile]
    private void Disparar(ref SystemState state)
    {

        if (inputComponent.disparoIniciar)
        {
            for (int i = 0; i < playerComponent.numeroBalasPorDisparo; i++)
            {
                EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

                Entity bulletEntity = entityManager.Instantiate(playerComponent.balaPrefab);

                // Inicializar BalasData y BalasTiempoMono

                entityCommandBuffer.AddComponent(bulletEntity, new BalasData
                {
                    velocidadBala = 30f,
                    da�oBala = 5f
                });

                entityCommandBuffer.AddComponent(bulletEntity, new BalasTiempoMono
                {
                    balasTiempoDesaparicion = 2.0f
                });

                LocalTransform balasTransform = entityManager.GetComponentData<LocalTransform>(bulletEntity);
                LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);

                float offsetY = 1.3f;

                // Colocar la bala en la posici�n del punto de disparo y dirigirla hacia adelante

                balasTransform.Position.x = playerTransform.Position.x;
                balasTransform.Position.y = playerTransform.Position.y + offsetY;
                balasTransform.Position.z = playerTransform.Position.z;

                balasTransform.Rotation = playerTransform.Rotation;

                // Agregar un desplazamiento aleatorio en el plano XY
                float2 randomOffset = UnityEngine.Random.insideUnitCircle * playerComponent.balasSpread;
                float3 offset = new(randomOffset.x, randomOffset.y, 0);

                // Sumar el desplazamiento al punto de disparo para dispersar las balas
                balasTransform.Position += math.mul(playerTransform.Rotation, offset);

                entityCommandBuffer.SetComponent(bulletEntity, balasTransform);

                entityCommandBuffer.Playback(entityManager);
            }
        }

    }
}
